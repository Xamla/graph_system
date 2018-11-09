using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xamla.Utilities
{
    public delegate void StringReceivedEventHandler(object sender, string e);

    /**
     * This is a helper class to mitigate against a dotnetcore framework bug. It can be removed once this issue is resolved: https://github.com/dotnet/corefx/issues/32089
     */
    public static class ProcessEx
    {
        public static Process BeginOutputReadLine(this Process process, StringReceivedEventHandler handler, CancellationToken cancel = default(CancellationToken))
        {
            BeginRead(process, process.StandardOutput, handler, cancel);
            return process;
        }

        public static Process BeginErrorReadLine(this Process process, StringReceivedEventHandler handler, CancellationToken cancel = default(CancellationToken))
        {
            BeginRead(process, process.StandardError, handler, cancel);
            return process;
        }

        private static async void BeginRead(Process process, StreamReader streamReader, StringReceivedEventHandler handler, CancellationToken cancel = default(CancellationToken))
        {
            while (!cancel.IsCancellationRequested)
            {
                string data = null;
                try
                {
                    data = await streamReader.ReadLineAsync().ConfigureAwait(false);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw e;
                }
                catch (ObjectDisposedException)
                {
                    break; // Treat as EOF
                }
                catch (InvalidOperationException)
                {
                    // Try again next time
                }

                if ((data != null) && (handler != null))
                {
                    handler(process, data);
                }
                await Task.Delay(1).ConfigureAwait(false); // very important otherwise the error still occurs
                await Task.Yield();
            }
        }
    }
}
