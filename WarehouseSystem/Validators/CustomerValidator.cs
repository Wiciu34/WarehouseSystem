using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Validators
{
    public class CustomerValidator : ICustomerValidator
    {
        public bool Validate(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name)) return false;
            if (!customer.Email.Contains("@")) return false;
            if (customer.Age < 18) return false;
            return true;
        }
    }
}
