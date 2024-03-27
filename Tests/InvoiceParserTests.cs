using NUnit.Framework;
using PDFDataExtraction;
using System;

namespace PDFDataExtractionTests
{
    [TestFixture]
    public class InvoiceParserTests
    {
        private InvoiceParser _parser;

        [SetUp]
        public void Setup()
        {
            // Initialize your parser here if needed
            _parser = new InvoiceParser();
        }

        [Test]
        public void ExtractInvoiceDate_ValidDate_ReturnsFormattedDate()
        {
            // Arrange
            string inputText = "The invoice date is 15/04/2023.";
            string pattern = @"\d{2}/\d{2}/\d{4}";

            // Act
            var result = _parser.ExtractInvoiceDate(inputText, pattern);
            Console.WriteLine(result);
            // Assert
            Assert.AreEqual("15/04/2023", result);
        }

        [Test]
        public void ExtractInvoiceDate_InvalidDate_ReturnsNA()
        {
            // Arrange
            string inputText = "The invoice date is invalid.";
            string pattern = @"The invoice date is (\d{2}/\d{2}/\d{4}).";

            // Act
            var result = _parser.ExtractInvoiceDate(inputText, pattern);

            // Assert
            Assert.AreEqual("N/A", result);
        }

        
    }
}
