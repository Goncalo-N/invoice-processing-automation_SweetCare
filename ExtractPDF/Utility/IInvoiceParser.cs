using PDFDataExtraction.Models;

namespace PDFDataExtraction.Utility
{
    public interface IInvoiceParser
    {
        // Extracts the order number from the invoice text.
        string ExtractOrderNumber(string text, string pattern);

        // Extracts the invoice number from the invoice text.
        string ExtractInvoiceNumber(string text, string pattern);

        // Extracts the total amount without IVA from the invoice text.
        decimal ExtractTotalWithoutVAT(string text, string pattern);

        // Extracts the total amount from the invoice text.
        decimal ExtractTotalPrice(string text, string pattern);

        // Extracts the invoice date from the invoice text.
        string ExtractInvoiceDate(string text, string pattern);

        // Extracts the due date from the invoice text.
        string ExtractDueDate(string text, string pattern);

        // Extracts the IVA percentage from the invoice text.
        string ExtractIvaPercentage(string text, string pattern);

        // Extracts the product details from the invoice text.
        List<Product> ExtractProductDetails(string invoiceText, string pattern);

        // Extracts the product details from the invoice text.
        List<Product> ExtractProductDetailsLEX(string invoiceText, string pattern);
    }

}
