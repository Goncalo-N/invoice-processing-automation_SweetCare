using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PDFDataExtraction.Models;
using PDFDataExtraction.Service;
using PDFDataExtraction.Utility;
using Serilog;

using static PDFDataExtraction.Utility.RegexParser;

namespace PDFDataExtraction.Core
{

    public class Program
    {
        static readonly DataService dataService;
        static readonly InvoiceParser invoiceParser = new InvoiceParser();

        static readonly Serilog.Core.Logger log;

        public static List<SupplierPattern> LoadSupplierPatterns()
        {
            var jsonContent = File.ReadAllText(regexPatternsFile);
            var supplierPatterns = JsonConvert.DeserializeObject<List<SupplierPattern>>(jsonContent);

            if (supplierPatterns != null)
                return supplierPatterns;

            throw new InvalidOperationException("Supplier patterns not loaded.");
        }


        // Static fields for folder paths
        static readonly string pdfFolder;
        static readonly string outputFolder;
        static readonly string validFolder;
        static readonly string invalidFolder;
        static readonly string missingValuesFolder;

        static readonly string regexPatternsFile;

        // Static constructor to initialize static readonly fields
        static Program()
        {
            Microsoft.Extensions.Configuration.IConfiguration configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Initialize logger
            log = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();


            // Initialize dataService with the connection string from configuration
            string connectionString = configuration.GetSection("DatabaseConfiguration:ConnectionString").Value
               ?? throw new InvalidOperationException("Connection string not configured.");

            dataService = new DataService(connectionString);

            // Initialize folder paths from configuration
            pdfFolder = configuration["ApplicationPaths:PdfFolder"]
                ?? throw new InvalidOperationException("PDF folder path not configured.");

            outputFolder = configuration["ApplicationPaths:OutputFolder"]
                ?? throw new InvalidOperationException("Output folder path not configured.");

            validFolder = configuration["ApplicationPaths:ValidFolder"]
                ?? throw new InvalidOperationException("Valid folder path not configured.");

            invalidFolder = configuration["ApplicationPaths:InvalidFolder"]
                ?? throw new InvalidOperationException("Invalid folder path not configured.");

            missingValuesFolder = configuration["ApplicationPaths:MissingValuesFolder"]
                ?? throw new InvalidOperationException("Missing values folder path not configured.");

            regexPatternsFile = configuration["ApplicationPaths:RegexPatternsFile"]
                ?? throw new InvalidOperationException("Regex patterns folder path not configured.");
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

            log.Information("Application Starting");



            Task.Run(() => MonitorPdfFolder());
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

        static void PreventApplicationExit()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Method to extract text from PDF using iText
        /// </summary>
        /// <param name="filePath">path of the pdf file</param>
        /// <returns>text output of the pdf</returns>
        static string ExtractTextFromPDF(string filePath)
        {
            using PdfReader reader = new PdfReader(filePath);
            using PdfDocument pdfDoc = new PdfDocument(reader);
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
        /// <summary>
        /// Method to check the file to see what company it belongs to
        /// </summary>
        /// <param name="text">text converted from pdf</param>
        /// <returns></returns>
        public static string CheckCompany(string text, List<SupplierPattern> supplierPatterns)
        {
            foreach (var pattern in supplierPatterns)
            {

                if (Regex.IsMatch(text, pattern.PadraoRegexNomeFornecedor, RegexOptions.IgnoreCase))
                {
                    return pattern.NomeEmpresa;
                }
            }
            return "N/A";
        }


        /// <summary>
        /// Method to monitor the PDF folder for new PDF files
        /// </summary>
        public static void MonitorPdfFolder()
        {
            // Create a timer with a 5-minute interval
            System.Timers.Timer timer = new System.Timers.Timer(5 * 60 * 1000);
            timer.Elapsed += (sender, e) => CheckFolderForNewPDFs();
            timer.AutoReset = true;
            timer.Start();

            // Initial check when starting the program
            CheckFolderForNewPDFs();
        }

        /// <summary>
        /// Method to monitor the PDF folder for new PDF files with a cancelation token
        /// </summary>
        /// <param name="cancellationToken">session token for pausing task</param>
        public static void MonitorPdfFolder(CancellationToken cancellationToken)
        {
            // Create a timer with a 5-minute interval
            var timer = new System.Threading.Timer(
                callback: _ => CheckFolderForNewPDFs(),
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

        /// <summary>
        /// Method to check the folder for new PDF files
        /// </summary>
        static void CheckFolderForNewPDFs()
        {
            if (!Directory.Exists(pdfFolder))
            {
                Console.WriteLine("PDF folder does not exist: " + pdfFolder);
                log.Error("PDF folder does not exist: " + pdfFolder);
                return;
            }
            string[] newPdfFiles = Directory.GetFiles(pdfFolder, "*.pdf");
            foreach (string pdfFilePath in newPdfFiles)
            {
                OnPdfFileCreated(pdfFilePath);
            }
        }

        /// <summary>
        /// Method that is called when a new PDF file is added to the folder
        /// </summary>
        /// <param name="pdfFilePath">path where pdf is located</param>
        static void OnPdfFileCreated(string pdfFilePath)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            var parentDirectory = Directory.GetParent(baseDirectory);
            if (parentDirectory != null)
            {
                baseDirectory = parentDirectory.FullName;
            }

            // Console.write the event
            Console.WriteLine($"New PDF file detected: {pdfFilePath}");
            log.Information($"New PDF file detected: {pdfFilePath}");
            // Extract text from PDF
            string invoiceText = ExtractTextFromPDF(pdfFilePath);

            // Check the company name
            var supplierPatterns = LoadSupplierPatterns();
            //PrintSupplierPatterns(supplierPatterns);

            var companyName = CheckCompany(invoiceText, supplierPatterns);

            Console.WriteLine("Company Name: " + companyName);
            if (companyName == null || companyName == "N/A")
            {
                log.Error("Supplier not found for file: " + pdfFilePath);
                Console.WriteLine("Supplier not found");
                OnValuesMissing(pdfFilePath);
                return;
            }

            log.Information("Company Name Detected: " + companyName);
            log.Information("Reading invoice : " + pdfFilePath);

            // Get the regex for the company
            SupplierPattern supplierPattern = supplierPatterns.FirstOrDefault(sp => sp.NomeEmpresa == companyName);

            //create a condition that based on the company name it will call the correct method;
            string numEncomenda = "N/A";
            string numFatura = "N/A";
            decimal totalSemIVA = 0;
            decimal totalPrice = 0;
            string invoiceDate = "N/A";
            string dueDate = "N/A";
            DateTime dueDateMissing = DateTime.Now;
            string IVA = "";

            List<Product> products = new List<Product>();

            List<Product> productsDistinct = new List<Product>();

            invoiceDate = invoiceParser.ExtractInvoiceDate(invoiceText, supplierPattern.PadraoRegexDataFatura);
            numEncomenda = invoiceParser.ExtractOrderNumber(invoiceText, supplierPattern.PadraoRegexNumeroEncomenda);
            numFatura = invoiceParser.ExtractInvoiceNumber(invoiceText, supplierPattern.PadraoRegexNumeroFatura);
            dueDate = invoiceParser.ExtractDueDate(invoiceText, supplierPattern.PadraoRegexDataVencimentoFatura);
            totalSemIVA = invoiceParser.ExtractTotalWithoutVAT(invoiceText, supplierPattern.PadraoRegexTotalSemIva);
            totalPrice = invoiceParser.ExtractTotalPrice(invoiceText, supplierPattern.PadraoRegexTotalAPagar);
            IVA = invoiceParser.ExtractIvaPercentage(invoiceText, supplierPattern.PadraoRegexTaxaIva);

            switch (companyName)
            {
                case "Roger & Gallet":
                    products = invoiceParser.ExtractProductDetails(invoiceText, supplierPattern.PadraoRegexProduto);
                    break;
                case "LABORATORIOS EXPANSCIENCE":
                    products = invoiceParser.ExtractProductDetailsLEX(invoiceText, supplierPattern.PadraoRegexProduto);
                    break;
                case "MERCAFAR, SA":
                    products = invoiceParser.ExtractProductDetailsMercafar(invoiceText, supplierPattern.PadraoRegexProduto);
                    break;
                case "OCP":
                    products = invoiceParser.ExtractProductDetailsOCP(invoiceText, supplierPattern.PadraoRegexProduto);
                    break;
                case "N/A":
                    Console.WriteLine("Company not found");
                    OnValuesMissing(pdfFilePath);
                    break;
            }

            // Generate output file path
            string outputFileName = Path.GetFileNameWithoutExtension(pdfFilePath) + "_data.txt";
            string outputFilePath = Path.Combine(outputFolder, outputFileName);

            // Write extracted data to the output file
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                // Write the entire text to the file for debugging purposes (will be deleted later)
                writer.WriteLine(invoiceText);

                // calculate due date based on invoice date ( in case of missing due date)
                if (dueDate == "N/A")
                {
                    dueDateMissing = DateTime.Parse(invoiceDate).AddDays(60);
                    dueDate = String.Format("{0:dd/MM/yyyy}", dueDateMissing);
                }
                // Write general information
                writer.WriteLine("-----     General Information     ------");
                writer.WriteLine("Data da Fatura: " + invoiceDate);
                writer.WriteLine("Nº Encomenda: " + numEncomenda);
                writer.WriteLine("Nº Fatura: " + numFatura);
                writer.WriteLine("Data Vencimento: " + dueDate);
                writer.WriteLine("Total sem IVA: " + totalSemIVA);
                writer.WriteLine("Total com IVA: " + totalPrice);
                writer.WriteLine("Taxa IVA: " + IVA);

                // Write product details
                writer.WriteLine("-----    Products    ------");
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
                log.Error("Order not found for invoiceNumber: " + numFatura + " in the invoice: " + pdfFilePath);
                Console.WriteLine("Order not found");
                OnValuesMissing(pdfFilePath);
                return;
            }

            // Check if any field is missing
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

            // counter for number of missing fields
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

            // check products being null or product list being empty
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
                ValidateProducts(orderID, productsDistinct, numFatura, pdfFilePath);

                Console.WriteLine("Data written to " + outputFilePath);
            }

        }

        /// <summary>
        /// Method to print the supplier patterns to the console for debugging purposes
        /// </summary>
        /// <param name="supplierPatterns"></param>
        public static void PrintSupplierPatterns(List<SupplierPattern> supplierPatterns)
        {
            foreach (var pattern in supplierPatterns)
            {
                Console.WriteLine($"Company: {pattern.NomeEmpresa}");
                Console.WriteLine($"Nome Fornecedor Regex: {pattern.PadraoRegexNomeFornecedor}");
                Console.WriteLine($"Data Fatura Regex: {pattern.PadraoRegexDataFatura}");
                Console.WriteLine($"Numero Encomenda Regex: {pattern.PadraoRegexNumeroEncomenda}");
                Console.WriteLine($"Numero Fatura Regex: {pattern.PadraoRegexNumeroFatura}");
                Console.WriteLine($"Data Vencimento Fatura Regex: {pattern.PadraoRegexDataVencimentoFatura}");
                Console.WriteLine($"Total Sem IVA Regex: {pattern.PadraoRegexTotalSemIva}");
                Console.WriteLine($"Total a Pagar Regex: {pattern.PadraoRegexTotalAPagar}\n");
            }
        }

        /// <summary>
        /// Method moves the pdf file to the missing folder
        /// </summary>
        /// <param name="pdfFilePath"></param>
        static void OnValuesMissing(string pdfFilePath)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            var parentDirectory = Directory.GetParent(baseDirectory);
            if (parentDirectory != null)
            {
                baseDirectory = parentDirectory.FullName;
            }

            string fileName = Path.GetFileName(pdfFilePath);
            log.Error("Missing values in PDF file: " + pdfFilePath);
            string destinationFilePath = Path.Combine(missingValuesFolder, fileName);

            File.Move(pdfFilePath, destinationFilePath);
            log.Error($"Moved PDF file to Missing_Values folder: {destinationFilePath}");

        }


        /// <summary>
        /// Validates products of the invoice by calling a method from the producer class.
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="products">List of products present in this pdf</param>
        /// <param name="invoiceNumber"></param>
        /// <param name="pdfFilePath"></param>
        static void ValidateProducts(int orderID, List<Product> products, string invoiceNumber, string pdfFilePath)
        {

            string fileName = "";
            string destinationFilePath = "";

            foreach (var product in products)
            {
                //price check
                //check if neither of the prices equal to 0
                if (product.NetPrice != 0 && product.UnitPrice != 0)
                {
                    bool isProductValid;

                    //check for invoices that multiply the quantity with unit price instead of using the net price per product                
                    if (product.UnitPrice < product.NetPrice)
                    {
                        isProductValid = dataService.ValidateProduct(product.CNP, orderID, product.NetPrice, product.UnitPrice, product.Quantity, invoiceNumber, product.isFactUpdated);
                    }
                    else
                    {
                        isProductValid = dataService.ValidateProduct(product.CNP, orderID, product.NetPrice, product.UnitPrice, product.Quantity, invoiceNumber, product.isFactUpdated);
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
                        destinationFilePath = Path.Combine(invalidFolder, fileName);

                        File.Move(pdfFilePath, destinationFilePath);
                        log.Error($"Moved PDF file to Invalid folder: {pdfFilePath}");

                        return;
                    }
                }
                log.Information("Invoice fact updated product: " + product.isFactUpdated, product.Code);
            }



            if (validFolder != null)
            {
                //if function didnt return, then all products were validated, invoice is moved to valid folder
                log.Information("Invoice validated successfuly: " + pdfFilePath);

                // Move processed PDF file to validated folder
                fileName = Path.GetFileName(pdfFilePath);
                destinationFilePath = Path.Combine(validFolder, fileName);

                File.Move(pdfFilePath, destinationFilePath);

                log.Information($"Moved PDF file to Valid folder: {pdfFilePath}");
            }

            log.Information("Finished Invoice Processing: " + pdfFilePath);

        }
    }
}