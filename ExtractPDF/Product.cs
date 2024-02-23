public class Product
{
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
