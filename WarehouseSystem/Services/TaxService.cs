using WarehouseSystem.Interfaces;

namespace WarehouseSystem.Services
{
    internal class TaxService : ITaxService
    {
        public decimal CalculateTax(decimal amount, decimal taxRate)
        {
            return Math.Round(amount * taxRate, 2);
        }
    }
}
