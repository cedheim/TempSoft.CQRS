using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class SetIdentifier : EntityCommandBase, IIdentifierCommand
    {
        public SetIdentifier(string entityId, string value)
        {
            Value = value;
            EntityId = entityId.ToUpper().Trim();
        }

        public string Value { get; private set; }
    }
}