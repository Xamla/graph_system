using Newtonsoft.Json;
using System;

namespace Xamla.Robotics.Types.JsonConverters
{
    public class JointTrajectorysJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(JointTrajectory);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            serializer.Deserialize<JointTrajectoryModel>(reader)?.ToJointTrajectory();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((JointTrajectory)value)?.ToModel());
    }
}
