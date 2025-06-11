public class InvoiceItemViewModel
{
    public int InvoiceItemId { get; set; }
    public string MedicineName { get; set; }
    public string BatchId { get; set; }
    public int MedId { get; set; }
    public int AvailableQty { get; set; }
    public string Expiry { get; set; }
    public int Quantity { get; set; }
    public double MRP { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Total { get; set; }
    public required DateTime PrushereDate { get; set; }
    public double SalePrice { get; set; }
}
