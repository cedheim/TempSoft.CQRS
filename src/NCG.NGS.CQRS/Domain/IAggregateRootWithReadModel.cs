namespace NCG.NGS.CQRS.Domain
{
    public interface IAggregateRootWithReadModel : IAggregateRoot
    {
        IAggregateRootReadModel GetReadModel();
    }
}