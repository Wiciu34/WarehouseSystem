namespace WarehouseSystem.Interfaces
{
    public interface ITaxService
    {
        decimal CalculateTax(decimal amount, decimal taxRate);
    }
}
