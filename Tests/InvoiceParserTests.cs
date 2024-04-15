using NUnit.Framework;
using PDFDataExtraction;
using PDFDataExtraction.Utility;
using System;

namespace PDFDataExtractionTests
{
    [TestFixture]
    public class InvoiceParserTests
    {
        private IInvoiceParser _parser;

        [SetUp]
        public void Setup()
        {
            // Initialize your parser here if needed
            _parser = new InvoiceParser();
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
            Assert.That(result, Is.EqualTo("N/A"));
        }

        [Test]
        public void ExtractInvoiceDate_ValidDate_ReturnsFormattedDate()
        {
            // Arrange
            string inputText = "The invoice date is 15/04/2023.";
            string pattern = @"\d{2}/\d{2}/\d{4}";

            // Act
            var result = _parser.ExtractInvoiceDate(inputText, pattern);

            // Assert
            Assert.That(result, Is.EqualTo("15/04/2023"));
        }

        [Test]
        public void ExtractTotalWithoutVAT_ValidInput_ReturnsDecimal()
        {
            // Arrange
            string inputText = "Total without VAT: 1234.56";
            string pattern = @"Total without VAT: (\d+(\.\d{1,2})?)";

            // Act
            decimal result = _parser.ExtractTotalWithoutVAT(inputText, pattern);

            // Assert
            Assert.That(result, Is.EqualTo(1234.56m));
        }

        [Test]
        public void ExtractTotalPrice_ValidInput_ReturnsDecimal()
        {
            // Arrange
            string inputText = "Total price: 1500.00";
            string pattern = @"Total price: (\d+(\.\d{1,2})?)";

            // Act
            decimal result = _parser.ExtractTotalPrice(inputText, pattern);

            // Assert
            Assert.That(result, Is.EqualTo(1500.00m));
        }

        [Test]
        public void ExtractDueDate_ValidDate_ReturnsFormattedDate()
        {
            // Arrange
            string inputText = "Due date: 31/12/2023";
            string pattern = @"\d{2}/\d{2}/\d{4}";

            // Act
            string result = _parser.ExtractDueDate(inputText, pattern);

            // Assert
            Assert.That(result, Is.EqualTo("31/12/2023"));
        }

        [Test]
        public void ExtractIvaPercentage_ValidInput_ReturnsPercentage()
        {
            // Arrange
            string inputText = "6.00                2.08";
            string pattern = @"\b(0\.00|6\.00|13\.00|23\.00)\s+(\d+\.\d{2})";

            // Act
            string result = _parser.ExtractIvaPercentage(inputText, pattern);

            // Assert
            Assert.That(result, Is.EqualTo("6.00 2.08"));
        }

        // Example test case for product detail extraction
        [Test]
        public void ExtractProductDetails_ValidInput_ReturnsProducts()
        {
            // Arrange
            string inputText = "8703987 STELATOPIA PLUS CR RELIP 300ML 7240523 -R180 6 UN 15.50 14.00 79.98 3";
            string pattern = @"(\bPT?\d+|\b\d+)\s+(.+?)\s+(\d+)\s*([-\w]*)\s+(\d+\sUN)\s+(\d+\.\d{2})\s+(\d+\.\d{2})\s+(\d+\.\d{2})\s+(\d+)";

            // Act
            var products = _parser.ExtractProductDetailsLEX(inputText, pattern);

            // Assert
            Assert.That(products, Is.Not.EqualTo(null));
            Assert.That(products.Count, Is.Not.EqualTo(0));
            Assert.That(products.Count, Is.EqualTo(1)); //One product match
            var firstProduct = products[0];
            Assert.That(firstProduct.Code, Is.EqualTo("8703987"));
            Assert.That(firstProduct.Quantity, Is.EqualTo(6));
            Assert.That(firstProduct.UnitPrice, Is.EqualTo(15.50m));
        }

    }
}
