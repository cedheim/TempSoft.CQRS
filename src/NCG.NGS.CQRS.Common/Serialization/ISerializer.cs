using System;

namespace NCG.NGS.CQRS.Common.Serialization
{
    public interface ISerializer
    {
        string Serialize(object o);

        object Deserialize(string data, Type type);
    }
}