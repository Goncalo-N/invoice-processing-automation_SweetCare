using PDFDataExtraction;
using PDFDataExtraction.DataAccess;

namespace PDFDataExtraction.Service
{
    public class DataService : IDataService
    {
        private readonly CompanyRepository companyRepository;
        private readonly OrderRepository orderRepository;

        public DataService(string connectionString)
        {
            this.orderRepository = new OrderRepository(connectionString);
        }

        public int GetOrderID(string invoiceNumber)
        {
            return orderRepository.GetOrderID(invoiceNumber);
        }

        public bool ValidateProduct(string productCode, int orderID, decimal netPrice, decimal unitPrice, int quantity, string supplierInvoiceNumber, int isFactUpdated)
        {
            return orderRepository.ValidateProduct(productCode, orderID, netPrice, unitPrice, quantity, supplierInvoiceNumber, isFactUpdated);
        }

        internal bool ValidateInvoice(int orderID, string invoiceNumber)
        {
            throw new NotImplementedException();
        }
    }
}