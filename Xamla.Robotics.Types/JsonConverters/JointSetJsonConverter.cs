using Newtonsoft.Json;
using System;

namespace Xamla.Robotics.Types.JsonConverters
{
    public class JointSetJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(JointSet);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jointNames = serializer.Deserialize<string[]>(reader);
            return jointNames != null ? new JointSet(jointNames) : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((JointSet)value)?.ToArray());
    }
}
