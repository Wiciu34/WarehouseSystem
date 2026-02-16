using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Services
{
    internal class NotificationService : INotificationService
    {
        public void SendLowStockAlert(int productId)
        {
            Console.WriteLine($"ALERT: Product {productId} is low on stock.");
        }

        public void SendOrderConfirmation(Customer customer, Order order)
        {
            Console.WriteLine($"Email sent to {customer.Email} for Order #{order.Id}");
        }
    }
}
