using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xamla.Graph;

[assembly: GraphRuntimeInitializer(typeof(Xamla.Graph.Modules.Python3.Initializer))]

namespace Xamla.Graph.Modules.Python3
{
    public class Initializer
         : IGraphRuntimeInitializer
    {
        // TODO: pass these path values from app settings
        static readonly string[] LINUX_PYTHON3_PATHS = new string[] {
            "/usr/lib/python3/dist-packages",
            "/usr/local/lib/python3.5/dist-packages",
            "lib/python",
            "python_lib",
            "python_packages",
            "src",
            "src/python",
            "."
        };

        IGraphRuntime runtime;
        ILogger logger;
        IPythonMainThread mainThread;
        HashSet<string> modulesExcludedFormReload;

        public void Initialize(IGraphRuntime runtime)
        {
            this.runtime = runtime;
            logger = runtime.ServiceLocator.GetService<ILoggerFactory>().CreateLogger<Initializer>();
            mainThread = runtime.ServiceLocator.GetService<IPythonMainThread>();

            try
            {
                mainThread.RunSync(InitializeOnMainThread);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Python module initialization failed: " + e.Message);
            }

            runtime.WhenExecutionControlEvent.Where(x => x == RuntimeEvent.Start).Subscribe(_ => ClearGobalScopesAndReloadModules());
            runtime.WhenGraphLoadedEvent.Subscribe(_ => ClearGobalScopesAndReloadModules());

            runtime.ModuleFactory.RegisterAllModules(Assembly.GetExecutingAssembly());
        }

        private void InitializeOnMainThread()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (!PythonEngine.IsInitialized)
            {
                char pathSplitChar = isWindows ? ';' : ':';

                logger.LogInformation("Initializing Python3");

                if (!isWindows)
                {
                    var pythonPathEntries = PythonEngine.PythonPath.Split(pathSplitChar).ToList();

                    foreach (var path in LINUX_PYTHON3_PATHS)
                    {
                        var libPath = Path.GetFullPath(path);
                        if (Directory.Exists(libPath) && !pythonPathEntries.Contains(libPath))
                        {
                            pythonPathEntries.Add(libPath);
                        }
                    }

                    PythonEngine.PythonPath = string.Join(pathSplitChar, pythonPathEntries);
                }
                else
                {
                    // on window for development use the defult PythonPath
                    // Note: In order to test local python-libs you can manually add the prj/lib/python path
                    // to the PYTHON_PATH environment variable..
                }

                logger.LogInformation("Python3.ProgramName: " + PythonEngine.ProgramName);
                logger.LogInformation("Python3.Version: " + PythonEngine.Version);
                logger.LogInformation("Python3.Platform: " + PythonEngine.Platform);
                logger.LogInformation("Python3.BuildInfo: " + PythonEngine.BuildInfo);
                logger.LogInformation("Python3.PythonHome: " + PythonEngine.PythonHome);
                logger.LogInformation("Python3.PythonPath: " + PythonEngine.PythonPath);

                PythonEngine.Initialize();
                try
                {
                    NumpyHelper.TryInitialize();
                    RoboticsTypesExtensions.Initialize();
                }
                catch (PythonException e)
                {
                    logger.LogError(e, "Python special type converter initialization failed: " + e.Message);
                }
                modulesExcludedFormReload = GetLoadedModuleNames().Concat(GetBuiltInModuleNames()).ToHashSet();
            }
        }

        [PyGIL]
        private IList<string> GetLoadedModuleNames()
        {
            var sys = PythonEngine.ImportModule("sys");
            var modules = new PyDict(sys.GetAttr("modules"));
            return modules.Keys().OfType<PyObject>().Select(x => x.As<string>()).ToList();
        }

        [PyGIL]
        public IList<string> GetBuiltInModuleNames()
        {
            var sys = PythonEngine.ImportModule("sys");
            return sys.GetAttr("builtin_module_names").OfType<PyObject>().Select(x => x.As<string>()).ToList();
        }

        private void ClearGobalScopesAndReloadModules()
        {
            if (!PythonEngine.IsInitialized)
                return;

            mainThread.RunSync(() =>
            {
                using (Py.GIL())
                {
                    PyScopeManager.Global.Clear();
                    ReloadModules();
                }
            });
        }

        [PyGIL]
        private void ReloadModules()
        {
            int count = 0;
            var sw = new Stopwatch();

            sw.Start();

            var sys = PythonEngine.ImportModule("sys");
            var importLib = PythonEngine.ImportModule("importlib");

            var modules = new PyDict(sys.GetAttr("modules"));
            foreach (var key in modules.Keys().OfType<PyObject>())
            {
                try
                {
                    var name = key.As<string>();
                    if (modulesExcludedFormReload.Contains(name))
                        continue;

                    var module = modules.GetItem(key);
                    if (!name.Contains("importlib"))
                    {
                        if (module.HasAttr("__file__") && module.HasAttr("__name__"))
                        {
                            var path = module.GetAttr("__file__").As<string>();
                            importLib.InvokeMethod("reload", module);
                            count += 1;
                        }
                    }
                }
                catch (PythonException)
                {
                }
            }

            sw.Stop();
            logger.LogInformation($"[PythonModules] Reloading {count} Python modules took {sw.ElapsedMilliseconds}ms.");
        }
    }
}
