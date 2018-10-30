using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules.XData
{
    [Module(ModuleType = "Xamla.PropertyAccessor")]
    [ModuleTypeAlias("Xamla.PropertiesAccessor")]
    public class PropertyAccessorModule
        : ModuleBase
    {
        private static readonly Type OBJECT_TYPE = typeof(object);
        private static readonly string OBJECT_TYPE_NAME = OBJECT_TYPE.AssemblyQualifiedName;
        private const BindingFlags BINDING_FLAGS = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;

        GenericInputPin itemPin;
        GenericInputPin itemTypeNamePin;
        List<GenericOutputPin> itemPropertyPins;
        Type itemType;

        public PropertyAccessorModule(IGraphRuntime runtime)
            : base(runtime)
        {
            this.itemPin = AddInputPin("Item", PinDataTypeFactory.Create<object>(), PropertyMode.Never);
            this.itemTypeNamePin = AddInputPin("ItemType", PinDataTypeFactory.Create<string>(OBJECT_TYPE_NAME), PropertyMode.Invisible);

            this.itemPropertyPins = new List<GenericOutputPin>();

            this.itemPin.WhenNodeEvent.Subscribe(evt =>
            {
                if (itemPin.Connections.Any())
                {
                    BuildOutputPins(itemPin.Connections.Single().Remote.DataType.UnderlyingType);
                }
                else
                {
                    BuildOutputPins(OBJECT_TYPE);
                }
            });
        }

        protected override void OnCreate()
        {
            base.OnStart();
            BuildOutputPins(OBJECT_TYPE);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            BuildOutputPins(Type.GetType(this.ItemTypeName) ?? OBJECT_TYPE);
        }

        void RestorePinConnection(IPin pin, Dictionary<string, IPin[]> oldConnections)
        {
            IPin[] remotePins;
            if (oldConnections.TryGetValue(pin.Id, out remotePins))
            {
                foreach (var remotePin in remotePins)
                {
                    if (remotePin.Graph == null || remotePin.Container == null)
                        return;

                    try
                    {
                        pin.Connect(remotePin);
                    }
                    catch (Exception)
                    {
                        // ignoring exception during connection restore
                    }
                }
            }
        }

        public void BuildOutputPins(Type itemType)
        {
            if (itemType == this.itemType)
                return;

            // do not fall back to object when we have build pins for another type before
            // (most likely the drop back to object is due to a disconnect and we do not want to loose all existing connections)
            if (itemType == typeof(object) && itemPropertyPins.Count > 0)
                return;

            var oldConnections = new Dictionary<string, IPin[]>();

            // remove all outputs
            foreach (var itemPropertiesPin in itemPropertyPins)
            {
                oldConnections.Add(itemPropertiesPin.Id, itemPropertiesPin.Connections.Select(x => x.Remote).ToArray());
                itemPropertiesPin.DisconnectAll();
                outputs.Remove(itemPropertiesPin);
            }
            itemPropertyPins.Clear();

            var itemProperties = itemType.GetProperties(BINDING_FLAGS).Where(p => p.GetIndexParameters().Length == 0);
            if (itemProperties.Any())
            {
                // add one output pin for every property
                foreach (var property in itemProperties)
                {
                    if (this.itemPropertyPins.Any(p => p.Id == property.Name))
                        continue;

                    var pin = AddOutputPin(property.Name, PinDataTypeFactory.FromType(property.PropertyType));
                    this.itemPropertyPins.Add(pin);

                    // try to reestablish exsting connections
                    RestorePinConnection(pin, oldConnections);
                }
            }
            else
            {
                // add a single pin for items without properties
                this.itemPropertyPins.Add(AddOutputPin(itemType.Name, PinDataTypeFactory.FromType(itemType)));
            }

            // store current type in property
            this.ItemTypeName = itemType.AssemblyQualifiedName;
            this.itemType = itemType;
        }

        public IInputPin ItemPin
        {
            get { return itemPin; }
        }

        public string ItemTypeName
        {
            get { return this.properties.Get<string>(itemTypeNamePin.Id); }
            set { this.Properties.Set(itemTypeNamePin.Id, value); }
        }

        public IEnumerable<IOutputPin> ItemPropertyPins
        {
            get { return itemPropertyPins; }
        }

        private IEnumerable<object> FetchProperties(object item)
        {
            var itemType = Type.GetType(this.ItemTypeName);

            if (itemType == null)
                throw new TypeLoadException("Type not found.");

            if (itemType == OBJECT_TYPE)
            {
                yield return item;
            }
            else
            {
                foreach (var output in this.outputs)
                {
                    var property = itemType.GetProperties(BINDING_FLAGS).Where(p => p.Name == output.Id).FirstOrDefault();
                    if (property == null)
                        throw new Exception("Invalid type property.");

                    yield return property.GetMethod.Invoke(item, new object[] { });
                }
            }
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var item = inputs[0];

            var itemProperties = FetchProperties(item).ToArray();

            return Task.FromResult(itemProperties);
        }
    }
}
