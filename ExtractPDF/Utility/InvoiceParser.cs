using System;
using System.Globalization;
using System.Text.RegularExpressions;
using PDFDataExtraction.Models;

namespace PDFDataExtraction.Utility
{
    public class InvoiceParser : IInvoiceParser
    {
        public string ExtractOrderNumber(string text, string pattern)
        {
            return ExtractSingleMatch(text, new Regex(pattern, RegexOptions.IgnoreCase));
        }

        public string ExtractInvoiceNumber(string text, string pattern)
        {
            return ExtractSingleMatch(text, new Regex(pattern, RegexOptions.IgnoreCase));
        }

        public decimal ExtractTotalWithoutVAT(string text, string pattern)
        {
            return ExtractDecimalValueWithNormalization(text, pattern);
        }

        public decimal ExtractTotalPrice(string text, string pattern)
        {
            return ExtractDecimalValueWithNormalization(text, pattern);
        }

        public string ExtractInvoiceDate(string text, string pattern)
        {
            return ExtractDateString(text, new Regex(pattern, RegexOptions.IgnoreCase));
        }

        public string ExtractDueDate(string text, string pattern)
        {
            // Adjust this method based on how you determine the correct due date among multiple dates
            return ExtractFurthestFutureDateMatch(text, new Regex(pattern, RegexOptions.IgnoreCase));
        }


        public string ExtractIvaPercentage(string text, string pattern)
        {
            return ExtractAllMatchesFormatted(text, new Regex(pattern, RegexOptions.IgnoreCase));
        }


        private string ExtractAllMatchesFormatted(string text, Regex regex)
        {
            MatchCollection matches = regex.Matches(text);
            List<string> formattedMatches = new List<string>();
            var distinctMatches = matches.GroupBy(match => match.ToString())
                                         .Select(group => group.First())
                                         .ToList();
            foreach (Match match in distinctMatches)
            {
                // Assuming your regex has two groups: one for the percentage and one for the amount
                // Adjust the group indexes as per your actual regex groups
                string percentage = match.Groups[1].Value.Trim();
                string amount = match.Groups[2].Value.Trim();
                // Format each match as desired, here assuming "percentage amount"
                // Adjust the formatting to match your needs
                formattedMatches.Add($"{percentage} {amount}");
            }

            // Join all formatted matches with a newline or another separator
            // This will ensure each IVA percentage and amount pair is on a new line
            return string.Join(Environment.NewLine, formattedMatches);
        }


        private string ExtractSingleMatch(string text, Regex regex)
        {
            var match = regex.Match(text);
            if (match.Success)
            {
                return match.Value.Trim();
            }
            throw new ParseException($"No match found with pattern: {regex.ToString()}");
        }
        private string ExtractSingleMatchNumeric(string text, Regex regex)
        {
            var match = regex.Match(text);
            if (match.Success)
            {
                var numericValue = Regex.Match(match.Value, @"\d+").Value;
                return numericValue.Trim();
            }
            throw new ParseException($"No match found with pattern: {regex.ToString()}");
        }
        private string ExtractDateString(string text, Regex regex)
        {
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (DateTime.TryParse(match.Value, out DateTime date))
                {
                    // Return the date formatted as "dd/MM/yyyy"
                    return date.ToString("dd/MM/yyyy");
                }
            }
            // Return "N/A" if no valid date is found
            return "N/A";
        }



        private string ExtractFurthestFutureDateMatch(string text, Regex regex)
        {
            MatchCollection matches = regex.Matches(text);

            // Start with the earliest possible date
            DateTime furthestFutureDate = DateTime.MinValue;

            foreach (Match match in matches)
            {
                if (DateTime.TryParse(match.Value, out DateTime parsedDate))
                {
                    // Check if the parsed date is further in the future than the current furthestFutureDate
                    if (parsedDate > furthestFutureDate)
                    {
                        // Update furthestFutureDate
                        furthestFutureDate = parsedDate;
                    }
                }
            }

            // Check if a date was found (furthestFutureDate is no longer DateTime.MinValue)
            if (furthestFutureDate > DateTime.MinValue)
            {
                // Return the furthest future date found, formatted as "dd/MM/yyyy"
                return furthestFutureDate.ToString("dd/MM/yyyy");
            }
            else
            {
                // Return "N/A" if no future date was found
                return "N/A";
            }
        }

        private static decimal ExtractDecimalValueWithNormalization(string text, string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(text);
            if (match.Success)
            {
                string decimalString = match.Groups[1].Value;

                // Normalize the string based on the last separator
                decimalString = NormalizeDecimalString(decimalString);

                // Attempt to parse the normalized string to decimal
                if (decimal.TryParse(decimalString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
                {
                    return decimalValue;
                }
                else
                {
                    Console.WriteLine($"Failed to parse '{decimalString}' as a decimal number.");
                }
            }
            // Return 0 or another appropriate default value if parsing fails
            return 0m;
        }

        private static string NormalizeDecimalString(string decimalString)
        {
            // Identify the last comma or period in the string to determine the decimal separator
            int lastCommaIndex = decimalString.LastIndexOf(',');
            int lastPeriodIndex = decimalString.LastIndexOf('.');

            if (lastCommaIndex > lastPeriodIndex)
            {
                // Comma is the decimal separator, remove all other commas
                decimalString = decimalString.Substring(0, lastCommaIndex).Replace(",", "") + "." + decimalString.Substring(lastCommaIndex + 1).Replace(",", "");
            }
            else if (lastPeriodIndex > lastCommaIndex)
            {
                // Period is the decimal separator, remove all commas
                decimalString = decimalString.Substring(0, lastPeriodIndex).Replace(",", "") + "." + decimalString.Substring(lastPeriodIndex + 1).Replace(".", "");
            }

            return decimalString;
        }

        // Method to extract product details using regular expression
        public List<Product> ExtractProductDetails(string invoiceText, string pattern)
        {
            //Console.WriteLine(pattern);
            List<Product> products = new List<Product>();

            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            HashSet<string> uniqueProductIdentifiers = new HashSet<string>();


            foreach (Match match in matches.Distinct())
            {
                string uniqueIdentifier = $"{match.Groups["Description"].Value}_{match.Groups["Quantity"].Value}_{match.Groups["PrecoSemIVA"].Value}";

                // Check if this product has already been processed
                if (!uniqueProductIdentifiers.Contains(uniqueIdentifier))
                {
                    uniqueProductIdentifiers.Add(uniqueIdentifier);

                    Product product = new Product();
                    product.Code = match.Groups[1].Value;

                    product.Barcode = match.Groups["Barcode"].Value;
                    product.Description = match.Groups["Description"].Value;
                    int quantity;
                    if (int.TryParse(match.Groups["Quantity"].Value, out quantity))
                    {
                        product.Quantity = quantity;
                    }
                    int bonus;
                    if (match.Groups["Bonus"].Success && int.TryParse(match.Groups["Bonus"].Value, out bonus))
                    {
                        product.Bonus = bonus;
                    }
                    else
                    {
                        product.Bonus = 0;
                    }
                    decimal precoComIVA;
                    if (decimal.TryParse(match.Groups["PrecoComIVA"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out precoComIVA))
                    {
                        product.NetPrice = precoComIVA;
                    }

                    decimal grossPrice;
                    if (decimal.TryParse(match.Groups["GrossPrice"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out grossPrice))
                    {
                        product.UnitPrice = grossPrice;
                    }
                    decimal discount;
                    if (decimal.TryParse(match.Groups["Discount"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out discount))
                    {
                        product.Discount1 = discount;
                        product.Discount2 = discount;
                        product.Discount3 = discount;
                    }
                    decimal precoSemIVA;
                    if (decimal.TryParse(match.Groups["PrecoSemIVA"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out precoSemIVA))
                    {
                        product.PrecoComIVA = precoSemIVA;
                    }

                    decimal iva;
                    if (decimal.TryParse(match.Groups["IVA"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out iva))
                    {
                        product.IVA = iva;
                    };

                    product.CNP = match.Groups["CNP"].Value;

                    products.Add(product);
                }
            }
            return products;
        }

        // Method to extract products from LEX invoices using regular expression
        public List<Product> ExtractProductDetailsLEX(string invoiceText, string pattern)
        {
            List<Product> products = new List<Product>();
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            HashSet<string> uniqueProductIdentifiers = new HashSet<string>();

            foreach (Match match in matches)
            {
                string uniqueIdentifier = $"{match.Groups[1].Value}_{match.Groups[5].Value}_{match.Groups[8].Value}";

                // Check if this product has already been processed
                if (!uniqueProductIdentifiers.Contains(uniqueIdentifier))
                {
                    uniqueProductIdentifiers.Add(uniqueIdentifier);

                    LEXProduct product = new LEXProduct();

                    // Assuming first capturing group is the product code
                    product.Code = match.Groups[1].Value;

                    // Second capturing group for product name
                    product.Name = match.Groups[2].Value;

                    // Third capturing group for CNP (numeric identifier)
                    product.CNP = match.Groups[3].Value;

                    // Fourth capturing group for Lot Number (optional)
                    product.LotNumber = match.Groups[4].Value;

                    // Fifth capturing group for Quantity, extract numeric part through regex matching
                    string rgxStr = @"\d+";

                    var intMatch = Regex.Match(match.Groups[5].Value, rgxStr);

                    if (intMatch.Success)
                    {
                        product.Quantity = int.Parse(intMatch.Value);
                    }


                    // Following groups for prices and percentages
                    if (decimal.TryParse(match.Groups[6].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal unitPrice))
                        product.UnitPrice = unitPrice;

                    if (decimal.TryParse(match.Groups[7].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discountPercentage))
                        product.DiscountPercentage = discountPercentage;

                    if (decimal.TryParse(match.Groups[8].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal netPrice))
                        product.NetPrice = netPrice;

                    // Last group for IVA
                    if (int.TryParse(match.Groups[9].Value, out int iva))
                        product.IVA = iva;

                    products.Add(product);
                }
            }

            return products;
        }
        public List<Product> ExtractProductDetailsMercafar(string invoiceText, string pattern)
        {
            List<Product> products = new List<Product>();
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            HashSet<string> uniqueProductIdentifiers = new HashSet<string>();

            foreach (Match match in matches)
            {
                // The code is directly captured from the pattern
                string code = match.Groups["Codigo"].Value;
                string uniqueIdentifier = $"{code}_{match.Groups["Ped"].Value}_{match.Groups["PUNIT"].Value}_{match.Groups["PVP"].Value}_{match.Groups["Desc"].Value}_{match.Groups["PVF"].Value}_{match.Groups["VALOR"].Value}_{match.Groups["IVA"].Value}";

                if (!uniqueProductIdentifiers.Contains(uniqueIdentifier))
                {
                    uniqueProductIdentifiers.Add(uniqueIdentifier);
                    MERCAFARProduct product = new MERCAFARProduct();

                    product.Code = match.Groups["Codigo"].Value;
                    product.Description = match.Groups["Designacao"].Value.Trim();
                    product.CNP = code;

                    string quantityString = Regex.Match(match.Groups["Env"].Value, @"\d+").Value;
                    product.Quantity = int.TryParse(quantityString, out int qty) ? qty : 0;

                    string quantityAskedString = Regex.Match(match.Groups["Ped"].Value, @"\d+").Value;
                    product.QtPed = int.TryParse(quantityAskedString, out int QtPed) ? QtPed : 0;

                    product.UnitPrice = decimal.TryParse(match.Groups["PVP"].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal unitPrice) ? unitPrice : 0;
                    product.Discount1 = decimal.TryParse(match.Groups["Desc"].Value.Replace("%", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discount) ? discount : 0;

                    product.PrecoComIVA = decimal.TryParse(match.Groups["PVF"].Value?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pvf) ? pvf
                        : decimal.TryParse(match.Groups["PVF1"].Value?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out pvf) ? pvf : 0;

                    product.NetPrice = decimal.TryParse(match.Groups["VALOR"].Value?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) ? valor
                        : decimal.TryParse(match.Groups["VALOR1"].Value?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out valor) ? valor : 0;

                    product.IVA = decimal.TryParse(match.Groups["IVA"].Value?.Replace("%", ""), out decimal iva) ? iva : 0;

                    products.Add(product);
                }
            }

            return products;
        }

        public List<Product> ExtractProductDetailsOCP(string invoiceText, string pattern)
        {
            List<Product> products = new List<Product>();
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            HashSet<string> uniqueProductIdentifiers = new HashSet<string>();

            foreach (Match match in matches)
            {
                // Using named groups for better clarity and maintainability
                string uniqueIdentifier = $"{match.Groups["codigo"].Value}_{match.Groups["designacao"].Value}_{match.Groups["qtped"].Value}_{match.Groups["qtavi"].Value}_{match.Groups["pvp"].Value}_{match.Groups["pvf"].Value}_{match.Groups["iva"].Value}_{match.Groups["sit"].Value}_{match.Groups["caixa"].Value}";

                if (!uniqueProductIdentifiers.Contains(uniqueIdentifier))
                {
                    uniqueProductIdentifiers.Add(uniqueIdentifier);
                    OCPProduct product = new OCPProduct();

                    product.Code = match.Groups["codigo"].Value;
                    product.Name = match.Groups["designacao"].Value;
                    product.CNP = match.Groups["codigo"].Value;
                    //product.LotNumber = match.Groups["Lote"].Value;

                    // Ensure quantity parsing is correct
                    string quantityString = Regex.Match(match.Groups["qtped"].Value, @"\d+").Value;
                    product.Quantity = int.TryParse(quantityString, out int qty) ? qty : 0;

                    // Parsing using invariant culture
                    product.UnitPrice = decimal.TryParse(match.Groups["pvf"].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal unitPrice) ? unitPrice : 0;
                    //product.DiscountPercentage = decimal.TryParse(match.Groups["Esc"].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discount) ? discount : 0;
                    product.NetPrice = decimal.TryParse(match.Groups["total"].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal netPrice) ? netPrice : 0;

                    // IVA parsing
                    if (int.TryParse(match.Groups["iva"].Value, out int iva))
                        product.IVA = iva;

                    products.Add(product);
                }
            }

            return products;
        }
    }


    /// <summary>
    /// Custom exception class for parsing errors
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}