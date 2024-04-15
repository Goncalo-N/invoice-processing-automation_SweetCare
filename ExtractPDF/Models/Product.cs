public class Product
{
     /// <summary>
    /// Product's code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Barcode of the product.
    /// </summary>
    public string Barcode { get; set; }

    /// <summary>
    /// Description or name of the product.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Quantity of the product for the order.
    /// </summary>
    public int Quantity { get; set; }

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

    /// <summary>
    /// Third discount applied to the product.
    /// </summary>
    public decimal Discount3 { get; set; }

    /// <summary>
    /// Final price after all discounts are applied.
    /// </summary>
    public decimal NetPrice { get; set; }

    /// <summary>
    /// Price of the product including VAT (Value Added Tax).
    /// </summary>
    public decimal PrecoComIVA { get; set; }

    /// <summary>
    /// Bonus quantity that come with the product.
    /// </summary>
    public int Bonus { get; set; }

    public int isFactUpdated { get; set; }

    public decimal IVA { get; set; }

    public string CNP { get; set; }

    /// <summary>
    /// Overrides the ToString method to provide a string representation of the product details.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Code: {Code}, Barcode: {Barcode}, Description: {Description}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, " +
               $"Discounts: {Discount1}, {Discount2}, {Discount3}, NetPrice: {NetPrice}, PrecoComIVA: {PrecoComIVA}, Bonus: {Bonus}, IVA: {IVA}, CNP: {CNP} isFactUpdated: {isFactUpdated}";
    }
}

public class LEXProduct : Product
{
    public string Name { get; set; }
    public string LotNumber { get; set; }
    public decimal DiscountPercentage { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, Name: {Name}, CNP: {CNP}, LotNumber: {LotNumber}, DiscountPercentage: {DiscountPercentage}, IVA: {IVA}";
    }
}
