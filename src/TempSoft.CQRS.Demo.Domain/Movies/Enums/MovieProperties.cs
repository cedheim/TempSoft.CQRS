using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Movies.Enums
{
    [DataContract]
    public enum MovieProperties
    {
        [EnumMember] Has3D = 1,
        [EnumMember] HasIMAX = 2,
        [EnumMember] HasTHX = 3
    }
}