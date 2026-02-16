using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface INotificationService
    {
        void SendOrderConfirmation(Customer customer, Order order);
        void SendLowStockAlert(int productId);
    }
}
