using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract(Namespace = ContractConstants.Namespace)]
    public class QueryResultMessage : GenericMessage
    {
        public QueryResultMessage(IQueryResult value, IEnumerable<KeyValuePair<string, object>> headers = null)
            : base(value, headers)
        {
        }

        [IgnoreDataMember]
        public new IQueryResult Body => (IQueryResult)base.Body;

        public TQueryResult GetQueryResult<TQueryResult>() where TQueryResult : IQueryResult
        {
            return (TQueryResult)Body;
        }
    }
}