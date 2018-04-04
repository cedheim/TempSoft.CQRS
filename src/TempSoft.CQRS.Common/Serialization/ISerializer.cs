using System;

namespace TempSoft.CQRS.Common.Serialization
{
    public interface ISerializer
    {
        string Serialize(object o);

        object Deserialize(string data, Type type);
    }
}