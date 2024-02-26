using System;
using System.IO;

namespace PDFDataExtraction
{
    public class dbSetup
    {

        string baseDirectory = Directory.GetCurrentDirectory();

        private static string[] attributes = { "NEncomenda", "totalsemIVA", "totalcomIVA", "DataFatura", "DataVencimento", "IVA", "Produto" };
        // Connection string for the local database

        public void SetupDatabase()
        {

            baseDirectory = Directory.GetParent(baseDirectory).FullName;

            string folderPath = Path.Combine(baseDirectory, "db");
            // Create the database folder if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //check if the file exists, if not create a .txt file for each attribute
            foreach (string attribute in attributes)
            {
                string path = Path.Combine(folderPath, attribute + ".txt");
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

            }


            Console.WriteLine("Database setup completed.");
        }
        //get patterns from the .txt files and put each line into an array position, being the first position the first line of the file
        public string[] GetPatterns(string attribute)
        {
            string path = Path.Combine(baseDirectory, "db", attribute + ".txt");
            string[] patterns = File.ReadAllLines(path);
            return patterns;
        }
    }
}
