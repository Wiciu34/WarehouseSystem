using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Services
{
    public class DiscountService : IDiscountService
    {
        public decimal ApplyDiscount(Customer customer, decimal subtotal)
        {
            decimal discount = 0m;

            // Logika: VIP dostaje 10%, a jeśli zamówienie > 1000 to dodatkowe 5%
            if (customer.IsVip) discount += 0.10m;

            if (subtotal > 1000) discount += 0.05m;

            return subtotal * (1 - discount);
        }
    }
}
