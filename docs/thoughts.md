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
