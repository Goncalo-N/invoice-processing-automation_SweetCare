using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PDFDataExtraction
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseDirectory = Directory.GetCurrentDirectory();

            // Navigate up 4 levels to reach invoice-processing-automation-main directory
            for (int i = 0; i < 4; i++)
            {
                baseDirectory = Directory.GetParent(baseDirectory).FullName;
            }

            Console.WriteLine("Base directory: " + baseDirectory);
            string folderPath = Path.Combine(baseDirectory, "pdfs");
            string outputFolderPath = Path.Combine(baseDirectory, "output");
            string validatedFolderPath = Path.Combine(baseDirectory, "validated");
            // Start a separate thread to monitor the PDF folder
            System.Threading.Tasks.Task.Run(() => MonitorPdfFolder(folderPath, outputFolderPath, validatedFolderPath));
            while (true)
            {
                System.Threading.Thread.Sleep(1000); // Sleep for 1 second
            }
        }

        static void MonitorPdfFolder(string folderPath, string outputFolderPath, string validatedFolderPath)
        {
            // Create a timer with a 5-minute interval
            System.Timers.Timer timer = new System.Timers.Timer(5 * 60 * 1000);
            timer.Elapsed += (sender, e) => CheckFolderForNewPDFs(folderPath, outputFolderPath, validatedFolderPath);
            timer.AutoReset = true;
            timer.Start();

            // Initial check when starting the program
            CheckFolderForNewPDFs(folderPath, outputFolderPath, validatedFolderPath);
        }

        static void CheckFolderForNewPDFs(string folderPath, string outputFolderPath, string validatedFolderPath)
        {
            string[] newPdfFiles = Directory.GetFiles(folderPath, "*.pdf");
            foreach (string pdfFilePath in newPdfFiles)
            {
                OnPdfFileCreated(pdfFilePath, outputFolderPath, validatedFolderPath);
            }
        }

        static void OnPdfFileCreated(string pdfFilePath, string outputFolderPath, string validatedFolderPath)
        {
            Console.WriteLine($"New PDF file detected: {pdfFilePath}");

            // Extract text from PDF
            string invoiceText = ExtractTextFromPDF(pdfFilePath);

            // Maybe extract logo from invoice to check the company?
            //int empresa = ExtractLogo(pdfFilePath);
            // Extract other invoice information
            string numEncomenda = ExtractNumEncomenda(invoiceText);
            decimal totalSemIVA = ExtractTotalSemIVA(invoiceText);
            decimal totalPrice = ExtractTotalPrice(invoiceText);
            string invoiceDate = ExtractInvoiceDate(invoiceText);
            string dueDate = ExtractDueDate(invoiceText);
            decimal IVA = ExtractIVAPercentage(invoiceText);

            // Generate output file path
            string outputFileName = Path.GetFileNameWithoutExtension(pdfFilePath) + "_data.txt";
            string outputFilePath = Path.Combine(outputFolderPath, outputFileName);

            // Write extracted data to the output file
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine(invoiceText);
                writer.WriteLine("Nº Encomenda: " + numEncomenda);
                writer.WriteLine("Total sem IVA: " + totalSemIVA);
                writer.WriteLine("Total com IVA: " + totalPrice);
                writer.WriteLine("Data da Fatura: " + invoiceDate);
                writer.WriteLine("Data Vencimento: " + dueDate);
                writer.WriteLine("Taxa IVA: " + IVA + "%");

                // Write product details
                List<Product> products = ExtractProductDetails(invoiceText);
                foreach (var product in products)
                {
                    writer.WriteLine("Article: " + product.Article);
                    writer.WriteLine("Barcode: " + product.Barcode);
                    writer.WriteLine("Description: " + product.Description);
                    writer.WriteLine("Quantity: " + product.Quantity);
                    writer.WriteLine("GrossPrice: " + product.GrossPrice);
                    writer.WriteLine("Discount1: " + product.Discount1 + "%");
                    writer.WriteLine("Discount2: " + product.Discount2 + "%");
                    writer.WriteLine("Discount3: " + product.Discount3 + "%");
                    writer.WriteLine("PrecoSemIVA: " + product.PrecoSemIVA);
                    writer.WriteLine("PrecoComIVA: " + product.PrecoComIVA);
                    writer.WriteLine("--------End of Product---------");
                }
            }

            // Move processed PDF file to validated folder
            string fileName = Path.GetFileName(pdfFilePath);
            string destinationFilePath = Path.Combine(validatedFolderPath, fileName);
            //File.Move(pdfFilePath, destinationFilePath);
            Console.WriteLine("Data written to " + outputFilePath);
            Console.WriteLine($"Moved PDF file to Validated folder: {pdfFilePath}");
        }

        static string ExtractTextFromPDF(string filePath)
        {
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                StringWriter output = new StringWriter();
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    PdfPage page = pdfDoc.GetPage(i);
                    ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(page, strategy);
                    output.WriteLine(text);
                }
                return output.ToString();
            }
        }

        static string ExtractNumEncomenda(string text)
        {
            // create an array of strings containing the possible patterns for the order number
            string[] patterns = new string[] { @"\b[A-Za-z]+\d+CRM\d+\b", @"N encomen\s*:\s*(\d+)" };
            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(text, pattern);
                if (match.Success)
                {
                    return match.Value;
                }
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
            string[] patterns = new string[] { @"Total com IVA\s*([\d,]+)\s*EUR", @"Total\s*([\d,]+)\s*EUR" };
            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(text, pattern);
                if (match.Success)
                {
                    string totalPriceStr = match.Groups[1].Value.Replace(",", ".");
                    return decimal.Parse(totalPriceStr, CultureInfo.InvariantCulture);
                }
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
            //string [] patterns = new string[] { @"(?<Article>\w+)\s+(?<Barcode>\d+)\s+(?<Description>.+?)\s+(?<Quantity>\d+)\s+(?<GrossPrice>[\d,]+)\s+(?<Discount>[\d,]+)\s+(?<PrecoSemIVA>[\d,]+)\s+(?<PrecoComIVA>[\d,]+)" };

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
}