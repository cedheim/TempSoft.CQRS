using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.ValueObjects;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public interface ILocalInformationCommand : IEntityCommand
    {
        Culture Culture { get; }
    }
}