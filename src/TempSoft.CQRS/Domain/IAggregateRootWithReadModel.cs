namespace TempSoft.CQRS.Domain
{
    public interface IAggregateRootWithReadModel : IAggregateRoot
    {
        IAggregateRootReadModel GetReadModel();
    }
}