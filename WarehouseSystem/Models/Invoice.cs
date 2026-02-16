namespace WarehouseSystem.Models
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public int OrderId { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
