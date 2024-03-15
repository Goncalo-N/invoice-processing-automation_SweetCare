using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Use the SQL Server client namespace

public class Producer
{
    // Connection string to connect to the SQL Server database.
    // Ensure that your connection string is appropriate for SQL Server.
    private string connectionString = "Server=localhost;Database=sweet;Integrated Security=True;";

    // Retrieves all company names from the database.
    public List<string> GetAllCompanyNames()
    {
        List<string> companyNames = new List<string>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT nome_empresa FROM regex";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string companyName = reader.GetString(0);
                        companyNames.Add(companyName);
                    }
                }
            }
        }

        return companyNames;
    }

    // Retrieves all regex patterns associated with a given company name.
    public List<string> GetAllRegex(string nomeEmpresa)
    {
        List<object> regexList = new List<object>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Regex WHERE nome_empresa = @nomeEmpresa";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // For SQL Server, use the Add method with a value to prevent SQL injection.
                command.Parameters.Add(new SqlParameter("@nomeEmpresa", nomeEmpresa));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            regexList.Add(reader.GetValue(i));
                        }
                    }
                }
            }
        }

        return regexList.ConvertAll(x => x.ToString());
    }
}
