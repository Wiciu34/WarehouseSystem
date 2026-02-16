using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Services
{
    public class PricingService : IPricingService
    {
        public decimal CalculateSubtotal(List<OrderItem> items)
        {
            return items.Sum(i => i.Product.BasePrice * i.Quantity);
        }
    }
}
