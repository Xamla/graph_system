using System;
using System.Collections.Generic;
using System.Linq;
using Xamla.Types.Converters;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph
{
    public enum PinDirection
    {
        Input,
        Output
    }

    public class PinConnection
    {
        public PinConnection(IPin origin, IPin remote, ITypeConverter adapter, Type dataType)
        {
            this.Origin = origin;
            this.Remote = remote;
            this.Adapter = adapter;
            this.DataType = dataType;
        }

        public IPin Origin { get; private set; }
        public IPin Remote { get; private set; }
        public ITypeConverter Adapter { get; private set; }
        public Type DataType { get; private set; }

        public IOutputPin Source
        {
            get { return this.Origin as IOutputPin ?? this.Remote as IOutputPin; }
        }

        public IInputPin Destination
        {
            get { return this.Origin as IInputPin ?? this.Remote as IInputPin; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            var other = obj as PinConnection;
            if (other == null)
                return false;

            return Source == other.Source
                && Destination == other.Destination;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + HashHelper.GetHashCode(Source, Destination);
        }
    }

    public enum PinConnectionChangeAction
    {
        Connect,
        Disconnect
    }

    public class PinConnectionChangeEvent
        : NodeEvent
    {
        public PinConnectionChangeEvent(INode source, IPin connectionTo, PinConnectionChangeAction change)
            : base(source, true)
        {
            this.ConnectionTo = connectionTo;
            this.Change = change;
        }

        public PinConnectionChangeAction Change { get; private set; }

        public IPin ConnectionTo { get; private set; }
    }

    [Flags]
    public enum PinFlags
    {
        None = 0,
        ResolvePath = 0x1,
    }

    public interface IPin
        : INode
    {
        PinDirection Direction { get; }
        int? MaxConnections { get; }
        IModule Module { get; }
        IPinDataType DataType { get; }
        bool IsFlow { get; }
        void Connect(IPin pin, ITypeConverter adapter = null);
        bool Disconnect(IPin pin);
        bool Disconnect(PinConnection connection);
        IList<PinConnection> Connections { get; }
        int ConnectionCount { get; }
        bool Removable { get; }
        bool Renameable { get; }
        int Index { get; }
        string Description { get; }
        PinFlags Flags { get; }
    }

    public interface IInputPin
        : IPin
    {
        PropertyMode PropertyConnectionMode { get; }

        /// <summary>
        /// Determines whether the pin is currently connected to the module's property container.
        /// </summary>
        bool IsConnectedToPropertyContainer { get; }
    }

    public interface IOutputPin
        : IPin
    {
        /// <summary>
        /// Indicates that the result generated at this OutputPin will trigger a breakpoint in the graph-runtime to inspect the value.
        /// </summary>
        bool InspectResult { get; set; }
    }

    public interface IDataTypeChangeable
    {
        IEnumerable<Exception> ChangeType(IPinDataType dataType);
        void SetType(IPinDataType type);
    }

    public static class PinExtensions
    {
        public static void DisconnectAll(this IPin pin)
        {
            foreach (var c in pin.Connections)
                pin.Disconnect(c);
        }

        public static bool IsConnectedToPropertyContainer(this IPin pin)
        {
            if (pin.Direction != PinDirection.Input || pin.Module == null || pin.Connections.Count != 1)
                return false;

            var propertyContainer = pin.Module.Properties;
            return pin.Connections.Any(x => x.Source.Container == propertyContainer);
        }
    }
}
