using Newtonsoft.Json;
using System;
using Xamla.Robotics.Types.Models;

namespace Xamla.Robotics.Types.JsonConverters
{
    public class PlanParametersJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(PlanParameters);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            serializer.Deserialize<PlanParametersModel>(reader)?.ToPlanParameters();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((PlanParameters)value)?.ToModel());
    }
}
