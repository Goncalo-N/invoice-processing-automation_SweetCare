using PDFDataExtraction;

public class DataService
{
    private readonly CompanyRepository companyRepository;
    private readonly OrderRepository orderRepository;

    public DataService(string connectionString)
    {
        this.companyRepository = new CompanyRepository(connectionString);
        this.orderRepository = new OrderRepository(connectionString);
    }

    public List<string> GetAllCompanyNames()
    {
        return companyRepository.GetAllCompanyNames();
    }

    public List<string> GetAllRegex(string companyName)
    {
        return orderRepository.GetAllRegex(companyName);
    }

    public int GetEmpresaID(string companyName)
    {
        return companyRepository.GetEmpresaID(companyName);
    }

    public int GetOrderID(string invoiceNumber)
    {
        return orderRepository.GetOrderID(invoiceNumber);
    }

    public bool ValidateAndUpdateProducts(string productCode, int orderID, decimal netPrice, decimal unitPrice, int quantity)
    {
        return orderRepository.ValidateAndUpdateProducts(productCode, orderID, netPrice, unitPrice, quantity);
    }

    internal bool ValidateAndUpdateInvoice(int orderID, string invoiceNumber)
    {
        throw new NotImplementedException();
    }
}
