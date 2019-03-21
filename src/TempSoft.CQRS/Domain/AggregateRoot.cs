using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Domain
{
    public abstract class AggregateRoot<T> : IAggregateRoot where T : AggregateRoot<T>
    {
        private static readonly Dictionary<Type, Func<T, ICommand, CancellationToken, Task>> CommandHandlers =
            new Dictionary<Type, Func<T, ICommand, CancellationToken, Task>>();

        private static readonly Dictionary<Type, Action<T, IEvent>> EventHandlers =
            new Dictionary<Type, Action<T, IEvent>>();

        private static readonly Dictionary<Type, MethodInfo> EntityCommandHandler = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<Type, MethodInfo> EntityEventHandler = new Dictionary<Type, MethodInfo>();
        private static readonly string AggregateRootTypeName = typeof(T).Name;

        // command registry
        private readonly HashSet<Guid> _commandIds = new HashSet<Guid>();

        // registered entities
        private readonly Dictionary<string, IEntity> _entities = new Dictionary<string, IEntity>();

        // session cache
        private readonly List<ICommand> _processedCommands = new List<ICommand>();
        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        protected readonly ICommandRouter CommandRouter;

        static AggregateRoot()
        {
            InitializeEventHandlers();
            InitializeCommandHandlers();
        }

        public string Id { get; set; }

        public int Version { get; private set; }

        public async Task Handle(ICommand command, CancellationToken cancellationToken)
        {
            if (IsDuplicateCommand(command)) return;

            if (command is IEntityCommand entityCommand)
            {
                if (!_entities.TryGetValue(entityCommand.EntityId, out var entity))
                {
                    entity = MissingEntityHandler(entityCommand);
                }

                if (EntityCommandHandler.TryGetValue(entity.GetType(), out var method))
                {
                    var task = (Task) method.Invoke(this, new object[] {entity, entityCommand, cancellationToken});
                    await task;
                }
                else
                {
                    throw new MissingEntityException($"Unable to find command handler for {entityCommand.GetType()} for entity {entityCommand.EntityId}.");
                }
            }
            else
            {
                var type = command.GetType();
                if (CommandHandlers.TryGetValue(type, out var caller))
                {
                    await caller((T) this, command, cancellationToken);
                    _processedCommands.Add(command);
                }
                else
                {
                    throw new MissingCommandHandlerException(
                        $"No command handler found on {typeof(T).Name} for command {type.Name}");
                }
            }
        }

        public void LoadFrom(IEnumerable<IEvent> events, IEnumerable<Guid> commandIds)
        {
            foreach (var @event in events)
            {
                if (@event.Version != Version + 1)
                    throw new EventsOutOfOrderException(
                        $"Unable to load events, expected version {Version + 1} but was {@event.Version}");

                Version = @event.Version;

                ApplyEvent(@event);
            }

            foreach (var commandId in commandIds) _commandIds.Add(commandId);
        }

        public Commit Commit()
        {
            var eventsToCommit = _uncommitedEvents.ToArray();
            _uncommitedEvents.Clear();

            var commandIdsToRegister = _processedCommands.Select(cmd => cmd.Id).ToArray();
            foreach (var commandId in commandIdsToRegister) _commandIds.Add(commandId);
            _processedCommands.Clear();

            return new Commit(Id, eventsToCommit, commandIdsToRegister);
        }

        public void ApplyChange(IEvent @event)
        {
            // validate event.
            if (Version == 0 && !(@event is IInitializationEvent))
                throw new DomainEventOnUninitializedAggregateException(
                    $"Domain event of type {@event.GetType().Name} on uninitialized root.");

            if (@event is IInitializationEvent)
            {
                if (Version != 0)
                    throw new InitializationOfAlreadyInitializedAggregateException(
                        $"Tried to initialize root that already is initialized.");
            }

            @event.AggregateRootId = Id;

            ++Version;

            @event.Version = Version;
            @event.EventGroup = AggregateRootTypeName;

            ApplyEvent(@event);

            _uncommitedEvents.Add(@event);
        }

        protected virtual IEntity MissingEntityHandler(IEntityCommand command)
        {
            throw new MissingEntityException($"Entity with id {command.EntityId} not found.");
        }

        private async Task HandleCommandForEntity<TEntity>(TEntity entity, IEntityCommand command,
            CancellationToken cancellationToken) where TEntity : Entity<TEntity>
        {
            var type = command.GetType();
            if (Entity<TEntity>.CommandHandlers.TryGetValue(type, out var caller))
            {
                await caller(entity, command, cancellationToken);
                _processedCommands.Add(command);
            }
            else
            {
                throw new MissingCommandHandlerException(
                    $"No command handler found on {typeof(TEntity).Name} for command {type.Name}");
            }
        }

        private void RegisterEntity(IEntity entity)
        {
            if (_entities.ContainsKey(entity.Id))
                throw new EntityAlreadyExistsException($"An entity with id {entity.Id} already exists.");
            if (string.IsNullOrEmpty(entity.Id))
                throw new EntityMissingIdException($"Tried to register an entity without a proper id.");

            _entities.Add(entity.Id, entity);
        }

        private void ApplyEvent(IEvent @event)
        {
            if (@event is IEntityEvent entityEvent)
            {
                if (_entities.TryGetValue(entityEvent.EntityId, out var entity) &&
                    EntityEventHandler.TryGetValue(entity.GetType(), out var method))
                    method.Invoke(this, new object[] {entity, entityEvent});
                else
                    throw new MissingEntityException($"Entity with id {entityEvent.EntityId} not found.");
            }
            else
            {
                var type = @event.GetType();
                if (EventHandlers.TryGetValue(type, out var handler))
                    handler((T) this, @event);
                else
                    throw new MissingEventHandlerException(
                        $"Aggregate root of type {typeof(T).Name} missing event handler for {type.Name}");
            }
        }

        private void ApplyEventForEntity<TEntity>(TEntity entity, IEntityEvent entityEvent)
            where TEntity : Entity<TEntity>
        {
            var type = entityEvent.GetType();
            if (Entity<TEntity>.EventHandlers.TryGetValue(type, out var handler))
                handler(entity, entityEvent);
            else
                throw new MissingEventHandlerException(
                    $"Entity {typeof(TEntity).Name} missing event handler for {type.Name}");
        }

        private static void InitializeEventHandlers()
        {
            // get all methods with event handler attribute decoration.
            foreach (var eventHandler in typeof(T).GetMethodsForAttribute<EventHandlerAttribute>())
            foreach (var eventHandlerAttribute in eventHandler.GetCustomAttributes(typeof(EventHandlerAttribute), true)
                .Cast<EventHandlerAttribute>())
            {
                var caller = eventHandler.GenerateHandler<T, IEvent>(eventHandlerAttribute.For);
                EventHandlers.Add(eventHandlerAttribute.For, caller);
            }
        }

        private static void InitializeCommandHandlers()
        {
            foreach (var commandHandler in typeof(T).GetMethodsForAttribute<CommandHandlerAttribute>())
            foreach (var commandHandlerAttribute in commandHandler
                .GetCustomAttributes(typeof(CommandHandlerAttribute), true).Cast<CommandHandlerAttribute>())
            {
                var caller = commandHandler.GenerateAsyncHandler<T, ICommand>(commandHandlerAttribute.For);
                CommandHandlers.Add(commandHandlerAttribute.For, caller);
            }
        }

        private bool IsDuplicateCommand(ICommand command)
        {
            return _commandIds.Contains(command.Id) || _processedCommands.Any(cmd => cmd.Id == command.Id);
        }

        public abstract class Entity<TEntity> : IEntity where TEntity : Entity<TEntity>
        {
            internal static readonly Dictionary<Type, Func<TEntity, IEntityCommand, CancellationToken, Task>>
                CommandHandlers = new Dictionary<Type, Func<TEntity, IEntityCommand, CancellationToken, Task>>();

            internal static readonly Dictionary<Type, Action<TEntity, IEntityEvent>> EventHandlers =
                new Dictionary<Type, Action<TEntity, IEntityEvent>>();

            protected readonly T Root;

            protected void ApplyChange(IEntityEvent @event)
            {
                @event.EntityId = Id;
                Root.ApplyChange(@event);
            }

            static Entity()
            {
                InitializeCommandHandlers();
                InitializeEventHandlers();

                CreateCommandHandlerForEntity();
                CreateApplyEventForEntity();
            }

            protected Entity(T root, string id)
            {
                Id = id;
                Root = root;
                Root.RegisterEntity(this);
            }

            public string Id { get; set; }


            private static void InitializeEventHandlers()
            {
                // get all methods with event handler attribute decoration.
                foreach (var eventHandler in typeof(TEntity).GetMethodsForAttribute<EventHandlerAttribute>())
                foreach (var eventHandlerAttribute in eventHandler
                    .GetCustomAttributes(typeof(EventHandlerAttribute), true).Cast<EventHandlerAttribute>())
                {
                    var caller = eventHandler.GenerateHandler<TEntity, IEntityEvent>(eventHandlerAttribute.For);
                    EventHandlers.Add(eventHandlerAttribute.For, caller);
                }
            }

            private static void InitializeCommandHandlers()
            {
                foreach (var commandHandler in typeof(TEntity).GetMethodsForAttribute<CommandHandlerAttribute>())
                foreach (var commandHandlerAttribute in commandHandler
                    .GetCustomAttributes(typeof(CommandHandlerAttribute), true).Cast<CommandHandlerAttribute>())
                {
                    var caller =
                        commandHandler.GenerateAsyncHandler<TEntity, IEntityCommand>(commandHandlerAttribute.For);
                    CommandHandlers.Add(commandHandlerAttribute.For, caller);
                }
            }

            private static void CreateCommandHandlerForEntity()
            {
                var method = typeof(AggregateRoot<T>)
                    .GetMethod("HandleCommandForEntity", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(typeof(TEntity));

                EntityCommandHandler.Add(typeof(TEntity), method);
            }

            private static void CreateApplyEventForEntity()
            {
                var method = typeof(AggregateRoot<T>)
                    .GetMethod("ApplyEventForEntity", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(typeof(TEntity));

                EntityEventHandler.Add(typeof(TEntity), method);
            }
        }
    }
}