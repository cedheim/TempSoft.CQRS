using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Enums
{
    [DataContract]
    public enum AuditoriumProperties
    {
        [EnumMember] Is3D = 1,
        [EnumMember] IsIMAX = 2,
        [EnumMember] IsTHX = 3
    }
}