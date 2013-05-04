using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitDemo
{
    public class AsyncHeatSensor
    {
        private bool isConnected = false;
        private object workInProgressLockObject = new object();

        public Task ConnectAsync(IPAddress address)
        {
            // Note that parameters are checked before the task is scheduled.
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }


            return TaskEx.Run(() =>
            {
                // Note that method calls are serialized using this lock statement.
                // If you want to specify a lock timeout, use Monitor.TryEnter(...)
                // instead of lock(...).
                lock (this.workInProgressLockObject)
                {
                    if (this.isConnected)
                    {
                        throw new InvalidOperationException("Already connected");
                    }

                    // Simulate connect
                    Thread.Sleep(3000);

                    this.isConnected = true;
                }
            });
        }

        public Task UploadFirmwareAsync(byte[] firmware, CancellationToken ct, IProgress<int> progress)
        {
            if (firmware == null)
            {
                throw new ArgumentNullException("firmeware");
            }

            return TaskEx.Run(() =>
            {
                lock (this.workInProgressLockObject)
                {
                    if (!this.isConnected)
                    {
                        throw new InvalidOperationException("Not connected");
                    }

                    // Simulate upload in chunks.
                    for (var i = 1; i <= 10; i++)
                    {
                        // Note that we throw an exception if cancellation has been requested.
                        ct.ThrowIfCancellationRequested();

                        // Simulate uploading of a chunk of data
                        Thread.Sleep(200);

                        // Report progress
                        progress.Report(i * 10);
                    }
                }
            }, ct);
        }

        public Task<bool> TryDisconnectAsync()
        {
            return TaskEx.Run(() =>
            {
                lock (this.workInProgressLockObject)
                {
                    if (!this.isConnected)
                    {
                        return false;
                    }

                    // Simulate disconnect
                    Thread.Sleep(500);

                    this.isConnected = false;
                    return true;
                }
            });
        }
    }
}