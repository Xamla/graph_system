using Newtonsoft.Json;
using System;

namespace Xamla.Robotics.Types.JsonConverters
{
    public class CartesianPathJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(CartesianPath);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            serializer.Deserialize<CartesianPathModel>(reader)?.ToCartesianPath();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((CartesianPath)value)?.ToModel());
    }
}
