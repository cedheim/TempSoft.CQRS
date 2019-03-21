using System;
using Newtonsoft.Json;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Serialization
{
    internal class JsonSerializer : ISerializer
    {
        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public object Deserialize(string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type);
        }
    }
}