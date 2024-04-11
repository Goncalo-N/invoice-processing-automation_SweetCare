using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace PDFDataExtraction.DataAcess
{
    public class CompanyRepository
{
    private readonly string connectionString;

    public CompanyRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }
        public List<string> GetAllCompanyNames()
        {
            List<string> companyNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT nome_empresa FROM supplierRegex";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string companyName = reader.GetString(0);
                            //Console.WriteLine(companyName);
                            companyNames.Add(companyName);
                        }
                    }
                }
            }
            return companyNames;
        }

      
        //get supplierID through companyName
        public int GetEmpresaID(string companyName)
        {
            int empresaID = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID FROM suppliersName WHERE supplierName = @nomeEmpresa";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // For SQL Server, use the Add method with a value to prevent SQL injection.
                    command.Parameters.Add(new SqlParameter("@nomeEmpresa", companyName));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empresaID = reader.GetInt32(0);
                        }
                    }
                }
            }
            return empresaID;
        }

       

        //generic method to check general invoice details in the database
        public bool ValidateAndUpdateInvoice(int orderID, string invoiceNumber)
        {
            if (orderID != 0 && !string.IsNullOrEmpty(invoiceNumber))
                return true;
            return false;
        }
    }
}