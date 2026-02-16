using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface IDiscountService
    {
        decimal ApplyDiscount(Customer customer, decimal subtotal);
    }
}
