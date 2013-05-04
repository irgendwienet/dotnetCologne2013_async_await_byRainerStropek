using AsyncAwaitDemo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AsyncUnitTest
{
    [TestClass]
    public class TestAsyncSensor
    {
        [TestMethod]
        public void TestConnectDisconnect()
        {
            var sensor = new SyncHeatSensor();
            sensor.Connect(Dns.GetHostAddresses("localhost").First());
            Assert.IsTrue(sensor.TryDisconnect());
        }

        [TestMethod]
        public async Task TestConnectDisconnectAsync()
        {
            var sensor = new AsyncHeatSensor();
            await sensor.ConnectAsync((await Dns.GetHostAddressesAsync("localhost")).First());
            Assert.IsTrue(await sensor.TryDisconnectAsync());
        }
    }
}