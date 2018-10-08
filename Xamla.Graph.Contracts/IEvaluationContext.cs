using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph
{
    public interface IEvaluationContext
    {
        CancellationToken Cancel { get; }

        void AddFlow(IModule start, IModule end);

        Task<object>[] AddOutputModule(IInterfaceModule interfaceModule, bool inspectOutputs);

        void SetOutputPinValue(IOutputPin pin, Task<object> value, bool additionalInput);

        Task<object> ReadOutputAsync(IOutputPin outputPin);

        Task Evaluate(Action<Task> onEnd = null);
    }
}
