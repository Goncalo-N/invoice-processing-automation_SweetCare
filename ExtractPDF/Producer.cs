using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Use the SQL Server client namespace

public class Producer
{
    // Connection string to connect to the SQL Server database.
    // Ensure that your connection string is appropriate for SQL Server.
    private string connectionString = "Server=localhost;Database=sweet;Trusted_Connection=True;";
    //private string connectionString = "Server=localhost;Database=sweet;Integrated Security=True;";

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
                        //Console.WriteLine(companyName);
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
    //get supplierID through companyName
    public int getEmpresaID(string companyName)
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

    //get orderID through invoiceNumber
    public int getOrderID(string invoiceNumber)
    {
        int orderID = 0;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT orderId FROM supplierOrderItems WHERE supplierInvoiceNumber = @invoiceNumber";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // For SQL Server, use the Add method with a value to prevent SQL injection.
                command.Parameters.Add(new SqlParameter("@invoiceNumber", invoiceNumber));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orderID = reader.GetInt32(0);
                    }
                }
            }
        }
        return orderID;
    }

    //validate products
    public bool ValidateAndUpdateProducts(string productCode, int orderID, decimal NetPrice, decimal UnitPrice)
    {
        bool isValid = false;
        bool needsUpdate = false;
        NetPrice = Math.Round(NetPrice, 4);
        Console.WriteLine("ProductCode: " + productCode);
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string selectQuery = "SELECT priceNoBonus, priceWithBonus FROM supplierOrderItems WHERE ref = @productCode AND orderId = @orderId";
            using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
            {
                // Check for null or empty productCode and handle accordingly
                if (string.IsNullOrEmpty(productCode))
                {
                    // Depending on your use case, you might want to return false, throw a new exception, or handle the null case differently
                    //throw new ArgumentException("Product code cannot be null or empty.", nameof(productCode));
                    return false;
                }

                selectCommand.Parameters.Add(new SqlParameter("@productCode", productCode ?? string.Empty)); // Using ?? operator as a safeguard
                selectCommand.Parameters.Add(new SqlParameter("@orderId", orderID));

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal priceNoBonus = reader.GetDecimal(0);
                        decimal priceWithBonus = reader.GetDecimal(1);
                        if (UnitPrice == priceNoBonus && NetPrice == priceWithBonus)
                        {
                            isValid = true;
                        }
                        else
                        {
                            needsUpdate = true;
                        }
                    }
                }
            }

            if (needsUpdate)
            {
                string updateQuery = "UPDATE supplierOrderItems SET priceNoBonus = @UnitPrice, priceWithBonus = @NetPrice WHERE ref = @productCode AND orderId = @orderId";
                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.Add(new SqlParameter("@UnitPrice", UnitPrice));
                    updateCommand.Parameters.Add(new SqlParameter("@NetPrice", NetPrice));
                    updateCommand.Parameters.Add(new SqlParameter("@productCode", productCode));
                    updateCommand.Parameters.Add(new SqlParameter("@orderId", orderID));

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    isValid = rowsAffected > 0;
                }
            }
        }
        return isValid;
    }

}
