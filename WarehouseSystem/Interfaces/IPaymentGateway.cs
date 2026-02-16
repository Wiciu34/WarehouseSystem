namespace WarehouseSystem.Interfaces
{
    public interface IPaymentGateway
    {
        bool ProcessPayment(string creditCardNumber, decimal amount);
        bool IsCardValid(string creditCardNumber);
    }
}
