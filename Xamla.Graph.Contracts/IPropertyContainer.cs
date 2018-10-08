using System;
using System.Collections.Generic;

namespace Xamla.Graph
{
    public enum PropertyMode
    {
        Allow,
        Default,
        Always,
        Never,
        Invisible
    }

    public class PinDataTypeValidationError
        : Exception
    {
        public PinDataTypeValidationError(string message)
            : base(message)
        {
        }
    }

    public class PropertyValue
    {
        public PropertyValue(string id, IPinDataType type, object value, bool connect)
        {
            var result = type.Validate(value);
            if (!result.IsValid)
                throw new PinDataTypeValidationError(result.Message);

            this.Id = id;
            this.Type = type;
            this.Value = value;
            this.Connect = connect;
        }

        public string Id { get; private set; }
        public IPinDataType Type { get; private set; }
        public object Value { get; private set; }
        public bool Connect { get; private set; }

        public PropertyValue WithValue(object value) =>
            object.Equals(value, this.Value) ? this : new PropertyValue(this.Id, this.Type, value, this.Connect);

        public PropertyValue WithConnect(bool connect) =>
            connect == this.Connect ? this : new PropertyValue(this.Id, this.Type, this.Value, connect);
    }

    public interface IPropertyContainer
        : IContainer<IOutputPin>
    {
        PropertyValue GetProperty(string pinId);
        void SetProperty(PropertyValue value);
        bool IsConnected(IInputPin pin);
        bool Connect(IInputPin pin);
        void Disconnect(IInputPin pin);

        IEnumerable<PropertyValue> GetProperties(bool onlyBound);
        void SetProperties(IEnumerable<PropertyValue> values);
    }
}
