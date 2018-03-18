using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public class GenericMessage
    {
        [DataMember(Name = "Body")]
        private MessageBody _body;

        [DataMember(Name = "Headers")]
        private List<MessageHeader> _headers;

        protected GenericMessage()
        {
        }

        public GenericMessage(object value, IEnumerable<KeyValuePair<string, object>> headers = default(IEnumerable<KeyValuePair<string, object>>))
        {
            _body = new MessageBody(value);
            _headers = new List<MessageHeader>();
            
            if (!object.ReferenceEquals(headers, default(IEnumerable<KeyValuePair<string, object>>)))
            {
                foreach (var kvp in headers)
                {
                    AddHeader(kvp.Key, kvp.Value);
                }
            }
        }
        
        [IgnoreDataMember] public object Body => _body.Body;

        [IgnoreDataMember] public Type Type => _body.Type;

        public void AddHeader(string name, object value)
        {
            if (_headers.Any(header => header.Name == name))
            {
                throw new ArgumentException($"Header with name \"{name}\" already exists", nameof(name));
            }

            _headers.Add(new MessageHeader(name, value));
        }

        public void UpdateOrAddHeader(string name, object value)
        {
            var existingHeader = _headers.FirstOrDefault(h => h.Name == name);
            if (!object.ReferenceEquals(existingHeader, default(MessageHeader)))
            {
                _headers.Remove(existingHeader);
            }


            _headers.Add(new MessageHeader(name, value));
        }

        public void DeleteHeader(string name)
        {
            var header = _headers.FirstOrDefault(h => h.Name == name);
            if (object.ReferenceEquals(header, default(MessageHeader)))
            {
                throw new ArgumentException($"Header with name \"{name}\" already exists", nameof(name));
            }

            _headers.Remove(header);
        }

        public bool TryGetHeader(string name, out object value)
        {
            var header = _headers.FirstOrDefault(h => h.Name == name);
            if (object.ReferenceEquals(header, default(MessageHeader)))
            {
                value = default(object);
                return false;
            }


            value = header.Body;
            return true;
        }

        [IgnoreDataMember] public IEnumerable<string> HeaderNames => _headers.Select(header => header.Name);
    }
}