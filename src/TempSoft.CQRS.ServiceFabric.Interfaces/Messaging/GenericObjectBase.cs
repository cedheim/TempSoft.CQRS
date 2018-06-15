using System;
using System.Runtime.Serialization;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Common.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract(Namespace = ContractConstants.Namespace)]
    public abstract class GenericObjectBase
    {
        [DataMember(Name = "Body")] private string _body;

        [IgnoreDataMember] private object _deserializedBody;

        [IgnoreDataMember] private Type _deserializedType;

        [DataMember(Name = "Type")] private string _type;

        protected GenericObjectBase()
        {
            _deserializedBody = default(object);
            _deserializedType = default(Type);
        }

        protected GenericObjectBase(object body)
        {
            if (object.ReferenceEquals(body, default(object)))
            {
                _deserializedType = typeof(object);
                _deserializedBody = null;
            }
            else
            {
                _deserializedType = body.GetType();
                _deserializedBody = body;
            }

            Serialize();
        }

        [IgnoreDataMember]
        public object Body => _deserializedBody ?? (_deserializedBody = Serializer.Deserialize(_body, Type));

        [IgnoreDataMember] public Type Type => _deserializedType ?? (_deserializedType = Type.GetType(_type));

        private void Serialize()
        {
            // Type name without version qualifier. Not sure how to handle this properly.
            _type = _deserializedType.ToFriendlyName();
            _body = Serializer.Serialize(_deserializedBody);
        }
    }
}