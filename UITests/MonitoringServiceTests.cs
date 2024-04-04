using NUnit.Framework;
using System.Threading;

namespace PDFDataExtraction.UITests
{
    [TestFixture]
    public class MonitoringServiceTests
    {
        [Test]
        public void StartMonitoring_SetsIsMonitoringToTrue()
        {
            var service = new MonitoringService();
            service.StartMonitoring();
            Assert.That(service.IsMonitoring, Is.EqualTo(true)); // NUnit syntax
        }

        [Test]
        public void StopMonitoring_SetsIsMonitoringToFalse()
        {
            var service = new MonitoringService();
            service.StartMonitoring(); // Start first to set it true
            service.StopMonitoring();
            Assert.That(service.IsMonitoring, Is.EqualTo(false)); // NUnit syntax
        }

    }
}
