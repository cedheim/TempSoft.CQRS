using System.Runtime.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract(Namespace = ContractConstants.Namespace)]
    public class MessageBody : GenericObjectBase
    {
        public MessageBody(object body) : base(body)
        {
        }

        protected MessageBody()
        {
        }
    }
}