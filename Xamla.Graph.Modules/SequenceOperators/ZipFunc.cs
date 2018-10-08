using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types.Sequence;

namespace Xamla.Graph.Modules.SequenceOperators
{
    [Module(ModuleType = "Xamla.Sequence.Operators.ZipFunc")]
    public class ZipFunc
        : ModuleBase
        , IInterfaceModule
    {
        private GenericInputPin funcPin;
        private DynamicInputPin dynamicInputPin;
        private GenericOutputPin outputPin;

        public ZipFunc(IGraphRuntime runtime)
            : base(runtime)
        {
            this.funcPin = AddInputPin("Function", PinDataTypeFactory.Create<Delegate>(), PropertyMode.Allow);
            this.dynamicInputPin = new DynamicInputPin(runtime, inputs, "Input", PinDataTypeFactory.Create<ISequence>());
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.Create<ISequence<object>>());
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

        public bool RemoveInterfacePin(string id)
        {
            return dynamicInputPin.RemovePin(id);
        }

        public IInputPin FuncPin
        {
            get { return funcPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        public bool ReorderPins(IList<string> objectIds)
        {
            if (objectIds[0] != funcPin.ObjectId)
                return false;

            return this.inputs.Reorder(objectIds) != null;
        }

        private ISequence<object> Evaluate(Delegate func, params ISequence[] sources)
        {
            var invokeMethod = func.GetType().GetMethod("Invoke");
            var parameters = invokeMethod.GetParameters();
            var returnType = invokeMethod.ReturnType;

            if (returnType != typeof(Task<object>))
                throw new Exception("Task<object> is the only supported return type for a Zip function.");

            bool hasCancel = parameters.Length > 0 && parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);
            int argCount = hasCancel ? parameters.Length - 1 : parameters.Length;

            if (sources.Length != argCount)
                throw new Exception("Delegate parameter list does not match input sequences.");

            return Sequence.ZipN(sources.Select(x => x.AsObjects()).ToArray())
                .SelectAsync<object[], object>((values, cancellationToken) =>
                {
                    object[] args;

                    if (hasCancel)
                    {
                        // add cancellation token as last argument
                        args = new object[parameters.Length];
                        Array.Copy(values, args, values.Length);
                        args[args.Length - 1] = cancellationToken;
                    }
                    else
                    {
                        args = values;
                    }

                    return (Task<object>)func.DynamicInvoke(args);
                });
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var func = (Delegate)inputs[0];
            var sources = inputs.Skip(1).OfType<ISequence>().Select(x => x.AsObjects()).ToArray();

            var result = Evaluate(func, sources);

            return Task.FromResult(new object[] { result });
        }
    }
}
