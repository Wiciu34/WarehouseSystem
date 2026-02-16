using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Validators
{
    public class ProductValidator : IProductValidator
    {
        public bool Validate(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name)) return false;
            if (product.BasePrice <= 0) return false;
            return true;
        }
    }
}
