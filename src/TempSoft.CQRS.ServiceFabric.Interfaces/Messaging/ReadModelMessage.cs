using System.Collections.Generic;
using System.Runtime.Serialization;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public class ReadModelMessage : GenericMessage
    {
        public ReadModelMessage(IAggregateRootReadModel value, IEnumerable<KeyValuePair<string, object>> headers = null) : base(value, headers)
        {
        }

        private ReadModelMessage()
        {
        }

        [IgnoreDataMember] public new IAggregateRootReadModel Body => (IAggregateRootReadModel)base.Body;

        public TReadModel GetReadModel<TReadModel>() where TReadModel : IAggregateRootReadModel
        {
            return (TReadModel) Body;
        }
    }
}