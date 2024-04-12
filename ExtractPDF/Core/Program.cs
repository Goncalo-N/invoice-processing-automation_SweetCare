using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Configuration;
using PDFDataExtraction.Models;
using PDFDataExtraction.Service;
using PDFDataExtraction.Utility;
using Serilog;

namespace PDFDataExtraction.Core
{

    public class Program
    {
        static readonly DataService dataService = new DataService("Server=localhost;Database=sweet;Trusted_Connection=True;");
        static readonly InvoiceParser invoiceParser = new InvoiceParser();

        static readonly Serilog.Core.Logger log;

        // Static constructor to initialize static readonly fields
        static Program()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Initialize logger
            log = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            // Initialize dataService with the connection string from configuration
            string connectionString = configuration.GetSection("DatabaseConfiguration:ConnectionString").Value;
            dataService = new DataService(connectionString);
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

            log.Information("Application Starting");

            // Since paths now come from configuration, ensure the static constructor has run
            // to initialize the configuration and log before accessing them.
            string baseDirectory = Directory.GetCurrentDirectory();
            string pdfFolder = Path.Combine(baseDirectory, "pdfs");
            string outputFolder = Path.Combine(baseDirectory, "output");
            string validFolder = Path.Combine(baseDirectory, "valid");
            string invalidFolder = Path.Combine(baseDirectory, "invalid");
            string missingValuesFolder = Path.Combine(baseDirectory, "missing");

            Task.Run(() => MonitorPdfFolder(pdfFolder, outputFolder, validFolder));
            PreventApplicationExit();
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error($"Unhandled exception: {e.ExceptionObject}");
            Environment.Exit(1);
        }

        static void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            log.Error($"Unobserved task exception: {e.Exception}");
            e.SetObserved();
        }
        public static string GetBaseDirectory()
        {
            var baseDirectory = Directory.GetCurrentDirectory();
            var parentDirectory = Directory.GetParent(baseDirectory);
            baseDirectory = parentDirectory?.FullName ?? baseDirectory;
            log.Information($"Base directory: {baseDirectory}");
            Console.WriteLine($"Base directory: {baseDirectory}");
            return baseDirectory;
        }

        public static (string folderPath, string outputFolderPath, string validatedFolderPath, string invalidFolerPath) GetFolderPaths(string baseDirectory)
        {
            return (
                Path.Combine(baseDirectory, "pdfs"),
                Path.Combine(baseDirectory, "output"),
                Path.Combine(baseDirectory, "valid"),
                Path.Combine(baseDirectory, "invalid")
            );
        }

        static void PreventApplicationExit()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        //Method to extract text from PDF using iText7
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
        public static void MonitorPdfFolder(string folderPath, string outputFolderPath, string validatedFolderPath)
        {
            // Create a timer with a 5-minute interval
            System.Timers.Timer timer = new System.Timers.Timer(5 * 60 * 1000);
            timer.Elapsed += (sender, e) => CheckFolderForNewPDFs(folderPath, outputFolderPath, validatedFolderPath);
            timer.AutoReset = true;
            timer.Start();

            // Initial check when starting the program
            CheckFolderForNewPDFs(folderPath, outputFolderPath, validatedFolderPath);
        }

        public static void MonitorPdfFolder(string folderPath, string outputFolderPath, string validatedFolderPath, CancellationToken cancellationToken)
        {
            // Create a timer with a 5-minute interval
            var timer = new System.Threading.Timer(
                callback: _ => CheckFolderForNewPDFs(folderPath, outputFolderPath, validatedFolderPath),
                state: null,
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromMinutes(1)
            );

            // Register a callback with the cancellation token to dispose of the timer when cancellation is requested
            cancellationToken.Register(() => timer.Dispose());

            // Wait for the cancellation token to be signaled
            cancellationToken.WaitHandle.WaitOne();

            // Dispose of the timer when the method exits
            timer.Dispose();
        }

        static void CheckFolderForNewPDFs(string folderPath, string outputFolderPath, string validatedFolderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("PDF folder does not exist: " + folderPath);
                return;
            }
            string[] newPdfFiles = Directory.GetFiles(folderPath, "*.pdf");
            foreach (string pdfFilePath in newPdfFiles)
            {
                OnPdfFileCreated(pdfFilePath, outputFolderPath, validatedFolderPath);
            }
        }

        static void OnPdfFileCreated(string pdfFilePath, string outputFolderPath, string validatedFolderPath)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            var parentDirectory = Directory.GetParent(baseDirectory);
            if (parentDirectory != null)
            {
                baseDirectory = parentDirectory.FullName;
            }
            string missingValuesFolderPath = Path.Combine(baseDirectory, "Missing_Values");
            // Console.write the event
            Console.WriteLine($"New PDF file detected: {pdfFilePath}");

            // Get all company names from the database
            List<string> companyNames = dataService.GetAllCompanyNames();

            // Extract text from PDF
            string invoiceText = ExtractTextFromPDF(pdfFilePath);

            // Check the company name
            string companyName = CheckCompany(invoiceText, companyNames);
            Console.WriteLine("Company Name: " + companyName);

            //Get supplierID
            int supplierID = dataService.GetEmpresaID(companyName);
            Console.WriteLine("Supplier ID: " + supplierID);
            if (supplierID == 0)
            {
                log.Error("Supplier not found for file: " + pdfFilePath);
                Console.WriteLine("Supplier not found");
                OnValuesMissing(pdfFilePath);
                return;
            }
            log.Information("Reading invoice : " + pdfFilePath);
            // return if company name is N/A
            if (companyName == "N/A")
            {
                log.Error("Company not found for file: " + pdfFilePath);
                Console.WriteLine("Company not found");
                OnValuesMissing(pdfFilePath);
                return;
            }

            // Get the regex for the company
            List<string> regex = dataService.GetAllRegex(companyName);
            Console.WriteLine("Company Name: " + companyName);

            //create a condition that based on the company name it will call the correct method;
            string numEncomenda = "N/A";
            string numFatura = "N/A";
            decimal totalSemIVA = 0;
            decimal totalPrice = 0;
            string invoiceDate = "N/A";
            string dueDate = "N/A";
            string IVA = "";

            List<Product> products = new List<Product>();

            List<Product> productsDistinct = new List<Product>();
            switch (companyName)
            {
                case "Roger & Gallet":
                    invoiceDate = invoiceParser.ExtractInvoiceDate(invoiceText, regex[3]);
                    numEncomenda = invoiceParser.ExtractOrderNumber(invoiceText, regex[4]);
                    numFatura = invoiceParser.ExtractInvoiceNumber(invoiceText, regex[5]);
                    dueDate = invoiceParser.ExtractDueDate(invoiceText, regex[6]);
                    totalSemIVA = invoiceParser.ExtractTotalWithoutVAT(invoiceText, regex[7]);
                    totalPrice = invoiceParser.ExtractTotalPrice(invoiceText, regex[11]);
                    IVA = invoiceParser.ExtractIvaPercentage(invoiceText, regex[13]);
                    products = invoiceParser.ExtractProductDetails(invoiceText, regex[12]);
                    break;
                case "LABORATORIOS EXPANSCIENCE":
                    invoiceDate = invoiceParser.ExtractInvoiceDate(invoiceText, regex[3]);
                    numEncomenda = invoiceParser.ExtractOrderNumber(invoiceText, regex[4]);
                    numFatura = invoiceParser.ExtractInvoiceNumber(invoiceText, regex[5]);
                    dueDate = invoiceParser.ExtractDueDate(invoiceText, regex[6]);
                    totalSemIVA = invoiceParser.ExtractTotalWithoutVAT(invoiceText, regex[7]);
                    totalPrice = invoiceParser.ExtractTotalPrice(invoiceText, regex[11]);
                    IVA = invoiceParser.ExtractIvaPercentage(invoiceText, regex[13]);
                    products = invoiceParser.ExtractProductDetailsLEX(invoiceText, regex[12]);
                    break;
                case "N/A":
                    Console.WriteLine("Company not found");
                    OnValuesMissing(pdfFilePath);
                    break;
            }

            // Generate output file path
            string outputFileName = Path.GetFileNameWithoutExtension(pdfFilePath) + "_data.txt";
            string outputFilePath = Path.Combine(outputFolderPath, outputFileName);

            // Write extracted data to the output file
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine(invoiceText); // Write the entire text to the file for debugging purposes (will be deleted later)

                // Write general information
                writer.WriteLine("-----    General Information    ------");
                writer.WriteLine("Data da Fatura: " + invoiceDate);
                writer.WriteLine("Nº Encomenda: " + numEncomenda);
                writer.WriteLine("Nº Fatura: " + numFatura);
                writer.WriteLine("Data Vencimento: " + dueDate);
                writer.WriteLine("Total sem IVA: " + totalSemIVA);
                writer.WriteLine("Total com IVA: " + totalPrice);
                writer.WriteLine("Taxa IVA: " + IVA);
                writer.WriteLine("-----    Products    ------");

                // Write product details
                foreach (var product in products.Distinct())
                {
                    productsDistinct.Add(product);
                    writer.WriteLine(product);
                }
            }

            // Grab the orderID through the numFatura
            int orderID = dataService.GetOrderID(numFatura);
            Console.WriteLine("Order ID: " + orderID);
            if (orderID == 0)
            {
                //log.Error("Order not found for invoiceNumber: " + numFatura + " in the invoice: " + pdfFilePath);
                //Console.WriteLine("Order not found");
                //OnValuesMissing(pdfFilePath);
                //return;
            }

            // Check if any field is missing
            //dictionary to store the fields and their values
            var fields = new Dictionary<string, object>{
                {"Invoice Date", invoiceDate},
                {"Num Encomenda", numEncomenda},
                {"Num Fatura", numFatura},
                {"Due Date", dueDate},
                {"Total Sem IVA", totalSemIVA},
                {"Total Price", totalPrice},
                {"IVA Percentage", IVA},
                };

            // boolean to indicate if any field is missing
            bool isAnyFieldMissing = false;
            // counter to keep track of the number of missing fields
            int missingFieldsCounter = 0;
            foreach (var field in fields)
            {
                if (field.Value == null || field.Value.ToString() == "N/A")
                {
                    log.Error($"Missing field: {field.Key}");
                    Console.WriteLine($"Missing field: {field.Key}");
                    missingFieldsCounter++;
                    isAnyFieldMissing = true;
                }
            }
            // Special check for products being null or having a count of 0
            if (products == null || products.Count == 0)
            {
                Console.WriteLine("Missing field: Products");
                missingFieldsCounter++;
                isAnyFieldMissing = true;
            }

            if (isAnyFieldMissing)
            {
                log.Error("Number of missing fields: " + missingFieldsCounter);
                OnValuesMissing(pdfFilePath);
                return;
            }
            else
            {
                validateProducts(orderID, productsDistinct, numFatura, pdfFilePath);

                Console.WriteLine("Data written to " + outputFilePath);
            }
            regex.Clear();
        }


        //Method to move the pdf file to the missing folder in case of missing values
        static void OnValuesMissing(string pdfFilePath)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            var parentDirectory = Directory.GetParent(baseDirectory);
            if (parentDirectory != null)
            {
                baseDirectory = parentDirectory.FullName;
            }

            string missingValuesFolderPath = Path.Combine(baseDirectory, "missing");

            string fileName = Path.GetFileName(pdfFilePath);
            Console.WriteLine("Missing values in PDF file: " + pdfFilePath);
            string destinationFilePath = Path.Combine(missingValuesFolderPath, fileName);

            Console.WriteLine("Moving PDF file to Missing_Values folder" + destinationFilePath);
            //File.Move(pdfFilePath, destinationFilePath);
            Console.WriteLine($"Moved PDF file to Missing_Values folder: {destinationFilePath}");

        }


        //Calls a method from the producer class to validate products of the invoice.
        static void validateProducts(int orderID, List<Product> products, string invoiceNumber, string pdfFilePath)
        {
            string baseDirectory = GetBaseDirectory();
            string invalidFolderPath = GetFolderPaths(baseDirectory).invalidFolerPath;
            string validFolderPath = GetFolderPaths(baseDirectory).validatedFolderPath;

            string fileName = "";
            string destinationFilePath = "";

            foreach (var product in products)
            {
                Console.WriteLine("CNP" + product.CNP);
                //price check
                //check if neither of the prices equal to 0
                if (product.NetPrice != 0 && product.UnitPrice != 0)
                {

                    bool isProductValid;
                    //check for invoices that multiply the quantity with unit price instead of using the net price per product                
                    if (product.UnitPrice < product.NetPrice)
                    {

                        isProductValid = dataService.ValidateProduct(product.CNP, orderID, product.NetPrice, product.UnitPrice, product.Quantity, invoiceNumber, product.isFactUpdated);
                        //Console.WriteLine("ValueCheck: " + product.NetPrice / product.Quantity);
                    }
                    else
                    {
                        isProductValid = dataService.ValidateProduct(product.CNP, orderID, product.NetPrice, product.UnitPrice, product.Quantity, invoiceNumber, product.isFactUpdated);
                        //Console.WriteLine("ValueCheck: " + product.NetPrice);
                    }

                    if (isProductValid)
                    {
                        //produto validado.
                        log.Information("Product validated: " + product.CNP + " in invoice: " + pdfFilePath);
                        product.isFactUpdated = 1;
                    }
                    else
                    {
                        //caso não seja possivel validar algum produto do invoice, o invoice é movido para a pasta invalid
                        //podendo ser verificado posteriormente o codigo do produto que não foi validado de qual invoice
                        log.Error("Product not validated: " + product.CNP + " in invoice: " + pdfFilePath);
                        log.Error("Invoice not validated: " + pdfFilePath);
                        fileName = Path.GetFileName(pdfFilePath);
                        destinationFilePath = Path.Combine(invalidFolderPath, fileName);
                        //File.Move(pdfFilePath, destinationFilePath);
                        Console.WriteLine($"Moved PDF file to Invalid folder: {pdfFilePath}");
                        return;
                    }
                }
                Console.WriteLine("Invoice fact updated product: " + product.isFactUpdated, product.Code);
            }

            //if function didnt return, then all products were validated, which means invoice is valid
            log.Information("Invoice validated: " + pdfFilePath);
            // Move processed PDF file to validated folder
            fileName = Path.GetFileName(pdfFilePath);
            destinationFilePath = Path.Combine(validFolderPath, fileName);
            File.Move(pdfFilePath, destinationFilePath);


        }
    }
}
