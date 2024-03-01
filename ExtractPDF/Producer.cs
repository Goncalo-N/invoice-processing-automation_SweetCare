using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class Producer
{
    private string connectionString = "server=localhost;user=root;database=invoicedb;port=3306;password=default;";

    public List<string> GetAllCompanyNames()
    {
        List<string> companyNames = new List<string>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT nome_empresa FROM regex";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
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

    //method to get all the regex into a list
    public List<string> GetAllRegex(string nomeEmpresa)
    {
        List<object> regexList = new List<object>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Regex WHERE nome_empresa = @nomeEmpresa";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nomeEmpresa", nomeEmpresa);
                using (MySqlDataReader reader = command.ExecuteReader())
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
