namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public interface IFabricClient
    {
        IQueryClient QueryManager { get; }
    }
}