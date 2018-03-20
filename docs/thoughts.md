# The event bus structure
* There is a service fabric service.
    * It receieves events and calls the event bus.
* There is an event bus class.
    * It is resposible for dispatching events to the query builders?
* Aggregate root repository
    * Should the aggregate root repository be dependent on the event bus?
    * Maybe the repository should dispatch directly to the service?
        * More logic in the repository, is that good? Not good but it reduces dependency on the Event Bus.
        * It makes it necessary to have a specific implementation of repository for Service Fabric.
        * Otherwise we need to separate implementations of Event Bus depending on if it is in the Event Bus Service or used from the aggregate root repository?
        * Skip the CommandRegistry and EventStore completely for now? (nah... they are good.)
* What is a query builder?
    * A query builder is responsbile for building query models.
    * Is there an actor for each query model?


* SHould each query builder be an actor? Or should it be some kund of service.
    * if it is an actor per query type we could get congestion if several different event bus:es trigger events at the same time.
    * Should we have an stateful service then with a queue? How is that different from just populating the query models in the event bus?


* What should the abstraction be?
    * We want the event bus to publish events to event listeners.
    * How should this be handled?
    * We have a QueryBuilderRegistry, this is the registry which contains the name of all query builders.
    * How should QueryBuilders be partitioned?
        * If query builders are actors can we get congestion?
        * There should also be a query object repository.



* Each QueryBuilder is an actor.
    * You register QueryBuilders in the QueryBuilder registry.
    * You can list query builders in the registry.
    * You can ask the query builder for listeners of a certain event type.
    * The event bus is responsible for dispatching these events to the query builder actors.
    * Each query builder actor is the identified by the event type?
        * No, each query builder can have multiple event types, os it needs to be something else. Maybe we assign a Guid to query builders on creation... No since there are different runtimes query builders will get different Guids.
        * Go by type name on query builder.
