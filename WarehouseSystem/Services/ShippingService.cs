using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Services
{
    public class ShippingService : IShippingService
    {
        public decimal CalculateShippingCost(int totalWeight, string destinationCountry)
        {
            decimal cost = 15.0m;

            if (destinationCountry == "Poland")
            {
                cost -= 5.0m; // Zniżka krajowa
            }
            else if (destinationCountry == "USA")
            {
                cost += 20.0m; // Dopłata za strefę USA
            }

            if (totalWeight > 10)
            {
                cost += 10.0m; // Dopłata za nadbagaż
            }

            return cost;

        }

        public void GenerateShippingLabel(Order order)
        {
            Console.WriteLine($"Label generated for Order {order.Id}");
        }
    }
}
