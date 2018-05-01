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


# Event Streams
An event stream is a stream of events with a certain filter.
* Should the filter be declared on creation?
* Events are ordered.
* Should be able to stream events from the beginning or from a certain point in time.
* How are event streams handled? Should they push events?
    * Are Event Streams actively pushing events or are the Projectors polling event streams?
    * Event streams are directly connected to projectors? Is it a 1:1 relationship or can we have several EventStreams for a single Projector.
* Should present a method ReadNext(CancellationToken) which returns the next event on the stream and blocks async until it comes.
* When registering each EventStream declares their filter?
* We can use named partitions.
* Basically each event stream is a stateless service? Maybe stateful, which is named? We should try to use stateless entities as far as possible.
* 


# Projectors
Projectors read from Event Streams to create Query Models.
* It doesn't have to be a query model, it can also be something like publishing events to Web Sockets.
* How do we handle projectors? Is each projector a stateless service? Or is there a "Projector" stateless service which hosts the projectors?
* Who remembers the point at which the projector last read events?

# Event Topics
We treat "Event Streams" as Topics on the Event Bus.
Each Topic can be replayed from the beginning and has a state.
Each Topic is a stateless service, which stores its state in a cosmos db.
Each Topic keeps track of a pointer to the top of its event stream.
If an topic does not have a pointer it will replay the entire event stream.
There are standard Event Topics it also possible to implement specific event streams depending on specific needs.
Should Event Topics be stateful services?

