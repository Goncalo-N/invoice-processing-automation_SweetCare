using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;


namespace PDFDataExtraction
{
    class Program
    {

        // global instance of Producer
        static Producer dbHelper = new Producer();


        static void Main(string[] args)
        {

            // Get the base directory of the project
            string baseDirectory = Directory.GetCurrentDirectory();

            // Navigate up 3 levels to reach invoice-processing-automation-main directory
            for (int i = 0; i <= 3; i++)
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
        //Method to check the file to see what company it belongs to
        static string CheckCompany(string text, List<string> companyNames)
        {

            foreach (string company in companyNames)
            {
                Match match = Regex.Match(text, company, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Value;
                }
            }
            return "N/A";
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
            // Get all company names from the database
            List<string> companyNames = dbHelper.GetAllCompanyNames();
            // Extract text from PDF
            string invoiceText = ExtractTextFromPDF(pdfFilePath);
            // Check the company name
            string companyName = CheckCompany(invoiceText, companyNames);

            // return if company name is N/A
            if (companyName == "N/A")
            {
                Console.WriteLine("Company not found");
                return;
            }

            // Get the regex for the company
            List<string> regex = dbHelper.GetAllRegex(companyName);
            Console.WriteLine("Company Name: " + companyName);

            /*  foreach (string value in regex)
              {
                  Console.WriteLine("regex array values: " + value);
              }*/
            // Maybe extract logo from invoice to check the company?
            //int empresa = ExtractLogo(pdfFilePath);
            // Extract other invoice information
            //create a condition that based on the company name it will call the correct method;
            string numEncomenda = "N/A";
            string numFatura = "N/A";
            decimal totalSemIVA = 0;
            decimal totalPrice = 0;
            string invoiceDate = "N/A";
            string dueDate = "N/A";
            decimal IVA = 0;

            switch (companyName)
            {
                case "Roger & Gallet":
                    invoiceDate = RG.ExtractInvoiceDateRG(invoiceText, regex[3]);
                    numEncomenda = RG.ExtractNumEncomendaRG(invoiceText, regex[4]);
                    Console.WriteLine("aaaaaaaaaaaaaa: " + regex[8]);
                    numFatura = RG.ExtractNumFaturaRG(invoiceText, regex[5]);
                    dueDate = RG.ExtractDueDateRG(invoiceText, regex[6]);
                    totalSemIVA = RG.ExtractTotalSemIVARG(invoiceText, regex[7]);
                    totalPrice = RG.ExtractTotalPriceRG(invoiceText, regex[11]);
                    IVA = RG.ExtractIVAPercentageRG(invoiceText, regex[13]);

                    break;
                case "":
                    //ExtractMEO(pdfFilePath, outputFolderPath, validatedFolderPath, invoiceText, regex);
                    break;
                case "s":
                    //ExtractRG(pdfFilePath, outputFolderPath, validatedFolderPath, invoiceText, regex);
                    break;
                case "N/A":
                    Console.WriteLine("Company not found");
                    break;
            }

            // Generate output file path
            string outputFileName = Path.GetFileNameWithoutExtension(pdfFilePath) + "_data.txt";
            string outputFilePath = Path.Combine(outputFolderPath, outputFileName);

            // Write extracted data to the output file
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine(invoiceText);
                writer.WriteLine("Data da Fatura: " + invoiceDate);
                writer.WriteLine("Nº Encomenda: " + numEncomenda);
                writer.WriteLine("Nº Fatura: " + numFatura);
                writer.WriteLine("Data Vencimento: " + dueDate);
                writer.WriteLine("Total sem IVA: " + totalSemIVA);
                writer.WriteLine("Total com IVA: " + totalPrice);
                writer.WriteLine("Taxa IVA: " + IVA + "%");

                // Write product details

                List<Product> products = RG.ExtractProductDetailsRG(invoiceText, regex[12]);
                foreach (var product in products)
                {
                    writer.WriteLine("--------Start of Product---------");
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

    }
}