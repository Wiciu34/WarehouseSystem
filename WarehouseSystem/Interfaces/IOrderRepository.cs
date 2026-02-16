using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }
}
