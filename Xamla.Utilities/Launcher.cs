using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public class ProcessOutput
    {
        public string StandardOutput { get; set; }      // all output printed to stdout
        public string StandardError { get; set; }       // all output printed to stderr
        public int ExitCode { get; set; }
        public Exception Error { get; set; }
        public bool Suceeded { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Utility class which runs a command and provide its output to stdout.
    /// Intended to be used for command line tools like find, catkin_find, etc.
    /// Blocks until the called program has exited.
    /// @param runtimeEnvironment Dictionary that is merged with the current environment variables. Variables that already exist in the current environment are replaced.
    /// </summary>
    public static class Launcher
    {
        const int CHECK_TERMINATION_INTERVAL_MS = 10;       // delay in ms between checks of process running state while waiting for process termination

        public static ProcessOutput Run(string executablePath, string arguments, string workingDirectory = "", bool mayThrow = false, ILogger logger = null, LogLevel logLevel = LogLevel.Debug, IReadOnlyDictionary<string, string> runtimeEnvironment = null)
        {
            var output = new ProcessOutput();

            try
            {
                var startInfo = CreateStartInfo(executablePath, arguments, workingDirectory);
                if (runtimeEnvironment != null)
                {
                    ProcessHelper.MergeEnvironmentVariables(runtimeEnvironment, ProcessHelper.Concatenation.None, startInfo.Environment);
                }
                logger?.Log(logLevel, $"Running program {startInfo.FileName} {startInfo.Arguments}");

                logger?.LogDebug($"Starting ${startInfo.FileName} with following environment variables:");
                var importantVariables = new string[] {"ROS_PACKAGE_PATH", "CMAKE_PREFIX_PATH", "LD_LIBRARY_PATH", "PYTHONPATH", "PATH"};
                foreach(var (k, v) in startInfo.Environment.Where(item => importantVariables.Contains(item.Key)))
                {
                    logger?.LogDebug($"{k} -> {v}");
                }

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    CaptureOutput(process, output, logger);
                }
            }
            catch (Exception error)
            {
                logger?.Log(logLevel, error, "Process execution failed: " + error.Message);

                if (mayThrow)
                    throw;

                output.Suceeded = false;
                output.Error = error;
                output.ErrorMessage = error.Message;
            }

            if (mayThrow && output.ErrorMessage != null)
            {
                throw new Exception(output.ErrorMessage);
            }

            return output;
        }

        public static async Task<ProcessOutput> RunAsync(
            string executablePath,
            string arguments,
            string workingDirectory = "",
            bool mayThrow = false,
            ILogger logger = null,
            LogLevel logLevel = LogLevel.Debug,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            var output = new ProcessOutput();

            try
            {
                var startInfo = CreateStartInfo(executablePath, arguments, workingDirectory);
                logger?.Log(logLevel, $"Running program {startInfo.FileName} {startInfo.Arguments}");

                using (var process = Process.Start(startInfo))
                {
                    try
                    {
                        while (!process.HasExited)
                        {
                            await Task.Delay(CHECK_TERMINATION_INTERVAL_MS, cancel);       // check with <= 100 Hz
                        }
                    }
                    catch (TaskCanceledException e)
                    {
                        logger?.Log(logLevel, e, "Cancellation of process execution was requested, terminating process.");

                        try
                        {
                            // hard termination of running process
                            if (!process.HasExited)
                            {
                                process.Kill();
                                process.WaitForExit();
                            }
                        }
                        catch (InvalidOperationException)
                        {
                        }

                        if (mayThrow)
                            throw;

                        output.Error = e;
                    }

                    CaptureOutput(process, output, logger);
                }
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception error)
            {
                logger?.Log(logLevel, error, "Process execution failed: " + error.Message);

                if (mayThrow)
                    throw;

                output.Suceeded = false;
                output.Error = error;
                output.ErrorMessage = error.Message;
            }

            if (mayThrow && output.ErrorMessage != null)
            {
                throw new Exception(output.ErrorMessage);
            }

            return output;
        }

        private static ProcessStartInfo CreateStartInfo(string executablePath, string arguments, string workingDirectory) =>
            new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

        private static void CaptureOutput(Process process, ProcessOutput output, ILogger logger)
        {
            output.StandardOutput = process.StandardOutput.ReadToEnd();
            output.StandardError = process.StandardError.ReadToEnd();
            output.ExitCode = process.ExitCode;
            output.Suceeded = process.ExitCode == 0;
            if (output.ExitCode != 0)
            {
                output.ErrorMessage = $"Progam {process.StartInfo.FileName} exited with errorcode {process.ExitCode}";
                logger?.LogDebug(output.ErrorMessage);
            }
        }
    }
}
