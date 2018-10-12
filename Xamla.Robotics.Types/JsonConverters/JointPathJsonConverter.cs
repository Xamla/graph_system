using Newtonsoft.Json;
using System;

namespace Xamla.Robotics.Types.JsonConverters
{
    public class JointPathJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(JointPath);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            serializer.Deserialize<JointPathModel>(reader)?.ToJointPath();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((JointPath)value)?.ToModel());
    }
}
