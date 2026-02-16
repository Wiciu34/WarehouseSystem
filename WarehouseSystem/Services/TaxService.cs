using WarehouseSystem.Interfaces;

namespace WarehouseSystem.Services
{
    public class TaxService : ITaxService
    {
        public decimal CalculateTax(decimal amount, decimal taxRate)
        {
            return Math.Round(amount * taxRate, 2);
        }
    }
}
