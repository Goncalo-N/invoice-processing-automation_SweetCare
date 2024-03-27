using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PDFDataExtraction
{
    public class InvoiceParser
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
        DateTime furthestFutureDate = DateTime.MinValue; // Start with the earliest possible date

        foreach (Match match in matches)
        {
            if (DateTime.TryParse(match.Value, out DateTime parsedDate))
            {
                // Check if the parsed date is further in the future than the current furthestFutureDate
                if (parsedDate > furthestFutureDate)
                {
                    furthestFutureDate = parsedDate; // Update furthestFutureDate
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

        private decimal ExtractDecimalValueWithNormalization(string text, string pattern)
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

        private string NormalizeDecimalString(string decimalString)
        {
            if (decimalString.Contains(",") && decimalString.Contains("."))
            {
                // Ambiguous case: decide based on the convention
                int lastCommaIndex = decimalString.LastIndexOf(',');
                int lastDotIndex = decimalString.LastIndexOf('.');
                if (lastCommaIndex > lastDotIndex)
                {
                    // Assume comma is the decimal separator
                    decimalString = decimalString.Replace(".", "").Replace(",", ".");
                }
                else
                {
                    // Assume dot is the decimal separator
                    decimalString = decimalString.Replace(",", "");
                }
            }
            else if (decimalString.Contains(",") && !decimalString.Contains("."))
            {
                // Likely using commas as decimal separators
                decimalString = decimalString.Replace(",", ".");
            }
            // No need to modify decimalString if it only contains dots or no separators at all

            return decimalString;
        }

        // Method to extract product details using regular expression
        public List<IProduct> ExtractProductDetails(string invoiceText, string pattern)
        {
            //Console.WriteLine(pattern);
            List<IProduct> products = new List<IProduct>();

            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            HashSet<string> uniqueProductIdentifiers = new HashSet<string>();


            foreach (Match match in matches.Distinct())
            {
                string uniqueIdentifier = $"{match.Groups["Description"].Value}_{match.Groups["Quantity"].Value}_{match.Groups["PrecoSemIVA"].Value}";

                // Check if this product has already been processed
                if (!uniqueProductIdentifiers.Contains(uniqueIdentifier))
                {
                    uniqueProductIdentifiers.Add(uniqueIdentifier);

                    IProduct product = new IProduct();
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
                        product.NetPrice = precoSemIVA;
                    }
                    decimal precoComIVA;
                    if (decimal.TryParse(match.Groups["PrecoComIVA"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out precoComIVA))
                    {
                        product.PrecoComIVA = precoComIVA;
                    }
                    products.Add(product);
                }
            }
            return products;
        }

        // Method to extract products from Moreno II invoices using regular expression
        public List<IProduct> ExtractProductDetailsMoreno(string invoiceText, string pattern)
        {
            //Console.WriteLine(pattern);
            List<IProduct> products = new List<IProduct>();
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            HashSet<string> uniqueProductIdentifiers = new HashSet<string>();

            foreach (Match match in matches)
            {
                string uniqueIdentifier = $"{match.Groups["Designation"].Value}_{match.Groups["Quantity"].Value}_{match.Groups["NetPrice"].Value}";

                // Check if this product has already been processed
                if (!uniqueProductIdentifiers.Contains(uniqueIdentifier))
                {
                    uniqueProductIdentifiers.Add(uniqueIdentifier);

                    MorenoProduct product = new MorenoProduct();
                    product.CNP = match.Groups["CNP"].Value;
                    product.Designation = match.Groups["Designation"].Value;
                    product.Lot = match.Groups["Lot"].Value;
                    DateTime expiryDate;
                    if (DateTime.TryParse(match.Groups["ExpiryDate"].Value, out expiryDate))
                    {
                        product.ExpiryDate = expiryDate;
                    }
                    product.Type = match.Groups["Type"].Value;
                    int quantity;
                    if (int.TryParse(match.Groups["Quantity"].Value, out quantity))
                    {
                        product.Quantity = quantity;
                    }
                    decimal unitPrice;
                    if (decimal.TryParse(match.Groups["UnitPrice"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out unitPrice))
                    {
                        product.UnitPrice = unitPrice;
                    }
                    decimal discount1;
                    if (decimal.TryParse(match.Groups["Discount1"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out discount1))
                    {
                        product.Discount1 = discount1;
                    }
                    decimal discount2;
                    if (decimal.TryParse(match.Groups["Discount2"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out discount2))
                    {
                        product.Discount2 = discount2;
                    }
                    decimal netPrice;
                    if (decimal.TryParse(match.Groups["NetPrice"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out netPrice))
                    {
                        product.NetPrice = netPrice;
                    }
                    int iva;
                    if (int.TryParse(match.Groups["IVA"].Value, out iva))
                    {
                        product.IVA = iva;
                    }
                    decimal total;
                    if (decimal.TryParse(match.Groups["Total"].Value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out total))
                    {
                        product.Total = total;
                    }
                    //Console.WriteLine(product);
                    products.Add(product);
                }
            }
            return products;
        }

        // Method to extract products from LEX invoices using regular expression
        public List<IProduct> ExtractProductDetailsLEX(string invoiceText, string pattern)
        {
            List<IProduct> products = new List<IProduct>();
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
    }

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
