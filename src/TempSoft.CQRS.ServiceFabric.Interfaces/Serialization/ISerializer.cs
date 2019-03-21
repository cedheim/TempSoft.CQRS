using System;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Serialization
{
    internal interface ISerializer
    {
        string Serialize(object o);

        object Deserialize(string data, Type type);
    }
}