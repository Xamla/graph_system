using Microsoft.Extensions.Logging;
using System;

namespace Xamla.Graph.Modules.Python3
{
    [Module(ModuleType = "Python3.Python3Script", Flow = true)]
    [ModuleTypeAlias("Py3Script", IncludeInCatalog = true)]
    public class Python3ScriptModule
        : PythonScriptModuleBase
    {
        public static readonly string DEFAULT_SCRIPT =
@"from typing import Dict, List, Tuple
import numpy as np

# To use the type-conversion features of the Xamla graph engine add
# python type hints on arguments and return value.
# The pins of the module will reflect the change direlyt after saving the script.
# See https://www.python.org/dev/peps/pep-0484/

# Examle for numpy array parameter + typed dictionary return value:
#def main(img: np.ndarray) -> Dict[str, List[int]]:
#    return {{ 'str': [1, 2, 3] }}

def main(x: int) -> int:
    return x * 2";

        GenericInputPin scriptPin;

        public Python3ScriptModule(IGraphRuntime runtime, ILoggerFactory loggerFactory, IPythonMainThread mainThread)
            : base(runtime, loggerFactory, mainThread)
        {
        }

        protected override string AddScriptSourcePin()
        {
            scriptPin = AddInputPin("Script", PinDataTypeFactory.Create<string>(DEFAULT_SCRIPT, WellKnownEditors.Python), PropertyMode.Always);
            return scriptPin.Id;
        }

        public string ScriptSource
        {
            get { return this.properties.Get<string>(scriptPin.Id); }
            set { this.Properties.Set(scriptPin.Id, value); }
        }

        protected override string GetScript(string source = null)
        {
            return source ?? this.ScriptSource;
        }
    }
}
