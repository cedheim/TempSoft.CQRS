using System;
using Newtonsoft.Json;
using TempSoft.CQRS.CosmosDb.Common;

namespace TempSoft.CQRS.CosmosDb.Commands
{
    public class CommandRegistryWrapper : StorageBase
    {
        public const string DocumentTypeName = "command";

        [JsonConstructor]
        private CommandRegistryWrapper()
        {
        }

        public CommandRegistryWrapper(string aggregateRootId, Guid commandId)
            : base(aggregateRootId, DocumentTypeName)
        {
            CommandId = commandId;
            Id = CreateIdentifier(commandId);
        }
        
        public Guid CommandId { get; set; }

        public static string CreateIdentifier(Guid commandId)
        {
            return IdentityFormatter.Format(commandId.ToString(), DocumentTypeName);
        }
    }
}