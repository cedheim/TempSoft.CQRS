using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Exception;

namespace NCG.NGS.CQRS.Domain
{
    public abstract class AggregateRoot<T> : IAggregateRoot where T : AggregateRoot<T>
    {
        private static readonly Dictionary<Type, Action<T, ICommand>> CommandHandlers = new Dictionary<Type, Action<T, ICommand>>();
        private static readonly Dictionary<Type, Action<T, IEvent>> EventHandlers = new Dictionary<Type, Action<T, IEvent>>();

        private readonly HashSet<Guid> _commandIds = new HashSet<Guid>();

        static AggregateRoot()
        {
            InitializeEventHandlers();
            InitializeCommandHandlers();
        }

        private List<IEvent> _events = new List<IEvent>();

        public IEnumerable<Guid> ProcessedCommands => _commandIds;

        public Guid Id { get; protected set; }

        public int Version { get; private set; }

        public virtual void Initialize(Guid id)
        {
            this.ApplyChange(new InitializationEvent(id));
        }

        public void Handle(ICommand command)
        {
            if (command.Id != Guid.Empty && _commandIds.Contains(command.Id))
            {
                return;
            }

            var type = command.GetType();
            if (CommandHandlers.TryGetValue(type, out var caller))
            {
                caller((T) this, command);
                _commandIds.Add(command.Id);
            }
            else
            {
                throw new MissingCommandHandlerException($"No command handler found on {typeof(T).Name} for command {type.Name}");
            }
        }

        public void LoadFrom(IEnumerable<IEvent> events, IEnumerable<Guid> commandIds = default(IEnumerable<Guid>))
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

            foreach (var commandId in commandIds ?? Enumerable.Empty<Guid>())
            {
                _commandIds.Add(commandId);
            }
        }

        public IEnumerable<IEvent> Commit()
        {
            var eventsToCommit = _events;

            _events = new List<IEvent>();
            return eventsToCommit;
        }

        public void ApplyChange(IEvent @event)
        {
            var type = @event.GetType();

            // validate event.
            if (Version == 0 && !(@event is InitializationEvent))
            {
                throw new DomainEventOnUninitializedAggregateException($"Domain event of type {type.Name} on uninitialized root.");

            }
            if(Version != 0 && @event is InitializationEvent)
            {
                throw new InitializationOfAlreadyInitializedAggregateException($"Tried to initialize root that already is initialized.");
            }

            ++Version;
            @event.Version = Version;

            ApplyEvent(@event);
            
            @event.AggregateRootId = Id;

            _events.Add(@event);
        }

        [EventHandler(typeof(InitializationEvent))]
        private void Apply(InitializationEvent e)
        {
            Id = e.AggregateRootId;
        }

        private void ApplyEvent(IEvent @event)
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
            foreach (var eventHandler in GetMethodsForAttribute<EventHandlerAttribute>())
            {
                foreach (var eventHandlerAttribute in eventHandler.GetCustomAttributes(typeof(EventHandlerAttribute), true).Cast<EventHandlerAttribute>())
                {
                    var caller = GenerateCaller<IEvent>(eventHandler, eventHandlerAttribute.For);
                    EventHandlers.Add(eventHandlerAttribute.For, caller);
                }
            }

        }

        private static void InitializeCommandHandlers()
        {
            foreach (var commandHandler in GetMethodsForAttribute<CommandHandlerAttribute>())
            {
                foreach (var commandHandlerAttribute in commandHandler.GetCustomAttributes(typeof(CommandHandlerAttribute), true).Cast<CommandHandlerAttribute>())
                {
                    var caller = GenerateCaller<ICommand>(commandHandler, commandHandlerAttribute.For);
                    CommandHandlers.Add(commandHandlerAttribute.For, caller);
                }
            }
        }

        private static Action<T, TObject> GenerateCaller<TObject>(MethodInfo method, Type objectType)
        {
            var parameters = method.GetParameters();

            if (parameters.Length == 1 && parameters[0].ParameterType == objectType)
            {
                return GenerateExplicitCaller<TObject>(method, objectType);
            }

            return GenerateImplicitCaller<TObject>(method, objectType);

        }

        private static Action<T, TObject> GenerateExplicitCaller<TObject>(MethodInfo method, Type objectType)
        {
            var rootParameter = Expression.Parameter(typeof(T), "root");
            var objectParameter = Expression.Parameter(typeof(TObject), "o");

            var castParameterToObjectType = Expression.TypeAs(objectParameter, objectType);
            var eventVariable = Expression.Parameter(objectType, "e");
            var assignment = Expression.Assign(eventVariable, castParameterToObjectType);

            var rootCall = Expression.Call(rootParameter, method, eventVariable);

            var body = Expression.Block(new ParameterExpression[] { eventVariable }, new Expression[] { assignment, rootCall });
            var lambda = Expression.Lambda<Action<T, TObject>>(body, rootParameter, objectParameter);

            var action = lambda.Compile();

            return action;

        }

        private static Action<T, TObject> GenerateImplicitCaller<TObject>(MethodInfo method, Type objectType)
        {
            // declaration of action parameters.
            var rootParameter = Expression.Parameter(typeof(T), "root");
            var objectParameter = Expression.Parameter(typeof(TObject), "o");

            // cast the input event to the actual object type.
            var castParameterToObjectType = Expression.TypeAs(objectParameter, objectType);
            var eventVariable = Expression.Parameter(objectType, "e");
            var assignment = Expression.Assign(eventVariable, castParameterToObjectType);
            // generate call to root method.

            List<Expression> parameterExpressions = new List<Expression>();
            var publicProperties = objectType.GetProperties();
            var parameters = method.GetParameters();
            
            foreach (var parameter in parameters)
            {
                var matchingProperty = publicProperties.FirstOrDefault(property => string.Compare(property.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (matchingProperty == default(PropertyInfo))
                {
                    throw new System.Exception($"Unable to find a property on {objectType.Name} which matches the parameter name {parameter.Name}");
                }

                parameterExpressions.Add(Expression.Property(eventVariable, matchingProperty));
            }
            
            var rootCall = Expression.Call(rootParameter, method, parameterExpressions);

            var body = Expression.Block(new ParameterExpression[] { eventVariable }, new Expression[] { assignment, rootCall });
            var lambda = Expression.Lambda<Action<T, TObject>>(body, rootParameter, objectParameter);

            var action = lambda.Compile();

            return action;
        }

        private static IEnumerable<MethodInfo> GetMethodsForAttribute<TAttribute>() where TAttribute : Attribute
        {
            var type = typeof(T);

            while (type != default(Type))
            {
                foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TAttribute))))
                {
                    yield return methodInfo;
                }

                type = type.BaseType;
            }
            

        }
    }
}