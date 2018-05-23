using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public abstract class MessageBase
    {
        [DataMember(Name = "Headers")] private List<MessageHeader> _headers;

        protected MessageBase()
        {
        }

        protected MessageBase(
            IEnumerable<KeyValuePair<string, object>> headers = default(IEnumerable<KeyValuePair<string, object>>))
        {
            _headers = new List<MessageHeader>();

            Id = Guid.NewGuid();

            if (!ReferenceEquals(headers, default(IEnumerable<KeyValuePair<string, object>>)))
                foreach (var kvp in headers)
                    SetHeader(kvp.Key, kvp.Value);
        }

        [DataMember] public Guid Id { get; private set; }

        [IgnoreDataMember] public IEnumerable<string> HeaderNames => _headers.Select(header => header.Name);

        public void SetHeader(string name, object value)
        {
            var existingHeader = _headers.FirstOrDefault(h => h.Name == name);
            if (!ReferenceEquals(existingHeader, default(MessageHeader))) _headers.Remove(existingHeader);


            _headers.Add(new MessageHeader(name, value));
        }

        public void DeleteHeader(string name)
        {
            var header = _headers.FirstOrDefault(h => h.Name == name);
            if (ReferenceEquals(header, default(MessageHeader)))
                throw new ArgumentException($"Header with name \"{name}\" already exists", nameof(name));

            _headers.Remove(header);
        }

        public bool TryGetHeader(string name, out object value)
        {
            var header = _headers.FirstOrDefault(h => h.Name == name);
            if (ReferenceEquals(header, default(MessageHeader)))
            {
                value = default(object);
                return false;
            }


            value = header.Body;
            return true;
        }
    }
}