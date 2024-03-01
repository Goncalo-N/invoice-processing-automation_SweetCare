using System.Globalization;
using System.Text.RegularExpressions;
namespace PDFDataExtraction
{
    public class RG
    {

        internal static string ExtractNumEncomendaRG(string text, string pattern)
        {
            Console.WriteLine(pattern);
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }
            return "N/A";
        }
        internal static string ExtractNumFaturaRG(string text, string pattern)
        {
            Console.WriteLine(pattern);
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }
            return "N/A";
        }
        // Method to extract total price sem IVA using regular expression
        internal static decimal ExtractTotalSemIVARG(string text, string pattern)
        {
            Console.WriteLine(pattern);
            Match match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string totalPriceStr = match.Groups[1].Value.Replace(",", ".");
                return decimal.Parse(totalPriceStr, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        // Method to extract total price using regular expression
        internal static decimal ExtractTotalPriceRG(string text, string pattern)
        {
            Console.WriteLine(pattern);
            decimal maxTotalPrice = 0;
            MatchCollection matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                string totalPriceStr = match.Groups[1].Value.Replace(",", ".");
                decimal totalPrice = decimal.Parse(totalPriceStr, CultureInfo.InvariantCulture);
                if (totalPrice > maxTotalPrice)
                {
                    maxTotalPrice = totalPrice;
                }
            }
            return maxTotalPrice;
        }

        // Method to extract invoice date using regular expression
        internal static string ExtractInvoiceDateRG(string text, string pattern)
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
            return "N/A";
        }

        // Method to extract due date using regular expression
        internal static string ExtractDueDateRG(string text, string pattern)
        {
            Console.WriteLine(pattern);
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
        internal static decimal ExtractIVAPercentageRG(string text, string pattern)
        {
            Console.WriteLine(pattern);
            Match match = Regex.Match(text, pattern);
            if (match.Success)
            {
                return decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        // Method to extract product details using regular expression
        internal static List<Product> ExtractProductDetailsRG(string invoiceText, string pattern)
        {
            Console.WriteLine(pattern);
            List<Product> products = new List<Product>();
            MatchCollection matches = Regex.Matches(invoiceText, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                Product product = new Product();
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
    }
}