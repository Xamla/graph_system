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
    public enum SortDirection
    {
        Ascending = 0,
        Descending
    }

    [Module(ModuleType = "Xamla.Sequence.Operators.Sort")]   
    public class Sort
        : ModuleBase
    {
        private GenericInputPin inputPin;
        private GenericInputPin sortDirectionPin;
        private GenericOutputPin outputPin;

        GenericDelegate<Func<object, object, object>> genericDelegate;

        public Sort(IGraphRuntime runtime)
            : base(runtime)
        {
            this.inputPin = AddInputPin("Input", PinDataTypeFactory.FromType(typeof(ISequence<>)), PropertyMode.Never);
            this.sortDirectionPin = AddInputPin("SortDirection", PinDataTypeFactory.CreateEnum<SortDirection>(SortDirection.Ascending, WellKnownEditors.DropDown), PropertyMode.Default);
            this.outputPin = AddOutputPin("Output", PinDataTypeFactory.FromType(typeof(ISequence<>)));

            this.inputPin.WhenNodeEvent.Subscribe(evt =>
            {
                PinConnectionChangeEventHandler.ConnectionSensitivePinDataType(evt, PinDataTypeFactory.FromType(typeof(ISequence<>)), true, pinDataType =>
                {
                    var genericType = pinDataType.UnderlyingType.GenericTypeArguments.FirstOrDefault();

                    if (genericType != null)
                        genericDelegate = new GenericDelegate<Func<object, object, object>>(this, EvaluateInternalAttribute.GetMethod(GetType()).MakeGenericMethod(genericType));
                    else
                        genericDelegate = null;

                    outputPin.ChangeType(pinDataType);
                });
            });
        }

        public IInputPin InputPin
        {
            get { return inputPin; }
        }

        public IInputPin SortDirectionPin
        {
            get { return sortDirectionPin; }
        }

        public IOutputPin OutputPin
        {
            get { return outputPin; }
        }

        [EvaluateInternal]
        private ISequence<T> EvaluateInternal<T>(ISequence<T> input, SortDirection sortDirection)
        {
            return input.Buffer()
                .SelectMany(xs =>
                {
                    var ys = (sortDirection == SortDirection.Ascending) ? xs.OrderBy(x => x) : xs.OrderByDescending(x => x);
                    return ys.ToSequence();
                });
        }

        protected override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (genericDelegate == null || genericDelegate.Delegate == null)
                throw new Exception("Evaluation failed due to an type error in the sequence evaluation.");

            var input = inputs[0];
            var sortDirection = inputs[1];

            var result = genericDelegate.Delegate(input, sortDirection);

            return Task.FromResult(new object[] { result });
        }
    }
}
