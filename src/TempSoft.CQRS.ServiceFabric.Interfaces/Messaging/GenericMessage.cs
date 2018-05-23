using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public class GenericMessage : MessageBase
    {
        [DataMember(Name = "Body")] private MessageBody _body;

        protected GenericMessage()
        {
        }

        public GenericMessage(object value,
            IEnumerable<KeyValuePair<string, object>> headers = default(IEnumerable<KeyValuePair<string, object>>))
            : base(headers)
        {
            _body = new MessageBody(value);
        }

        [IgnoreDataMember] public object Body => _body.Body;

        [IgnoreDataMember] public Type Type => _body.Type;
    }
}