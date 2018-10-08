using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Collections.Generic;
using System;
using Xamla.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Graph.Modules
{
    public static class RunProcessModule
    {
        static ILogger logger;

        internal static void Init(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("RunProcess");
        }

        /// <summary>
        /// Starts an executable and waits until it terminates.
        /// </summary>
        /// <param name="executablePath">Path of executable to start. Specifying the full path can be omitted if the executable file is in the system search path.</param>
        /// <param name="arguments">Command line arguments for the application.</param>
        /// <param name="workingDirectory">Working directory used when starting. Leave empty to use the current working directory of the running process.</param>
        /// <param name="mayThrow">Flag whether an exception should be thrown in case of an error (e.g. exit code != 0).</param>
        /// <param name="cancel"> cancallation token to stop async running process </param>
        /// <returns>
        /// <return name="stdout">Standard output (STDOUT)</return>
        /// <return name="stderr">Error output (STDERR)</return>
        /// <return name="exitCode">Exit code of the process</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.OS.RunProcess", Flow = true)]
        public static async Task<Tuple<string, string, int>> RunProcess(
           [InputPin(PropertyMode = PropertyMode.Default, ResolvePath = false)] string executablePath = "",
           [InputPin(PropertyMode = PropertyMode.Default)] string arguments = "",
           [InputPin(PropertyMode = PropertyMode.Default)] string workingDirectory = "",
           [InputPin(PropertyMode = PropertyMode.Default)] bool mayThrow = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            ProcessOutput processOutput = await Launcher.RunAsync(executablePath, arguments, workingDirectory, mayThrow, logger, LogLevel.Information, cancel);
            return Tuple.Create(processOutput.StandardOutput, processOutput.StandardError, processOutput.ExitCode);
        }
    }
}
