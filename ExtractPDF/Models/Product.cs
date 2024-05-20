using System.Text;

namespace PDFDataExtraction.Models
{
    public class Product
    {
        /// <summary>
        /// Product's code.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Barcode of the product.
        /// </summary>
        public string Barcode { get; set; } = string.Empty;

        /// <summary>
        /// Description or name of the product.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of the product for the order.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Price of the product including VAT (Value Added Tax).
        /// </summary>
        public decimal PrecoComIVA { get; set; }

        /// <summary>
        /// Unit price of the product before any discounts.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// First discount applied to the product.
        /// </summary>
        public decimal Discount1 { get; set; }

        /// <summary>
        /// Second discount applied to the product.
        /// </summary>
        public decimal Discount2 { get; set; }

        // Third discount applied to the product.
        public decimal Discount3 { get; set; }

        /// <summary>
        /// Final price after all discounts are applied.
        /// </summary>
        public decimal NetPrice { get; set; }



        /// <summary>
        /// Bonus quantity that come with the product.
        /// </summary>
        public int Bonus { get; set; }

        public int isFactUpdated { get; set; }

        public decimal IVA { get; set; }

        public string CNP { get; set; } = string.Empty;

        /// <summary>
        /// Overrides the ToString method to provide a string representation of the product details.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Code))
                stringBuilder.Append($"Code: {Code}, ");
            if (!string.IsNullOrEmpty(Barcode))
                stringBuilder.Append($"Barcode: {Barcode}, ");
            if (!string.IsNullOrEmpty(Description))
                stringBuilder.Append($"Description: {Description}, ");
            if (Quantity != 0)
                stringBuilder.Append($"Quantity: {Quantity}, ");
            if (PrecoComIVA != 0)
                stringBuilder.Append($"PrecoComIVA: {PrecoComIVA}, ");
            if (UnitPrice != 0)
                stringBuilder.Append($"UnitPrice: {UnitPrice}, ");
            if (Discount1 != 0)
                stringBuilder.Append($"Discounts: {Discount1}, ");
            if (Discount2 != 0)
                stringBuilder.Append($"{Discount2}, ");
            if (Discount3 != 0)
                stringBuilder.Append($"{Discount3}, ");
            if (NetPrice != 0)
                stringBuilder.Append($"NetPrice: {NetPrice}, ");
            if (Bonus != 0)
                stringBuilder.Append($"Bonus: {Bonus}, ");
            if (IVA != 0)
                stringBuilder.Append($"IVA: {IVA}, ");
            if (!string.IsNullOrEmpty(CNP))
                stringBuilder.Append($"CNP: {CNP}, ");
            stringBuilder.Append($"isFactUpdated: {isFactUpdated}");

            return stringBuilder.ToString();
        }
    }

    public class LEXProduct : Product
    {
        public string Name { get; set; } = string.Empty;
        public string LotNumber { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(base.ToString());

            if (!string.IsNullOrEmpty(Name))
                stringBuilder.Append($", Name: {Name}");
            if (!string.IsNullOrEmpty(CNP))
                stringBuilder.Append($", CNP: {CNP}");
            if (!string.IsNullOrEmpty(LotNumber))
                stringBuilder.Append($", LotNumber: {LotNumber}");
            stringBuilder.Append($", DiscountPercentage: {DiscountPercentage}");

            return stringBuilder.ToString();
        }

    }

    public class OCPProduct : Product
    {
        public string Name { get; set; } = string.Empty;
        public string LotNumber { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(base.ToString());

            if (!string.IsNullOrEmpty(Name))
                stringBuilder.Append($", Name: {Name}");
            if (!string.IsNullOrEmpty(CNP))
                stringBuilder.Append($", CNP: {CNP}");
            if (!string.IsNullOrEmpty(LotNumber))
                stringBuilder.Append($", LotNumber: {LotNumber}");
            stringBuilder.Append($", DiscountPercentage: {DiscountPercentage}");

            return stringBuilder.ToString();
        }

    }
    public class MERCAFARProduct : Product
    {
        public string Name { get; set; } = string.Empty;
        public string LotNumber { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }

        public int QtPed { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(base.ToString());

            if (!string.IsNullOrEmpty(Name))
                stringBuilder.Append($", Name: {Name}");
            if (!string.IsNullOrEmpty(CNP))
                stringBuilder.Append($", CNP: {CNP}");
            if (!string.IsNullOrEmpty(LotNumber))
                stringBuilder.Append($", LotNumber: {LotNumber}");
            if (QtPed != 0)
                stringBuilder.Append($", QtPed: {QtPed}");
            stringBuilder.Append($", DiscountPercentage: {DiscountPercentage}");

            return stringBuilder.ToString();
        }
    }
}