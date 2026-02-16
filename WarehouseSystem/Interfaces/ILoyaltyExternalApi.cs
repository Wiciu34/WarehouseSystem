namespace WarehouseSystem.Interfaces
{
    public interface ILoyaltyExternalApi
    {
        bool AddPoints(int customerId, int points);
    }
}
