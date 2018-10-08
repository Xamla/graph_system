using System;
using Xamla.Graph.Models;
using Newtonsoft.Json.Linq;

namespace Xamla.Graph
{
    public struct ValidationResult
    {
        public static readonly ValidationResult Valid = new ValidationResult() { IsValid = true };

        public bool IsValid;
        public string Message;

        public ValidationResult(string message)
        {
            IsValid = false;
            Message = message;
        }
    }

    public interface IPinDataType
    {
        PinTypeCode TypeCode { get; }
        string Editor { get; }
        object Parameters { get; }
        Type UnderlyingType { get; }
        string ShortUnderlyingTypeName { get; }

        bool IsAssignableFrom(IPinDataType dataType);
        bool TryConvert(object source, out object destination);
        ValidationResult Validate(object value);
        object DefaultValue { get; }
        PinDataTypeModel ToModel();

        object Deserialize(JToken token);
        JToken Serialize(object value);
    }
}
