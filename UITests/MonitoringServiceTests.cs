using NUnit.Framework;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic; // Required for Dictionary
using PDFDataExtraction; // Ensure this is the correct namespace for MonitoringService

namespace PDFDataExtraction.UITests
{
    [TestFixture]
    public class MonitoringServiceTests
    {
        private IConfiguration _configuration;

        [SetUp] // Setup method to initialize common objects for each test
        public void SetUp()
        {
            // Create an in-memory configuration for testing
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {{"ApplicationPaths:PdfFolder", "..\\pdf"},
                    {"ApplicationPaths:OutputFolder", "..\\output"},
                    {"ApplicationPaths:ValidFolder","..\\valid"}}).Build();
        }

        [Test]
        public void StartMonitoring_SetsIsMonitoringToTrue()
        {
            var service = new MonitoringService(_configuration);
            service.StartMonitoring();
            Assert.That(service.IsMonitoring, Is.EqualTo(true));
        }

        [Test]
        public void StopMonitoring_SetsIsMonitoringToFalse()
        {
            var service = new MonitoringService(_configuration);
            service.StartMonitoring(); // Start first to set it true
            service.StopMonitoring();
            Assert.That(service.IsMonitoring, Is.EqualTo(false));
        }
    }
}
