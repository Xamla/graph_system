using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;

namespace Xamla.Graph.Modules.Python3
{
    [Module(ModuleType = "Python3.Python3ScriptFile", Flow = true)]
    [ModuleTypeAlias("Py3ScriptFile", IncludeInCatalog = true)]
    public class Python3ScriptFileModule
        : PythonScriptModuleBase
    {
        FileSystemWatcher watcher;
        GenericInputPin pathPin;

        public Python3ScriptFileModule(IGraphRuntime runtime, ILoggerFactory loggerFactory, IPythonMainThread mainThread)
            : base(runtime, loggerFactory, mainThread)
        {
        }

        protected override string AddScriptSourcePin()
        {
            pathPin = AddInputPin("path", PinDataTypeFactory.Create<string>(null, WellKnownEditors.SingleLineText), PropertyMode.Always, false, false, PinFlags.ResolvePath);
            return pathPin.Id;
        }

        protected override void OnRegistered()
        {
            if (watcher == null)
            {
                watcher = new FileSystemWatcher()
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
                };

                watcher.Changed += new FileSystemEventHandler((source, evt) =>
                {
                    logger.LogInformation($"Script file change detected: '{watcher.Filter}'");

                    try
                    {
                        // reorganize module interface pins
                        this.UpdateSignature();
                    }
                    catch (Exception e)
                    {
                        logger.LogInformation(e, $"Error while updating signature: {e.Message}");
                    }
                });

                watcher.Deleted += new FileSystemEventHandler((source, evt) =>
                {
                    // empty filename, which leads to removing of all inputs
                    this.Path = string.Empty;
                });

                watcher.Renamed += new RenamedEventHandler((source, evt) =>
                {
                    // file rename detected, update property
                    this.Path = this.Graph != null
                            ? this.Graph.MakeRelative(evt.FullPath)
                            : evt.FullPath;
                });
            }

            base.OnRegistered();
        }

        private void UpdateWatcher()
        {
            if (watcher == null)        // watcher is only available while node is registered in graph
                return;

            if (string.IsNullOrWhiteSpace(this.Path))
            {
                // disable watcher
                watcher.EnableRaisingEvents = false;
                return;
            }

            try
            {
                var path = ResolvePath(this.Path);
                this.filename = path;
                if (!File.Exists(path))
                {
                    // disable watcher, keep old pins and throw exception
                    var error = new FileNotFoundException("File not found", path);
                    this.parserError = error;
                    throw error;
                }

                // set directory path
                var directoryPath = System.IO.Path.GetDirectoryName(path);
                var fileNameFilter = System.IO.Path.GetFileName(path);
                if (watcher.EnableRaisingEvents == false || directoryPath != watcher.Path || fileNameFilter != watcher.Filter)
                {
                    watcher.Path = directoryPath;
                    watcher.Filter = fileNameFilter;      // filter on the single file name
                    watcher.EnableRaisingEvents = true;   // start watching file
                }
            }
            catch
            {
                watcher.EnableRaisingEvents = false;
                throw;
            }
        }

        protected override void UpdateSignature()
        {
            try
            {
                UpdateWatcher();
            }
            catch (IOException)
            {
            }

            base.UpdateSignature();
        }

        protected override void OnUnregistered()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
            }

            base.OnUnregistered();
        }

        public string Path
        {
            get { return this.properties.Get<string>(pathPin.Id); }
            set { this.Properties.Set(pathPin.Id, value); }
        }

        private string ResolvePath(string path)
        {
            if (this.Graph == null)
                throw new Exception("Path resolution unavailable in detached module.");

            return this.Graph.ResolvePath(path);
        }

        protected override string GetScript(string source = null)
        {
            var path = source ?? this.Path;
            if (string.IsNullOrEmpty(path))
            {
                logger.LogInformation("Path to script file is empty.");
                return string.Empty;
            }

            path = ResolvePath(path);
            logger.LogInformation($"Path to script file is '{path}'.");

            // read the file with simple retry logic:
            // The file is sometimes still exclusively locked when a
            // file system watcher change event is processed.
            for (int i = 0; ; i += 1)
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch (IOException)
                {
                    if (i > 4)
                        throw;
                    else
                        Thread.Sleep(10);
                }
            }
        }
    }
}
