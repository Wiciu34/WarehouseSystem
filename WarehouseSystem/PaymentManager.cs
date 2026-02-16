using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem
{
    public class PaymentManager
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IShippingService _shippingService;

        public PaymentManager(IPaymentGateway paymentGateway, IShippingService shippingService)
        {
            _paymentGateway = paymentGateway;
            _shippingService = shippingService;

        }

        public bool PayForOrder(Order order, string creditCardNumber)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (string.IsNullOrEmpty(creditCardNumber)) throw new ArgumentException("Card number required.");

            if (!_paymentGateway.IsCardValid(creditCardNumber))
            {
                Console.WriteLine("Payment failed: Invalid card.");
                return false;
            }

            bool success = _paymentGateway.ProcessPayment(creditCardNumber, order.TotalAmount);

            if (success)
            {
                order.IsPaid = true;
                _shippingService.GenerateShippingLabel(order);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
