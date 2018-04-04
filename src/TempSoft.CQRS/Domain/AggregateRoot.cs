using System;
using System.Collections.Generic;
using System.Linq;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Domain
{
    public abstract class AggregateRoot<T> : IAggregateRoot where T : AggregateRoot<T>
    {
        private static readonly Dictionary<Type, Action<T, ICommand>> CommandHandlers = new Dictionary<Type, Action<T, ICommand>>();
        private static readonly Dictionary<Type, Action<T, IEvent>> EventHandlers = new Dictionary<Type, Action<T, IEvent>>();

        // command registry
        private readonly HashSet<Guid> _commandIds = new HashSet<Guid>();

        // session cache
        private readonly List<ICommand> _processedCommands = new List<ICommand>();
        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        
        static AggregateRoot()
        {
            InitializeEventHandlers();
            InitializeCommandHandlers();
        }

        public Guid Id { get; protected set; }

        public int Version { get; private set; }

        public abstract void Initialize(Guid id);
        
        public void Handle(ICommand command)
        {
            if (IsDuplicateCommand(command))
            {
                return;
            }

            var type = command.GetType();
            if (CommandHandlers.TryGetValue(type, out var caller))
            {
                caller((T) this, command);
                _processedCommands.Add(command);
            }
            else
            {
                throw new MissingCommandHandlerException($"No command handler found on {typeof(T).Name} for command {type.Name}");
            }
        }

        public void LoadFrom(IEnumerable<IEvent> events, IEnumerable<Guid> commandIds)
        {
            foreach (var @event in events)
            {
                if (@event.Version != (Version + 1))
                {
                    throw new EventsOutOfOrderException($"Unable to load events, expected version {Version + 1} but was {@event.Version}");
                }

                Version = @event.Version;

                ApplyEvent(@event);
            }

            foreach (var commandId in commandIds)
            {
                _commandIds.Add(commandId);
            }
        }

        public Commit Commit()
        {
            var eventsToCommit = _uncommitedEvents.ToArray();
            _uncommitedEvents.Clear();

            var commandIdsToRegister = _processedCommands.Select(cmd => cmd.Id).ToArray();
            foreach (var commandId in commandIdsToRegister)
            {
                _commandIds.Add(commandId);
            }
            _processedCommands.Clear();

            return new Commit(Id, eventsToCommit, commandIdsToRegister);
        }

        public void ApplyChange(IEvent @event)
        {
            var type = @event.GetType();

            // validate event.
            if (Version == 0 && !(@event is IInitializationEvent))
            {
                throw new DomainEventOnUninitializedAggregateException($"Domain event of type {type.Name} on uninitialized root.");

            }

            if (@event is IInitializationEvent)
            {
                if (Version != 0)
                {
                    throw new InitializationOfAlreadyInitializedAggregateException($"Tried to initialize root that already is initialized.");
                }

                Id = @event.AggregateRootId;
            }
            else
            {
                @event.AggregateRootId = Id;
            }

            ++Version;
            @event.Version = Version;

            ApplyEvent(@event);
            
            _uncommitedEvents.Add(@event);
        }

        [EventHandler(typeof(IInitializationEvent))]
        private void Apply(IInitializationEvent e)
        {
            Id = e.AggregateRootId;
        }

        protected virtual void ApplyEvent(IEvent @event)
        {
            var type = @event.GetType();
            if (EventHandlers.TryGetValue(type, out var handler))
            {
                handler((T)this, @event);
            }
            else
            {
                throw new MissingEventHandlerException($"Aggregate root of type {typeof(T).Name} missing event handler for {type.Name}");
            }
        }

        private static void InitializeEventHandlers()
        {
            // get all methods with event handler attribute decoration.
            foreach (var eventHandler in typeof(T).GetMethodsForAttribute<EventHandlerAttribute>())
            {
                foreach (var eventHandlerAttribute in eventHandler.GetCustomAttributes(typeof(EventHandlerAttribute), true).Cast<EventHandlerAttribute>())
                {
                    var caller = eventHandler.GenerateHandler<T, IEvent>(eventHandlerAttribute.For);
                    EventHandlers.Add(eventHandlerAttribute.For, caller);
                }
            }

        }

        private static void InitializeCommandHandlers()
        {
            foreach (var commandHandler in typeof(T).GetMethodsForAttribute<CommandHandlerAttribute>())
            {
                foreach (var commandHandlerAttribute in commandHandler.GetCustomAttributes(typeof(CommandHandlerAttribute), true).Cast<CommandHandlerAttribute>())
                {
                    var caller = commandHandler.GenerateHandler<T, ICommand>(commandHandlerAttribute.For);
                    CommandHandlers.Add(commandHandlerAttribute.For, caller);
                }
            }
        }
        
        private bool IsDuplicateCommand(ICommand command)
        {
            return _commandIds.Contains(command.Id) || _processedCommands.Any(cmd => cmd.Id == command.Id);
        }
    }
}