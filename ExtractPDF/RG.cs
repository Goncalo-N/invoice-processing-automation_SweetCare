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
            //Console.WriteLine(pattern);
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string totalPriceStr = match.Groups[1].Value.Replace(",", ".");
                return decimal.Parse(totalPriceStr, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        // Method to extract total price using regular expression
        internal static decimal ExtractTotalPrice(string text, string pattern)
        {
            decimal maxTotalPrice = 0;
            MatchCollection matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                // Extract the matched price string.
                string totalPriceStr = match.Groups[1].Value;

                // Ensure proper decimal parsing for numbers like "1,158.06".
                // Convert the extracted string into a format that can be parsed by decimal.Parse.
                totalPriceStr = totalPriceStr.Replace(",", string.Empty);

                // Parse the number using InvariantCulture to handle the dot as a decimal separator.
                if (decimal.TryParse(totalPriceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal totalPrice))
                {
                    if (totalPrice > maxTotalPrice)
                    {
                        maxTotalPrice = totalPrice;
                    }
                }
            }
            return maxTotalPrice;
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

            foreach(Match match in distinctMatches)
            {
                if (match.Success)
                {
                    finalIVA = finalIVA + match.Value+"\n";
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
            foreach (Match match in matches)
            {
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
            return products;
        }
        // Method to extract products from Moreno II invoices using regular expression
        internal static List<IProduct> ExtractProductDetailsMoreno(string invoiceText, string pattern)
        {
            //Console.WriteLine(pattern);
            List<IProduct> products = new List<IProduct>();
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
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
                product.Quantity = match.Groups["Quantity"].Value;
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
            return products;
        }
        // Method to extract products from LEX invoices using regular expression
        public static List<IProduct> ExtractProductDetailsLEX(string invoiceText, string pattern)
        {
            List<IProduct> products = new List<IProduct>();
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                LEXProduct product = new LEXProduct();

                // Assuming first capturing group is the product code
                product.Code = match.Groups[1].Value;

                // Second capturing group for product name
                product.Name = match.Groups[2].Value;

                // Third capturing group for CNP (numeric identifier)
                product.CNP = match.Groups[3].Value;

                // Fourth capturing group for Lot Number (optional)
                product.LotNumber = match.Groups[4].Value;

                // Fifth capturing group for Quantity, extract numeric part
                product.Quantity = match.Groups[5].Value;

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

            return products;
        }
    }
}