using NUnit.Framework;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using PDFDataExtraction;

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
                .AddInMemoryCollection(new Dictionary<string, string?>
                {{"ApplicationPaths:PdfFolder", "..\\pdf"},
                    {"ApplicationPaths:OutputFolder", "..\\output"},
                    {"ApplicationPaths:ValidFolder","..\\valid"},
                    {"ApplicationsPaths:LogsFolder","..\\logs"}}).Build();
        
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
            service.StartMonitoring();
            service.StopMonitoring();
            Assert.That(service.IsMonitoring, Is.EqualTo(false));
        }
    }
}
