using System.Runtime.Serialization;

namespace TempSoft.CQRS.Events
{
    [DataContract]
    public enum EventStreamStatus
    {
        [EnumMember] Uninitialized = 0,
        [EnumMember] Initializing = 1,
        [EnumMember] Initialized = 2
    }
}