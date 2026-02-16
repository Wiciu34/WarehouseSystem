using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface IProductValidator
    {
        bool Validate(Product product);
    }
}
