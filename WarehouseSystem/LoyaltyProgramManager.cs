using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem
{
    public class LoyaltyProgramManager
    {
        private readonly ILoyaltyExternalApi _externalApi;
        private readonly INotificationService _notificationService;

        public LoyaltyProgramManager(ILoyaltyExternalApi externalApi, INotificationService notificationService)
        {
            _externalApi = externalApi;
            _notificationService = notificationService;
        }

        public void RewardCustomer(Order order)
        {
            if (order.TotalAmount < 50m) return;

            int points = (int)(order.TotalAmount / 10);

            bool success = _externalApi.AddPoints(order.Customer.Id, points);

            if (success)
            {
                _notificationService.SendOrderConfirmation(order.Customer, order);
            }
            else
            {
                throw new Exception("Loyalty API failed");
            }
        }
    }
}
