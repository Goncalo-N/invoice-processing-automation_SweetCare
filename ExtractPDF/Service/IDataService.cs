using System.Collections.Generic;

namespace PDFDataExtraction.Service
{
    public interface IDataService
    {
        List<string> GetAllCompanyNames();
        List<string> GetAllRegex(string companyName);
        int GetEmpresaID(string companyName);
        int GetOrderID(string invoiceNumber);
        bool ValidateProduct(string productCode, int orderID, decimal netPrice, decimal unitPrice, int quantity, string supplierInvoiceNumber, int isFactUpdated);
    }
}
