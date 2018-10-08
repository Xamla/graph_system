using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Python.Runtime;

namespace Xamla.Graph.Modules.Python3
{
    public class XamlaPythonException : System.Exception
    {
        private string message;
        private List<string> stackTraceElements = null;
        private string stackTrace = "";

        public XamlaPythonException(PythonException e) : base(e.Message, e)
        {
            IntPtr pyTBPython = e.PyTB;

            using (Py.GIL())
            {
                if (pyTBPython != IntPtr.Zero)
                {
                    PyObject tb_module = PythonEngine.ImportModule("traceback");
                    using (var pyTB = new PyObject(pyTBPython))
                    {
                        var traceRaw = (List<string>)PyConvert.ToClrObject(tb_module.InvokeMethod("format_tb", pyTB),
                                                                                 typeof(List<string>));
                        stackTraceElements = traceRaw.Select(i => Regex.Replace(i, @"\r\n?|\n", "")).ToList();
                    }

                    stackTrace = String.Join(System.Environment.NewLine, stackTraceElements);
                }
            }

            message = String.Join(System.Environment.NewLine, new List<string> { e.Message, "Stack trace: ", stackTrace });
        }

        /// <summary>
        /// Message Property
        /// </summary>
        /// <remarks>
        /// A string representing the complete python exception
        /// consist of the message, the error type name and the
        /// stack trace
        /// </remarks>
        public override string Message
        {
            get { return message; }
        }

        /// <summary>
        /// StackTrace Property
        /// </summary>
        /// <remarks>
        /// A string representing the python stack trace
        /// </remarks>
        public override string StackTrace
        {
            get { return stackTrace; }
        }

        /// <summary>
        /// StackTraceElements Property
        /// </summary>
        /// <remarks>
        /// A list where each item contains
        /// a python stack trace print line
        /// </remarks>
        public List<string> StackTraceElements
        {
            get { return stackTraceElements; }
        }
    }
}