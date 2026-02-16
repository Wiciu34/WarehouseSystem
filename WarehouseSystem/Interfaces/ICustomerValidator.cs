using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface ICustomerValidator
    {
        bool Validate(Customer customer);
    }
}
