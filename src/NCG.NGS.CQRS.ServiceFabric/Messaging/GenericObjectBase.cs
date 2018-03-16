using System;
using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.ServiceFabric.Messaging
{
    [DataContract]
    public abstract class GenericObjectBase
    {
        [DataMember(Name = "Type")]
        private string _type;

        [DataMember(Name = "Body")]
        private string _body;
        
        [IgnoreDataMember] private Type _deserializedType;
        [IgnoreDataMember] private object _deserializedBody;

        protected GenericObjectBase()
        {
            _deserializedBody = default(object);
            _deserializedType = default(Type);
        }

        protected GenericObjectBase(object body)
        {
            _deserializedType = body.GetType();
            _deserializedBody = body;

            Serialize();
        }

        [IgnoreDataMember]
        public object Body => _deserializedBody ?? (_deserializedBody = Newtonsoft.Json.JsonConvert.DeserializeObject(_body, Type));

        [IgnoreDataMember]
        public Type Type => _deserializedType ?? (_deserializedType = Type.GetType(_type));

        private void Serialize()
        {
            // Type name without version qualifier. Not sure how to handle this properly.
            _type = $"{_deserializedType.FullName}, {_deserializedType.Assembly.GetName().Name}";
            _body = Newtonsoft.Json.JsonConvert.SerializeObject(_deserializedBody);
        }
    }
}