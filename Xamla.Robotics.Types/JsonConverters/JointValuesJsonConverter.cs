using Newtonsoft.Json;
using System;
using Xamla.Robotics.Types.Models;

namespace Xamla.Robotics.Types.JsonConverters
{
    public class JointValuesJsonConverter : JsonConverter
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) =>
            objectType == typeof(JointValues);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            serializer.Deserialize<JointValuesModel>(reader)?.ToJointValues();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((JointValues)value)?.ToModel());
    }
}
