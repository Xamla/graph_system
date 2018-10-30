using Newtonsoft.Json;
using System;
using Xamla.Robotics.Types.Models;

namespace Xamla.Robotics.Types.JsonConverters
{
    public class PoseJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(Pose);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            serializer.Deserialize<PoseModel>(reader)?.ToPose();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((Pose)value)?.ToModel());
    }
}
