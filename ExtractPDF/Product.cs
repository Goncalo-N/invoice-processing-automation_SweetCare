public class IProduct
{
    // Defines a base product class for R&G invoices with common properties.

    // Unique identifier or name for the article.
    public virtual string Code { get; set; }

    // Barcode of the product for inventory tracking.
    public virtual string Barcode { get; set; }

    // Description of the product.
    public virtual string Description { get; set; }

    // Quantity of the product in the invoice.
    public virtual int Quantity { get; set; }

    // Gross price before any discounts are applied.
    public virtual decimal UnitPrice { get; set; }

    // First level of discount applied to the gross price.
    public virtual decimal Discount1 { get; set; }

    // Second level of discount applied after the first.
    public virtual decimal Discount2 { get; set; }

    // Third level of discount applied after the second.
    public virtual decimal Discount3 { get; set; }

    // Price of the product without VAT (Value Added Tax).
    public virtual decimal NetPrice { get; set; }

    // Price of the product with VAT included.
    public virtual decimal PrecoComIVA { get; set; }

    // Bonus points or units given with the product.
    public virtual int Bonus { get; set; }

    // Overrides the ToString method to provide a string representation of the product details.
    public override string ToString()
    {
        return $"Code: {Code}, Barcode: {Barcode}, Description: {Description}, Quantity: {Quantity}, GrossPrice: {UnitPrice}, " +
               $"Discount1: {Discount1}, Discount2: {Discount2}, Discount3: {Discount3}, PrecoSemIVA: {NetPrice}, PrecoComIVA: {PrecoComIVA}, " +
               $"Bonus: {Bonus}";
    }
}

// LEX Invoices derived class.
public class LEXProduct : IProduct
{
    // Additional code property specific to LEX products.
    public override string Code { get; set; }

    // Name of the LEX product.
    public string Name { get; set; }

    // National code of the product.
    public string CNP { get; set; }

    // Lot number for tracking batches of products.
    public string LotNumber { get; set; }

    // Overrides Quantity property from IProduct for specific LEX usage.
    public override int Quantity { get; set; }

    // Price per unit of the product.
    public override decimal UnitPrice { get; set; }

    // Percentage of discount applied to the unit price.
    public decimal DiscountPercentage { get; set; }

    // Net price after discounts are applied.
    public override decimal NetPrice { get; set; }

    // Value Added Tax rate applicable to the product.
    public int IVA { get; set; }

    // Overrides the ToString method to provide a string representation of LEX product details.
    public override string ToString()
    {
        return $"Code: {Code}, Name: {Name}, CNP: {CNP}, LotNumber: {LotNumber}, Quantity: {Quantity}, " +
               $"UnitPrice: {UnitPrice}, DiscountPercentage: {DiscountPercentage}, NetPrice: {NetPrice}, IVA: {IVA}";
    }
}

// Moreno II Invoices derived class.
public class MorenoProduct : IProduct
{
    // National code of the product, overridden from IProduct.
    public string CNP { get; set; }

    // Designation or name of the Moreno product.
    public string Designation { get; set; }

    // Lot number for tracking product batches.
    public string Lot { get; set; }

    // Expiry date of the product.
    public DateTime ExpiryDate { get; set; }

    // Type or category of the product.
    public string Type { get; set; }

    // Overrides Quantity property from IProduct for specific Moreno usage.
    public new int Quantity { get; set; }

    // Price per unit of the product.
    public override decimal UnitPrice { get; set; }

    // First level of discount applied to the unit price.
    public decimal Discount1 { get; set; }

    // Second level of discount applied after the first.
    public decimal Discount2 { get; set; }

    // Net price after discounts are applied.
    public override decimal NetPrice { get; set; }

    // Value Added Tax rate applicable to the product.
    public int IVA { get; set; }

    // Total price after all calculations.
    public decimal Total { get; set; }

    // Overrides the ToString method to provide a string representation of Moreno product details.
    public override string ToString()
    {
        return $"CNP: {CNP}, Designation: {Designation}, Lot: {Lot}, ExpiryDate: {ExpiryDate}, " +
               $"Type: {Type}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, Discount1: {Discount1}, " +
               $"Discount2: {Discount2}, NetPrice: {NetPrice}, IVA: {IVA}, Total: {Total}";
    }
}
