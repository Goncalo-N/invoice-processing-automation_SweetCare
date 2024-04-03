public class Product
{
    // Product's code.
    public string Code { get; set; }

    // Barcode of the product.
    public string Barcode { get; set; }

    // Description or name of the product.
    public string Description { get; set; }

    // Quantity of the product for the order.
    public int Quantity { get; set; }

    // Unit price of the product before any discounts.
    public decimal UnitPrice { get; set; }

    // First discount applied to the product.
    public decimal Discount1 { get; set; }

    // Second discount applied to the product.
    public decimal Discount2 { get; set; }

    // Third discount applied to the product.
    public decimal Discount3 { get; set; }

    // Final price after all discounts are applied.
    public decimal NetPrice { get; set; }

    // Price of the product including VAT (Value Added Tax).
    public decimal PrecoComIVA { get; set; }

    // Bonus quantity that come with the product.
    public int Bonus { get; set; }

    public int isFactUpdated { get; set; }

    // Overrides the ToString method to provide a string representation of the product details.
    public override string ToString()
    {
        return $"Code: {Code}, Barcode: {Barcode}, Description: {Description}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, " +
               $"Discounts: {Discount1}, {Discount2}, {Discount3}, NetPrice: {NetPrice}, PrecoComIVA: {PrecoComIVA}, Bonus: {Bonus} isFactUpdated: {isFactUpdated}";
    }
}

public class LEXProduct : Product
{
    public string Name { get; set; }
    public string CNP { get; set; }
    public string LotNumber { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int IVA { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, Name: {Name}, CNP: {CNP}, LotNumber: {LotNumber}, DiscountPercentage: {DiscountPercentage}, IVA: {IVA}";
    }
}
/*
public class MorenoProduct : Product
{
    public string CNP { get; set; }
    public string Designation { get; set; }
    public string Lot { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Type { get; set; }
    public int IVA { get; set; }
    public decimal Total { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, CNP: {CNP}, Designation: {Designation}, Lot: {Lot}, ExpiryDate: {ExpiryDate.ToString("yyyy-MM-dd")}, " +
               $"Type: {Type}, IVA: {IVA}, Total: {Total}";
    }
}*/
