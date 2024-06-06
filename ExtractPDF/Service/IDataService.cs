namespace PDFDataExtraction.Service
{
    public interface IDataService
    {
        int GetOrderID(string invoiceNumber);
        bool ValidateProduct(string productCode, int orderID, decimal netPrice, decimal unitPrice, int quantity, string supplierInvoiceNumber, int isFactUpdated);
    }
}