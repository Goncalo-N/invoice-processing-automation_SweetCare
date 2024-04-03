using System.Data.SqlClient;
using PDFDataExtraction;

public class OrderRepository
{
    private readonly string connectionString = "Server=localhost;Database=sweet;Trusted_Connection=True;";

    public OrderRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    // Retrieves all regex patterns associated with a given company name.
    public List<string> GetAllRegex(string nomeEmpresa)
    {
        List<object> regexList = new List<object>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM supplierRegex WHERE nome_empresa = @nomeEmpresa";
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
    //get orderID through invoiceNumber
    public int GetOrderID(string invoiceNumber)
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
    public bool ValidateProduct(string productCode, int orderID, decimal NetPrice, decimal UnitPrice, int Quantity, string invoiceNumber, int isFactUpdated)
    {

        //check if the product is already validated from before.
        if (isFactUpdated == 1)
        {
            return true;
        }

        bool isValid = false;
        bool pricesMatch = false;
        bool quantityMatch = false;
        NetPrice = Math.Round(NetPrice, 4);
        UnitPrice = Math.Round(UnitPrice, 4);


        //Console.WriteLine("ProductCode: " + productCode);
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string selectQuery = "SELECT priceNoBonus, priceWithBonus, qntOrder FROM supplierOrderItems WHERE ref = @productCode AND orderId = @orderId AND isFactUpdated = 0";
            using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
            {
                // Check for null or empty productCode and handle accordingly
                if (string.IsNullOrEmpty(productCode))
                {
                    throw new ArgumentException("Product code cannot be null or empty.", nameof(productCode));
                }

                selectCommand.Parameters.Add(new SqlParameter("@productCode", productCode));
                selectCommand.Parameters.Add(new SqlParameter("@orderId", orderID));

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal priceNoBonus = reader.GetDecimal(0);
                        decimal priceWithBonus = reader.GetDecimal(1);
                        int quantity = reader.GetInt32(2);
                        //checking priceNoBonus and priceWithBonus fields from db
                        if (priceNoBonus == NetPrice && priceWithBonus == UnitPrice)
                        {
                            Console.WriteLine("Price matched on productCode: " + productCode);
                            pricesMatch = true;
                        }

                        //checking quantity field from db
                        if (quantity == Quantity)
                        {
                            Console.WriteLine("Quantity matched on productCode: " + productCode);
                            quantityMatch = true;
                        }

                        if (pricesMatch && quantityMatch)
                        {

                            Program.log.Information("Product with code {productCode} has a null price field.", productCode);
                            string updateQuery = "UPDATE supplierOrderItems SET supplierInvoiceNumber = @invoiceNumber WHERE ref = @productCode AND orderId = @orderId AND isFactUpdated = 0";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.Add(new SqlParameter("@invoiceNumber", invoiceNumber));
                                updateCommand.Parameters.Add(new SqlParameter("@productCode", productCode));
                                updateCommand.Parameters.Add(new SqlParameter("@orderId", orderID));
                                //execute query
                                int rowsAffected = updateCommand.ExecuteNonQuery();
                                isValid = rowsAffected > 0;

                            }
                        }
                    }
                }
            }
        }
        return isValid;
    }
}