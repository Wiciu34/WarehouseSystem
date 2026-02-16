using WarehouseSystem.Interfaces;

namespace WarehouseSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly Dictionary<int, int> _stock = new Dictionary<int, int>();
        private readonly INotificationService _notificationService;
        public InventoryService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public void AddStock(int productId, int quantity)
        {
            if (_stock.ContainsKey(productId)) _stock[productId] = 0;
            else _stock.Add(productId, 0);
            _stock[productId] += quantity;
        }

        public bool IsAvailable(int productId, int quantity)
        {
            return _stock.ContainsKey(productId) && _stock[productId] >= quantity;
        }

        public void ReduceStock(int productId, int quantity)
        {
            if (!IsAvailable(productId, quantity))
            {
                throw new InvalidOperationException("Not enough stock.");
            }

            _stock[productId] -= quantity;

            if (_stock[productId] < 5) 
            {
                _notificationService.SendLowStockAlert(productId);
            }
        }
    }
}
