/*using System;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace PDFDataExtraction
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = "C:/Users/nogue/Documents/PDF/pdfs";
            string outputFolderPath = "C:/Users/nogue/Documents/PDF/output";

            // Process existing PDF files in the folder
            string[] existingPdfFiles = Directory.GetFiles(folderPath, "*.pdf");
            foreach (string pdfFilePath in existingPdfFiles)
            {
                OnPdfFileCreated(pdfFilePath, outputFolderPath);
            }
            // Start a separate thread to monitor the PDF folder
            Task.Run(() => MonitorPdfFolder(folderPath, outputFolderPath));
            while (true)
            {
                Thread.Sleep(1000); // Sleep for 1 second
            }
        }

       /* static void MonitorPdfFolder(string folderPath, string outputFolderPath)
        {
            // Create a FileSystemWatcher to monitor the PDF folder
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = folderPath;
                watcher.Filter = "*.pdf";
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.IncludeSubdirectories = false;

                // Subscribe to the events
                watcher.Created += (sender, e) => OnPdfFileCreated(e.FullPath, outputFolderPath);

                // Start monitoring
                watcher.EnableRaisingEvents = true;

                // Keep the thread alive
                while (true)
                {
                    Thread.Sleep(5 * 60 * 1000); // Sleep for 5 minutes
                }
            }
        }
       
        static void OnPdfFileCreated(string pdfFilePath, string outputFolderPath)
        {
            Console.WriteLine($"New PDF file detected: {pdfFilePath}");

            //variable just to print the text of the invoice for debugging
            string invoiceText = ExtractTextFromPDF(pdfFilePath);

            string numEncomenda = ExtractNumEncomenda(invoiceText);

            decimal totalSemIVA = ExtractTotalSemIVA(invoiceText);

            decimal totalPrice = ExtractTotalPrice(invoiceText);

            string invoiceDate = ExtractInvoiceDate(invoiceText);

            string dueDate = ExtractDueDate(invoiceText);

            decimal IVA = ExtractIVAPercentage(invoiceText);

            // Generate a unique output file path for this invoice based on the name of the PDF file
            string outputFileName = System.IO.Path.GetFileNameWithoutExtension(pdfFilePath) + "_data.txt";
            string outputFilePath = System.IO.Path.Combine(outputFolderPath, outputFileName);

            List<Product> products = ExtractProductDetails(invoiceText);
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                // Writes the extracted data to the console
                //Console.WriteLine(invoiceText);
                foreach (var product in products)
                {
                    writer.WriteLine("Article: " + product.Article);
                    writer.WriteLine("Barcode: " + product.Barcode);
                    writer.WriteLine("Description: " + product.Description);
                    writer.WriteLine("Quantity: " + product.Quantity);
                    writer.WriteLine("Bonus: " + product.Bonus);
                    writer.WriteLine("Preço bruto sem IVA: " + product.GrossPrice);
                    writer.WriteLine("Discount1: " + product.Discount1 + "%");
                    writer.WriteLine("Discount2: " + product.Discount2 + "%");
                    writer.WriteLine("Discount3: " + product.Discount3 + "%");
                    writer.WriteLine("Preço sem IVA: " + product.PrecoSemIVA);
                    writer.WriteLine("Preço total: " + product.PrecoComIVA);
                    writer.WriteLine("--------End of Product--------- ");
                }

                writer.WriteLine("Nº Encomenda: " + numEncomenda);
                writer.WriteLine("Total sem IVA: " + totalSemIVA);
                writer.WriteLine("Total com IVA: " + totalPrice);
                writer.WriteLine("Data da Fatura: " + invoiceDate);
                writer.WriteLine("Data Vencimento: " + dueDate);
                writer.WriteLine("Taxa IVA: " + IVA + "%");
            }
            Console.WriteLine("Data written to " + outputFilePath);
        }


        // Method to extract text from PDF using iTextSharp library (https://sourceforge.net/projects/itextsharp/) 
        static string ExtractTextFromPDF(string filePath)
        {
            using (PdfReader reader = new PdfReader(filePath))
            {
                string text = "";
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text += PdfTextExtractor.GetTextFromPage(reader, i);
                }
                return text;
            }
        }

        // Method to extract invoice number using regular expression
        static string ExtractNumEncomenda(string text)
        {
            //explanation of the regular expression:
            //\b[A-Za-z]+\d+CRM\d+\b - 1 or more letters, followed by 1 or more digits, followed by CRM,
            //followed by 1 or more digits
            string pattern = @"\b[A-Za-z]+\d+CRM\d+\b";
            Match match = Regex.Match(text, pattern);
            if (match.Success)
            {
                return match.Value;
            }
            return "N/A";
        }

        // Method to extract total price sem IVA using regular expression
        static decimal ExtractTotalSemIVA(string text)
        {
            //explanation of the regular expression:
            //Total sem IVA\s*([\d,]+)\s* - Total sem IVA followed by 0 or more spaces,
            //followed by 1 or more digits or commas, followed by 0 or more spaces
            string pattern = @"Total sem IVA\s*([\d,]+)\s*";
            Match match = Regex.Match(text, pattern);
            if (match.Success)
            {
                string totalPriceStr = match.Groups[1].Value.Replace(",", ".");
                return decimal.Parse(totalPriceStr, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        // Method to extract total price using regular expression
        static decimal ExtractTotalPrice(string text)
        {
            //explanation of the regular expression:
            //Total com IVA\s*([\d,]+)\s*EUR - Total com IVA followed by 0 or more spaces,
            //followed by 1 or more digits or commas, followed by 0 or more spaces, followed by EUR
            string pattern = @"Total com IVA\s*([\d,]+)\s*EUR";
            Match match = Regex.Match(text, pattern);
            if (match.Success)
            {
                string totalPriceStr = match.Groups[1].Value.Replace(",", ".");
                return decimal.Parse(totalPriceStr, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        // Method to extract invoice date using regular expression
        static string ExtractInvoiceDate(string text)
        {
            //explanation of the regular expression:
            //Data\s+da\s+Fatura - Data followed by 1 or more spaces, followed by da,
            // followed by 1 or more spaces, followed by Fatura
            string pattern = @"Data\s+da\s+Fatura";
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                int startIndex = match.Index + match.Length;
                string remainingText = text.Substring(startIndex);
                Match dateMatch = Regex.Match(remainingText, @"\b\d{2}/\d{2}/\d{4}\b");
                if (dateMatch.Success)
                {
                    return dateMatch.Value;
                }
            }
            return "N/A";
        }

        // Method to extract due date using regular expression
        static string ExtractDueDate(string text)
        {

            //explanation of the regular expression:
            //Data\s+Vencimento - Data followed by 1 or more spaces, followed by Vencimento
            string pattern = @"Data\s+Vencimento";
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                int startIndex = match.Index + match.Length;
                string remainingText = text.Substring(startIndex);
                Match dateMatch = Regex.Match(remainingText, @"\b\d{2}/\d{2}/\d{4}\b");
                if (dateMatch.Success)
                {
                    return dateMatch.Value;
                }
            }
            return "N/A";
        }

        // Method to extract IVA percentage using regular expression
        static decimal ExtractIVAPercentage(string text)
        {
            //explanation of the regular expression: 
            //IVA\s*:?(\d+)% - IVA followed by 0 or more spaces, followed by an optional colon, followed by 1 or more digits, followed by %
            string pattern = @"IVA\s*:?(\d+)%";
            Match match = Regex.Match(text, pattern);
            if (match.Success)
            {
                return decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        // Method to extract product details using regular expression
        static List<Product> ExtractProductDetails(string invoiceText)
        {
            List<Product> products = new List<Product>();

            string pattern = @"(?<Article>\w+)\s+(?<Barcode>\d+)\s+(?<Description>.+?)\s+(?<Quantity>\d+)\s+(?<GrossPrice>[\d,]+)\s+(?<Discount>[\d,]+)\s+(?<PrecoSemIVA>[\d,]+)\s+(?<PrecoComIVA>[\d,]+)";
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                Product product = new Product();
                product.Article = match.Groups["Article"].Value;
                product.Barcode = match.Groups["Barcode"].Value;
                product.Description = match.Groups["Description"].Value;

                // Parse quantity if present
                int quantity;
                if (int.TryParse(match.Groups["Quantity"].Value, out quantity))
                {
                    product.Quantity = quantity;
                }

                // Parse bonus quantity if present
                int bonus;
                if (match.Groups["Bonus"].Success && int.TryParse(match.Groups["Bonus"].Value, out bonus))
                {
                    product.Bonus = bonus;
                }
                else
                {
                    product.Bonus = 0;
                }

                // Parse gross price if present
                decimal grossPrice;
                if (decimal.TryParse(match.Groups["GrossPrice"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out grossPrice))
                {
                    product.GrossPrice = grossPrice;
                }

                // Parse discount if present
                decimal discount;
                if (decimal.TryParse(match.Groups["Discount"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out discount))
                {
                    product.Discount1 = discount;
                    product.Discount2 = discount;
                    product.Discount3 = discount;
                }

                // Parse net price if present
                decimal precoSemIVA;
                if (decimal.TryParse(match.Groups["PrecoSemIVA"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out precoSemIVA))
                {
                    product.PrecoSemIVA = precoSemIVA;
                }

                // Parse net price with IVA if present
                decimal precoComIVA;
                if (decimal.TryParse(match.Groups["PrecoComIVA"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out precoComIVA))
                {
                    product.PrecoComIVA = precoComIVA;
                }

                products.Add(product);
            }


            return products;
        }


        // Method to parse decimal value from string
        static decimal ParseDecimal(string value)
        {
            return decimal.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture);
        }

    }
}*/