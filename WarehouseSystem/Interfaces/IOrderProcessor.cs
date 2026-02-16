using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface IOrderProcessor
    {
        Order ProcessOrder(Customer customer, List<OrderItem> items);
    }
}
