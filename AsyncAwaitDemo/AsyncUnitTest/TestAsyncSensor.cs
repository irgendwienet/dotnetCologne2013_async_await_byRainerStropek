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
    }
}