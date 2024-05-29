using System.Data.SqlClient;

namespace PDFDataExtraction.DataAccess

{
    public class OrderRepository
    {
        private readonly string connectionString;

        public OrderRepository(string connectionString)
        {
            this.connectionString = connectionString;
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

        public bool ValidateProduct(string productCNP, int orderID, decimal NetPrice, decimal UnitPrice, int Quantity, string invoiceNumber, int isFactUpdated)
        {
            // Check if the product is already validated from before.
            if (isFactUpdated == 1)
            {
                return true;
            }

            bool isValid = false;
            bool pricesMatch = false;
            bool quantityMatch = false;
            NetPrice = Math.Round(NetPrice, 2);
            UnitPrice = Math.Round(UnitPrice, 2);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT priceNoBonus, priceWithBonus, qntOrder FROM supplierOrderItems WHERE ref = @productCNP AND orderId = @orderId";
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.Add(new SqlParameter("@productCNP", productCNP));
                    selectCommand.Parameters.Add(new SqlParameter("@orderId", orderID));

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal priceNoBonus = reader.GetDecimal(0);
                            decimal priceWithBonus = reader.GetDecimal(1);
                            priceNoBonus = Math.Round(priceNoBonus, 4);
                            priceWithBonus = Math.Round(priceWithBonus, 4);
                            int quantity = reader.GetInt32(2);

                            // Check if prices and quantity match
                            pricesMatch = (priceNoBonus == UnitPrice && priceWithBonus == NetPrice);
                            quantityMatch = (quantity == Quantity);
                            Console.WriteLine("Prices match: " + priceNoBonus + "||" + priceWithBonus);
                            Console.WriteLine("Price Match 2: " + UnitPrice + "||" + NetPrice);
                            Console.WriteLine("Quantity match: " + quantity + "|| " + Quantity);
                        }
                    }
                }

                // If prices and quantity match, update database
                if (pricesMatch && quantityMatch)
                {
                    // Update database
                    string updateQuery = "UPDATE supplierOrderItems SET supplierInvoiceNumber = @invoiceNumber, isFactUpdated = 1 WHERE ref = @productCNP AND orderId = @orderId";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.Add(new SqlParameter("@invoiceNumber", invoiceNumber));
                        updateCommand.Parameters.Add(new SqlParameter("@productCNP", productCNP));
                        updateCommand.Parameters.Add(new SqlParameter("@orderId", orderID));

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        isValid = rowsAffected > 0;
                    }
                }
            }
            return isValid;
        }

    }
}