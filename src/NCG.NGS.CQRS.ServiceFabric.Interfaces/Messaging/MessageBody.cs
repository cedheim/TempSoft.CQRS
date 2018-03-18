using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
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