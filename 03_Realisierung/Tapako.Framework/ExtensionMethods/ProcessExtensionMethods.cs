using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Tapako.Framework.ExtensionMethods
{
    public static class ProcessExtensionMethods
    {
        public static async Task StartAsync(this Process process, TaskCompletionSource<bool> processStarted = null)
        {
            // there is no non-generic TaskCompletionSource
            var tcs = new TaskCompletionSource<bool>();

            process.EnableRaisingEvents = true;

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(true);
                process.Dispose();
            };

            process.Start();

            if (processStarted != null) processStarted.SetResult(true);

            await tcs.Task;
        }
    }
}
