public class IProduct
{
    //ORIGINAL PRODUCT CLASS IS USED FOR R&G INVOICES
    public string Article { get; set; }
    public string Barcode { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal GrossPrice { get; set; }
    public decimal Discount1 { get; set; }
    public decimal Discount2 { get; set; }
    public decimal Discount3 { get; set; }
    public decimal PrecoSemIVA { get; set; }
    public decimal PrecoComIVA { get; set; }
    public int Bonus { get; set; }

    public override string ToString()
    {
        return $"Article: {Article}, Barcode: {Barcode}, Description: {Description}, Quantity: {Quantity}, GrossPrice: {GrossPrice}, " +
               $"Discount1: {Discount1}, Discount2: {Discount2}, Discount3: {Discount3}, PrecoSemIVA: {PrecoSemIVA}, PrecoComIVA: {PrecoComIVA}, " +
               $"Bonus: {Bonus}";
    }
}
//LEX INVOICES
public class LEXProduct : IProduct
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string CNP { get; set; }
    public string LotNumber { get; set; }
    public string Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal NetPrice { get; set; }
    public int IVA { get; set; }

    public override string ToString()
    {
        return $"Code: {Code}, Name: {Name}, CNP: {CNP}, LotNumber: {LotNumber}, Quantity: {Quantity}, " +
               $"UnitPrice: {UnitPrice}, DiscountPercentage: {DiscountPercentage}, NetPrice: {NetPrice}, IVA: {IVA}";
    }
}

// Moreno II INVOICES
public class MorenoProduct : IProduct
{
    public string CNP { get; set; }
    public string Designation { get; set; }
    public string Lot { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Type { get; set; }
    public string Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount1 { get; set; }
    public decimal Discount2 { get; set; }
    public decimal NetPrice { get; set; }
    public int IVA { get; set; }
    public decimal Total { get; set; }

    public override string ToString()
    {
        return $"CNP: {CNP}, Designation: {Designation}, Lot: {Lot}, ExpiryDate: {ExpiryDate}, " +
               $"Type: {Type}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, Discount1: {Discount1}, " +
               $"Discount2: {Discount2}, NetPrice: {NetPrice}, IVA: {IVA}, Total: {Total}";
    }
}


