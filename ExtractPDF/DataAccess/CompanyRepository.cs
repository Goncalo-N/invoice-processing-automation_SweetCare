using Microsoft.Extensions.Configuration;
using static PDFDataExtraction.Utility.RegexParser;

namespace PDFDataExtraction.DataAccess
{
    public class CompanyRepository
    {
        // Path to the JSON file containing supplier patterns
        // Adjust the path as necessary to match your project structure
        private readonly List<SupplierPattern> _supplierPatterns;

        public CompanyRepository(IConfiguration configuration)
        {
            var jsonFilePath = configuration.GetValue<string>("SupplierPatternsPath");
            if (!string.IsNullOrEmpty(jsonFilePath) && File.Exists(jsonFilePath))
            {
                _supplierPatterns = LoadSupplierPatterns(jsonFilePath);
            }
            else
            {
                //file is not found
                Console.WriteLine("Supplier patterns file not found.");
            }
        }

        public List<string> GetAllCompanyNames()
        {
            // Using loaded JSON data instead of database query
            return _supplierPatterns.Select(sp => sp.NomeEmpresa).ToList();
        }
    }
}