using System;

namespace NCG.NGS.CQRS.Common.Serialization
{
    internal class JsonSerializer : ISerializer
    {
        public string Serialize(object o)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o);
        }

        public object Deserialize(string data, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(data, type);
        }
    }
}