namespace WarehouseSystem.Interfaces
{
    public interface IInventoryService
    {
        bool IsAvailable(int productId, int quantity);
        void ReduceStock(int productId, int quantity);
        void AddStock(int productId, int quantity);
    }
}
