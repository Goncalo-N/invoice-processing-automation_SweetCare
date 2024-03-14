using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class Producer
{
    // Connection string to connect to the MySQL database.
    private string connectionString = "server=localhost;user=root;database=invoicedb;port=3306;password=default;";

    // Retrieves all company names from the database.
    public List<string> GetAllCompanyNames()
    {
        // Initialize an empty list to store company names.
        List<string> companyNames = new List<string>();

        // Establish a connection to the database using the connection string.
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            // Open the database connection.
            connection.Open();

            // SQL query to select all company names from the 'regex' table.
            string query = "SELECT nome_empresa FROM regex";
            
            // Execute the query using a MySqlCommand.
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // Execute the command and use a reader to fetch the results.
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // Read each row in the result set.
                    while (reader.Read())
                    {
                        // Get the company name from the current row (first column) and add it to the list.
                        string companyName = reader.GetString(0);
                        companyNames.Add(companyName);
                    }
                }
            }
        }

        // Return the list of company names.
        return companyNames;
    }

    // Retrieves all regex patterns associated with a given company name.
    public List<string> GetAllRegex(string nomeEmpresa)
    {
        // Initialize a list to store the regex patterns (stored as objects initially).
        List<object> regexList = new List<object>();

        // Establish a connection to the database using the connection string.
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            // Open the database connection.
            connection.Open();

            // SQL query to select all columns from 'Regex' table where 'nome_empresa' matches the input parameter.
            string query = "SELECT * FROM Regex WHERE nome_empresa = @nomeEmpresa";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // Add the 'nomeEmpresa' parameter to the command to prevent SQL injection.
                command.Parameters.AddWithValue("@nomeEmpresa", nomeEmpresa);
                
                // Execute the command and use a reader to fetch the results.
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // Read each row in the result set.
                    while (reader.Read())
                    {
                        // Iterate through all columns in the current row.
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Add each column value to the regex list.
                            regexList.Add(reader.GetValue(i));
                        }
                    }
                }
            }
        }

        // Convert the object list to a list of strings and return it.
        // This assumes all database fields are convertible to strings.
        return regexList.ConvertAll(x => x.ToString());
    }
}
