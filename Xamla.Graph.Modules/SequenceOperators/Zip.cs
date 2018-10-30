using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.Zip")]
    public class Zip
        : SubGraphModule
        , IInterfaceModule
    {
        DynamicInputPin dynamicInputPin;
        GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;
        GenericInputPin subGraphResult;
        bool addingSubGraphPin;

        public Zip(IGraphRuntime runtime)
            : base(runtime, true)
        {
            this.dynamicInputPin = new DynamicInputPin(
                runtime,
                inputs,
                "Input",
                PinDataTypeFactory.FromType(typeof(ISequence<>)),
                OnDynamicInputAdd,
                id => SubGraph.InputModule.RemoveModulePin(id)
            );
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<object>)));

            this.subGraphResult = this.SubGraph.OutputModule.DefaultInputPin;
            this.subGraphResult.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.CreateAny(), false, pinDataType =>
                {
                    this.genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(pinDataType.UnderlyingType));

                    this.outputPin.ChangeType(PinDataTypeFactory.FromType(typeof(ISequence<>).MakeGenericType(pinDataType.UnderlyingType)));
                });
            });
        }

        private bool OnDynamicInputAdd(string id)
        {
            dynamicInputPin.Pin(id).WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);

                    var pinEvent = (PinConnectionChangeEvent)evt;

                    // subgraph input
                    var pin = SubGraph.InputModule.Outputs.Where(x => x.Id == pinEvent.Source.Id).OfType<IDataTypeChangeable>().FirstOrDefault();
                    pin.ChangeType(PinDataTypeFactory.FromType(genericType));
                });
            });

            try
            {
                addingSubGraphPin = true;
                return SubGraph.InputModule.AddModulePin(id, true, PinDataTypeFactory.CreateAny()) != null;
            }
            finally
            {
                addingSubGraphPin = false;
            }
        }

        private void DefaultState()
        {
            dynamicInputPin.AdjustPinCount(2);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.DefaultState();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            if (dynamicInputPin.Count == 0)
                DefaultState();
        }

        public override bool InputAddable
        {
            get { return true; }
        }

        public PinDirection InterfaceDirection
        {
            get { return PinDirection.Input; }
        }

        public IPin AddInterfacePin(string id, bool loading = false)
        {
            return dynamicInputPin.AddPin(id, loading);
        }

        protected override string BeforeAddSubGraphInputPin(string id)
        {
            if (addingSubGraphPin)
                return id;

            return dynamicInputPin.AddPin(id).Id;
        }

        public bool RemoveInterfacePin(string id)
        {
            return dynamicInputPin.RemovePin(id);
        }

        protected override bool BeforeRemoveSubGraphInputPin(string id)
        {
            return dynamicInputPin.RemovePin(id);
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        [EvaluateInternal]
        private ISequence<T> EvaluateInternal<T>(ISequence[] sources, Delegate subGraphDelegate)
        {
            var invokeMethod = subGraphDelegate.GetType().GetMethod("Invoke");
            var parameters = invokeMethod.GetParameters();
            var returnType = invokeMethod.ReturnType;

            if (sources.Length != parameters.Length - 1)
                throw new Exception("Delegate parameter list does not match input sequences.");

            return Sequence.ZipN(sources.Select(x => x.AsObjects()).ToArray())
                .SelectAsync<object[], T>((values, cancellationToken) =>
                {
                    object[] args;
                    // add cancellation token as last argument
                    args = new object[parameters.Length];
                    Array.Copy(values, args, values.Length);
                    args[args.Length - 1] = cancellationToken;

                    return (Task<T>)subGraphDelegate.DynamicInvoke(args);
                });
        }

        public override Task<object[]> Evaluate(object[] inputs, Delegate subGraphDelegate, CancellationToken cancel)
        {
            var sources = inputs.OfType<ISequence>().ToArray();

            var result = genericDelegate.Delegate(sources, subGraphDelegate);

            return Task.FromResult(new object[] { result });
        }
    }
}
