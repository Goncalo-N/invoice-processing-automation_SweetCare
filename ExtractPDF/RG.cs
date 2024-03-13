using System.Globalization;
using System.Text.RegularExpressions;
namespace PDFDataExtraction
{
    public class RG
    {

        internal static string ExtractNumEncomenda(string text, string pattern)
        {
            ////Console.WriteLine(pattern);
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }
            return "N/A";
        }
        internal static string ExtractNumFatura(string text, string pattern)
        {
            ////Console.WriteLine(pattern);
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }
            return "N/A";
        }
        // Method to extract total price sem IVA using regular expression
        internal static decimal ExtractTotalSemIVA(string text, string pattern)
        {
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string totalPriceStr = match.Groups[1].Value;

                // Normalize the string based on the last separator
                if (totalPriceStr.Contains(",") && totalPriceStr.Contains("."))
                {
                    // Ambiguous case: decide based on the convention
                    int lastCommaIndex = totalPriceStr.LastIndexOf(',');
                    int lastDotIndex = totalPriceStr.LastIndexOf('.');
                    if (lastCommaIndex > lastDotIndex)
                    {
                        // Assume comma is the decimal separator
                        totalPriceStr = totalPriceStr.Replace(".", "").Replace(",", ".");
                    }
                    else
                    {
                        // Assume dot is the decimal separator
                        totalPriceStr = totalPriceStr.Replace(",", "");
                    }
                }
                else if (totalPriceStr.Contains(",") && !totalPriceStr.Contains("."))
                {
                    // Likely using commas as decimal separators
                    totalPriceStr = totalPriceStr.Replace(",", ".");
                }
                // No need to modify totalPriceStr if it only contains dots or no separators at all

                // Parse the modified string to decimal
                if (decimal.TryParse(totalPriceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal totalPrice))
                {
                    return totalPrice;
                }
                else
                {
                    Console.WriteLine($"Failed to parse '{totalPriceStr}' as a decimal number.");
                }
            }
            return 0;
        }


        // Method to extract total price using regular expression
        internal static decimal ExtractTotalPrice(string text, string pattern)
        {
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string totalPriceStr = match.Groups[1].Value;

                // Normalize the string based on the last separator
                if (totalPriceStr.Contains(",") && totalPriceStr.Contains("."))
                {
                    // Ambiguous case: decide based on the convention
                    int lastCommaIndex = totalPriceStr.LastIndexOf(',');
                    int lastDotIndex = totalPriceStr.LastIndexOf('.');
                    if (lastCommaIndex > lastDotIndex)
                    {
                        // Assume comma is the decimal separator
                        totalPriceStr = totalPriceStr.Replace(".", "").Replace(",", ".");
                    }
                    else
                    {
                        // Assume dot is the decimal separator
                        totalPriceStr = totalPriceStr.Replace(",", "");
                    }
                }
                else if (totalPriceStr.Contains(",") && !totalPriceStr.Contains("."))
                {
                    // Likely using commas as decimal separators
                    totalPriceStr = totalPriceStr.Replace(",", ".");
                }
                // No need to modify totalPriceStr if it only contains dots or no separators at all

                // Parse the modified string to decimal
                if (decimal.TryParse(totalPriceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal totalPrice))
                {
                    return totalPrice;
                }
                else
                {
                    Console.WriteLine($"Failed to parse '{totalPriceStr}' as a decimal number.");
                }
            }
            return 0;
        }

        // Method to extract invoice date using regular expression
        internal static string ExtractInvoiceDate(string text, string pattern)
        {
            Console.WriteLine(pattern);
            MatchCollection matches = Regex.Matches(text, pattern);
            foreach (Match match in matches)
            {
                if (DateTime.TryParse(match.Value, out DateTime date))
                {
                    return date.ToString("dd/MM/yyyy");
                }
            }
            return "N/Aa";
        }

        // Method to extract due date using regular expression
        internal static string ExtractDueDate(string text, string pattern)
        {
            //Console.WriteLine(pattern);
            DateTime latestDueDate = DateTime.MinValue;
            MatchCollection matches = Regex.Matches(text, pattern);
            foreach (Match match in matches)
            {
                string dateString = match.Value;
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    if (date > latestDueDate)
                    {
                        latestDueDate = date;
                    }
                }
            }
            if (latestDueDate != DateTime.MinValue)
            {
                return latestDueDate.ToString("dd/MM/yyyy");
            }
            else
            {
                return "N/A";
            }
        }

        // Method to extract IVA percentage using regular expression
        internal static string ExtractIVAPercentage(string text, string pattern)
        {
            //Console.WriteLine(pattern);

            MatchCollection matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
            string finalIVA = "";
            var distinctMatches = matches.GroupBy(match => match.ToString()) // Use the property(s) that define uniqueness
                             .Select(group => group.First())
                             .ToList();

            foreach (Match match in distinctMatches)
            {
                if (match.Success)
                {
                    finalIVA = finalIVA + match.Value + "\n";
                }
            }
            return finalIVA;
        }

        // Method to extract product details using regular expression
        internal static List<IProduct> ExtractProductDetails(string invoiceText, string pattern)
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
                    product.Article = match.Groups["Article"].Value;
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
                        product.GrossPrice = grossPrice;
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
                        product.PrecoSemIVA = precoSemIVA;
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
        internal static List<IProduct> ExtractProductDetailsMoreno(string invoiceText, string pattern)
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
                    Console.WriteLine(product);
                    products.Add(product);
                }
            }
            return products;
        }
        // Method to extract products from LEX invoices using regular expression
        public static List<IProduct> ExtractProductDetailsLEX(string invoiceText, string pattern)
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
}