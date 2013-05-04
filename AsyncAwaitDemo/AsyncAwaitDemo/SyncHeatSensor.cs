using System;
using System.Net;
using System.Threading;

namespace AsyncAwaitDemo
{
    public class SyncHeatSensor
    {
        /// <summary>
        /// Flag indicating whether the sensor is connected.
        /// </summary>
        private bool isConnected = false;

        public void Connect(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (this.isConnected)
            {
                throw new InvalidOperationException("Already connected");
            }

            // Simulate connect
            Thread.Sleep(3000);

            this.isConnected = true;
        }

        public void UploadFirmware(byte[] firmware)
        {
            if (firmware == null)
            {
                throw new ArgumentNullException("firmeware");
            }

            if (!this.isConnected)
            {
                throw new InvalidOperationException("Not connected");
            }

            for (var i = 0; i < 10; i++)
            {
                // Simulate uploading of a chunk of data
                Thread.Sleep(200);
            }
        }

        public bool TryDisconnect()
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
    }
}