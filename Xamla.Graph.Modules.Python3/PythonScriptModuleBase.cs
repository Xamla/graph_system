using Microsoft.Extensions.Logging;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.Python3
{
    public abstract class PythonScriptModuleBase
        : ModuleBase
    {
        const int CAPTION_PIN_INDEX = 1;
        const int FUNCTION_NAME_PIN_INDEX = 2;

        public const string DEFAULT_FUNCTION_NAME = "main";
        public const string DEFAULT_FILE_NAME = "";

        protected ILogger logger;
        protected IPythonMainThread mainThread;
        readonly GenericInputPin captionPin;
        readonly GenericInputPin functionNamePin;
        readonly GenericInputPin useMainThreadPin;
        readonly GenericInputPin parserErrorPin;
        List<GenericInputPin> argumentPins = new List<GenericInputPin>();       // script inputs
        List<GenericOutputPin> resultPins = new List<GenericOutputPin>();       // script outputs
        PyObject script;

        IDisposable graphLoadedSubscription;

        protected string filename = DEFAULT_FILE_NAME;      // used for error messages and __file__
        protected RunFlagType mode = RunFlagType.File;
        protected PySignature signature;
        protected Exception parserError;

        public PythonScriptModuleBase(IGraphRuntime runtime, ILoggerFactory loggerFactory, IPythonMainThread mainThread)
           : base(runtime, ModuleKind.ScriptModule)
        {
            logger = loggerFactory.CreateLogger<PythonScriptModuleBase>();
            this.mainThread = mainThread;

            this.captionPin = this.AddInputPin("caption", PinDataTypeFactory.Create<string>(), PropertyMode.Always);
            this.functionNamePin = this.AddInputPin("functionName", PinDataTypeFactory.Create<string>(DEFAULT_FUNCTION_NAME), PropertyMode.Default);
            this.useMainThreadPin = this.AddInputPin("useMainThread", PinDataTypeFactory.Create<bool>(true), PropertyMode.Always);
            this.parserErrorPin = this.AddInputPin("parserError", PinDataTypeFactory.Create<Models.ExceptionModel>(null, WellKnownEditors.ExceptionViewer), PropertyMode.Always);
            var scriptSourcePinId = this.AddScriptSourcePin();

            this.argumentPins = new List<GenericInputPin>();
            this.resultPins = new List<GenericOutputPin>();

            this.properties[scriptSourcePinId].WhenNodeEvent.OfType<PropertyChangedEvent>().Subscribe(evt =>
            {
                logger.LogInformation("Script source changed");
                UpdateSignatureSafe();
            });

            this.properties[functionNamePin.Id].WhenNodeEvent.OfType<PropertyChangedEvent>().Subscribe(evt =>
            {
                logger.LogInformation("Function name changed");
                UpdateSignatureSafe();
            });

            this.DynamicDisplayName = new DynamicDisplayNameFunc(FormatDisplayName);
        }

        protected virtual string FormatDisplayName()
        {
            string caption = this.Properties.GetString(captionPin.Id);
            if (!string.IsNullOrEmpty(caption))
                return caption;

            if (this.Graph == null)
                return null;

            string fn = string.IsNullOrEmpty(filename) ? "Python3Script": this.Graph.MakeRelative(filename);
            var functionNameProperty = this.Properties.GetProperty(functionNamePin.Id);
            string functionName = functionNameProperty.Connect ? (string)functionNameProperty.Value : "?";
            return $"{functionName} ({fn})";
        }

        protected void UpdateSignatureSafe()
        {
            try
            {
                UpdateSignature();
            }
            catch (Exception e)
            {
                this.SetParserError(e);
            }
        }

        public string Caption
        {
            get { return properties.Get<string>(captionPin.Id); }
            set { properties.Set(captionPin.Id, value, true); }
        }

        public string FunctionName
        {
            get { return properties.Get<string>(functionNamePin.Id) ?? DEFAULT_FUNCTION_NAME; }
            set { properties.Set(functionNamePin.Id, value, true); }
        }

        protected PyObject Script
        {
            set
            {
                using (Py.GIL())
                {
                    if (script != null && this.script != value)
                        script.Dispose();
                    script = value;
                }
            }
            get
            {
                return script;
            }
        }

        protected void SetParserError(Exception e)
        {
            parserError = e;
            this.Properties.Set(parserErrorPin.Id, parserErrorPin.DataType, Models.ExceptionModel.FromException(e));
        }

        protected virtual void UpdateSignature()
        {
            if (this.Graph == null)     // file path resolving only possible when module is attached to graph
                return;

            logger.LogInformation("UpdateSignature");
            lock (gate)
            {
                PySignature signature = null;

                // when there is no script available
                var src = this.GetScript();
                if (string.IsNullOrEmpty(src))
                {
                    // there are no arguments
                    signature = new PySignature();
                    this.Script = null;
                    SetParserError(null);
                }
                else
                {
                    mainThread.RunSync(() =>
                    {
                        using (PyScope ps = Py.CreateScope())
                        {
                            PyObject script = PythonEngine.Compile(src, filename ?? "", mode);
                            ps.Execute(script);
                            signature = PySignature.GetSignature(ps, this.FunctionName);
                            this.Script = script;
                        }
                    });

                    this.SetParserError(null);
                }

                try
                {
                    this.signature = signature;

                    // remove all argument pins that no longer exist
                    var unusedPins = argumentPins.Where(pin => !signature.HasArgument(pin.Id)).ToArray();
                    foreach (var unusedPin in unusedPins)
                    {
                        unusedPin.DisconnectAll();
                        argumentPins.Remove(unusedPin);
                        logger.LogInformation($"Removing input pin: {unusedPin.Id}");
                        inputs.Remove(unusedPin);
                    }

                    foreach (var arg in signature.Arguments)
                    {
                        var existingPin = argumentPins.FirstOrDefault(x => x.Id == arg.Name);
                        if (existingPin == null)
                        {
                            var newPin = this.AddInputPin(arg.Name, PinDataTypeFactory.FromType(arg.Type), PropertyMode.Allow);
                            argumentPins.Add(newPin);
                        }
                        else
                        {
                            // check type
                            if (existingPin.DataType.UnderlyingType != arg.Type)
                                existingPin.ChangeType(PinDataTypeFactory.FromType(arg.Type));
                        }
                    }

                    // get all pin ids which are not member of this dynamic pin collection
                    var nonDynamicPinIds = this.inputs.Where(x => !argumentPins.Contains(x)).Select(x => x.ObjectId);

                    // reorder module input pins
                    var orderedArgumentPinObjectIds = signature.Arguments.Select(x => argumentPins.First(y => y.Id == x.Name).ObjectId);
                    var newOrder = nonDynamicPinIds.Concat(orderedArgumentPinObjectIds).ToArray();
                    this.inputs.Reorder(newOrder);

                    // analyse return type of script
                    Type[] returnTypes;
                    if (signature.ReturnType == null)
                    {
                        returnTypes = new Type[0];
                    }
                    else if (TypeHelpers.IsTuple(signature.ReturnType))
                    {
                        returnTypes = TypeHelpers.GetTupleTypes(signature.ReturnType);
                    }
                    else
                    {
                        returnTypes = new Type[] { signature.ReturnType };
                    }

                    // remove pins that are not needed anymore
                    while (resultPins.Count > returnTypes.Length)
                    {
                        var last = resultPins[resultPins.Count - 1];
                        logger.LogInformation($"Removing output pin: {last.Id}");
                        last.DisconnectAll();
                        outputs.Remove(last);
                        resultPins.RemoveAt(resultPins.Count - 1);
                    }

                    for (int i = 0; i < returnTypes.Length; i++)
                    {
                        if (i < resultPins.Count)
                        {
                            var existingPin = resultPins[i];
                            if (existingPin.DataType.UnderlyingType != returnTypes[i])
                                existingPin.ChangeType(PinDataTypeFactory.FromType(returnTypes[i]));
                        }
                        else
                        {
                            var newPin = this.AddOutputPin($"output{i}", PinDataTypeFactory.FromType(returnTypes[i]));
                            resultPins.Add(newPin);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

        protected abstract string AddScriptSourcePin();

        protected abstract string GetScript(string source = null);

        protected override async Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            if (parserError != null)
                throw parserError;

            var script = this.Script;
            if (script == null)
            {
                return new object[0];       // no script available
            }

            object[] returnValue = null;

            void evalAction()
            {
                // create new python scope
                using (PyScope ps = Py.CreateScope())
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        ps.Set("__file__", filename);
                    }

                    // load script into scope
                    ps.Execute(script);

                    ps.Set("context", this.Runtime.ScriptContext.ToPython());
                    ps.Set("store", this.Graph.ValueStore.ToPython());

                    // process module inputs and convert them to python objects
                    int firstArgIndex = inputs.Length - argumentPins.Count;
                    var args = CreateArguments(inputs, firstArgIndex);

                    // call python method
                    string functionName = (string)inputs[FUNCTION_NAME_PIN_INDEX] ?? DEFAULT_FUNCTION_NAME;
                    PyObject fn = ps.Get(functionName);

                    PyObject pyResult = null;
                    try
                    {
                        pyResult = fn.Invoke(args);
                    }
                    catch (PythonException e)
                    {
                        throw new XamlaPythonException(e);
                    }

                    // convert result back to clr object
                    var result = PyConvert.ToClrObject(pyResult, signature.ReturnType);

                    if (result == null)
                        returnValue = new object[] { null };
                    else if (!result.GetType().IsTuple())
                        returnValue = new object[] { result };
                    else
                        returnValue = TypeHelpers.GetTupleValues(result);
                }
            }

            int useMainThreadPinIndex = 2;
            if (this.flowMode != FlowMode.NoFlow)
                useMainThreadPinIndex += 1;
            bool useMainThread = (bool)inputs[useMainThreadPinIndex];

            if (useMainThread)
            {
                await mainThread.Enqueue(evalAction, cancel);
            }
            else
            {
                using (Py.GIL())
                {
                    evalAction();
                }
            }

            if (this.flowMode != FlowMode.NoFlow)
            {
                return (new object[] { Flow.Default }.Concat(returnValue)).ToArray();
            }
            else
            {
                return returnValue;
            }
        }

        private PyObject[] CreateArguments(object[] inputs, int firstArgIndex)
        {
            var args = new PyObject[argumentPins.Count];

            if (firstArgIndex + args.Length != inputs.Length)
                throw new Exception("Invalid argument count");

            for (int i = 0; i < args.Length; i++)
            {
                var x = inputs[firstArgIndex + i];

                args[i] = PyConvert.ToPyObject(x);
            }

            return args;
        }


        protected override void OnCreate()
        {
            base.OnStart();
            UpdateSignatureSafe();
        }

        protected override void OnStart()
        {
            base.OnStart();
            UpdateSignatureSafe();
        }

        void OnGraphLoaded()
        {
            UpdateSignatureSafe();
        }

        protected override void OnRegistered()
        {
            graphLoadedSubscription = this.Runtime.WhenGraphLoadedEvent.Subscribe(x => OnGraphLoaded());
            UpdateSignatureSafe();
            base.OnRegistered();
        }

        protected override void OnUnregistered()
        {
            graphLoadedSubscription?.Dispose();
            graphLoadedSubscription = null;

            base.OnUnregistered();
        }
    }
}
