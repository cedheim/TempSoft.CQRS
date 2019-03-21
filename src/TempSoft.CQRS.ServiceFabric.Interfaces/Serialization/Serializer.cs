using System;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Serialization
{
    internal static class Serializer
    {
        private static ISerializer _serializer;

        static Serializer()
        {
            DefaultSerializer = new JsonSerializer();
            _serializer = DefaultSerializer;
        }

        public static ISerializer DefaultSerializer { get; }

        public static void SetSerializer(ISerializer serializer)
        {
            _serializer = serializer ?? DefaultSerializer;
        }

        public static string Serialize(object o)
        {
            return _serializer.Serialize(o);
        }

        public static object Deserialize(string data, Type type)
        {
            return _serializer.Deserialize(data, type);
        }
    }
}