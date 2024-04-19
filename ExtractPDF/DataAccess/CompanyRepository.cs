using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using PDFDataExtraction.Utility;
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
            _supplierPatterns = LoadSupplierPatterns(jsonFilePath);
        }

        public List<string> GetAllCompanyNames()
        {
            // Using loaded JSON data instead of database query
            return _supplierPatterns.Select(sp => sp.NomeEmpresa).ToList();
        }

        // GetEmpresaID is adapted to work with the in-memory list instead of querying the database
        // Assuming a simple mapping of company name to a unique ID based on index (or any other logic as needed)
        public int GetEmpresaID(string companyName)
        {
            var supplier = _supplierPatterns.FirstOrDefault(sp => sp.NomeEmpresa.Equals(companyName, StringComparison.OrdinalIgnoreCase));
            if (supplier != null)
            {
                // Assuming we return the index as a makeshift ID; adjust as necessary
                return _supplierPatterns.IndexOf(supplier) + 1;
            }
            return 0;
        }
    }
}