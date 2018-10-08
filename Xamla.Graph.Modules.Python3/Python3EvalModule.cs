using Python.Runtime;
using System;
using System.Linq;
using Xamla.Graph.MethodModule;

namespace Xamla.Graph.Modules.Python3
{
    [Module(ModuleType = "Python3.Python3Eval", Flow = false, Description = "This module evaluates a Python expression providing its inputs as local variables.")]
    [ModuleTypeAlias("Py3Eval", IncludeInCatalog = true)]
    public class Python3EvalModule
        : SingleInstanceMethodModule
    {
        IPythonMainThread mainThread;

        public Python3EvalModule(IGraphRuntime runtime, IPythonMainThread mainThread)
            : base(runtime, ModuleKind.ScriptModule)
        {
            this.mainThread = mainThread;
            this.DynamicDisplayName = new DynamicDisplayNameFunc(FormatDisplayName);
        }

        private string FormatDisplayName()
        {
            string caption = this.Properties.GetString("caption");
            if (!string.IsNullOrEmpty(caption))
                return caption;
            return null;
        }

        [ModuleMethod]
        [OutputPin(Name = "result", Description = "The result of the expression evaluation")]
        public object Run(
            [InputPin(Name = "caption", Description = "The caption of this module", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.SingleLineText)] string caption,
            [InputPin(Name = "expression", Description = "The Python expression", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.SingleLineText)] string expression,
            [InputPin(Name = "useMainThread", Description = "Whether to run the code on the Python main thread", PropertyMode = PropertyMode.Always, Editor = WellKnownEditors.CheckBox, DefaultValue = "false")] bool useMainThread,
            [InputPin(Name = "input", Description = "These are the inputs for the expression", PropertyMode = PropertyMode.Never)] params object[] inputs
        )
        {
            object evalBlock()
            {
                using (PyScope ps = Py.CreateScope())
                {
                    int i = 0;
                    foreach (var inputPin in this.dynamicInputPin.Pins)
                    {
                        if (inputPin.Connections.Any())
                            ps.Set(inputPin.Id, PyConvert.ToPyObject(inputs[i]));
                        i++;
                    }

                    ps.Set(dynamicInputPin.Alias + "s", inputs);
                    PyObject evalResult = ps.Eval(expression);
                    return PyConvert.ToClrObject(evalResult, typeof(object));
                }
            }

            if (useMainThread)
            {
                return mainThread.EvalSync(evalBlock);
            }
            else
            {
                using (Py.GIL())
                {
                    return evalBlock();
                }
            }
        }
    }
}
