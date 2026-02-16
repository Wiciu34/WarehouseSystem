using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface IPricingService
    {
        decimal CalculateSubtotal(List<OrderItem> items);
    }
}
