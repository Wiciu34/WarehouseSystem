namespace WarehouseSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public string Category { get; set; }
        public int WeightInKg { get; set; }
    }
}
