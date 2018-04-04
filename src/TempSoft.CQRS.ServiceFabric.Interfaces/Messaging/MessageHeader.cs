using System.Runtime.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public class MessageHeader : GenericObjectBase
    {
        protected MessageHeader()
        {
        }

        public MessageHeader(string name, object body)
            : base(body)
        {
            Name = name;
        }



        [DataMember]
        public string Name { get; private set; }
    }
}