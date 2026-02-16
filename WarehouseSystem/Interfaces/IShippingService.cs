using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface IShippingService
    {
        decimal CalculateShippingCost(int totalWeight, string destinationCountry);
        void GenerateShippingLabel(Order order);
    }
}
