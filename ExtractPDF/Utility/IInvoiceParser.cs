using System.Collections.Generic;
using PDFDataExtraction.Models;

namespace PDFDataExtraction.Utility
{
    public interface IInvoiceParser
    {
        string ExtractOrderNumber(string text, string pattern);
        string ExtractInvoiceNumber(string text, string pattern);
        decimal ExtractTotalWithoutVAT(string text, string pattern);
        decimal ExtractTotalPrice(string text, string pattern);
        string ExtractInvoiceDate(string text, string pattern);
        string ExtractDueDate(string text, string pattern);
        string ExtractIvaPercentage(string text, string pattern);
        List<Product> ExtractProductDetails(string invoiceText, string pattern);
        List<Product> ExtractProductDetailsLEX(string invoiceText, string pattern);
    }
    
}
