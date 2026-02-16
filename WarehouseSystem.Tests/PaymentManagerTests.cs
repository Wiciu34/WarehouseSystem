using Moq;
using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Tests
{
    public class PaymentManagerTests
    {
        private Mock<IPaymentGateway> _paymentGatewayMock;
        private Mock<IShippingService> _shippingServiceMock;
        private PaymentManager _paymentManager;

        [SetUp]
        public void Setup()
        {
            _paymentGatewayMock = new Mock<IPaymentGateway>();
            _shippingServiceMock = new Mock<IShippingService>();
            _paymentManager = new PaymentManager(_paymentGatewayMock.Object, _shippingServiceMock.Object);
        }


        //TC_006 Scenariusz: Płatność zakończona sukcesem(Happy Path)
        //Dane: Zamówienie na kwotę 100.00, karta kredytowa "1234-5678".

        //Mock(Bramka) :
        //IsCardValid zwraca true.
        //ProcessPayment zwraca true (bank zaakceptował).

        //Akcja: _paymentManager.PayForOrder(order, card)
        //Oczekiwany wynik:
        //Metoda zwraca true.
        //Pole order.IsPaid zmienia się na true.

        [Test]
        public void Payment_ends_with_success()
        {
            var order = new Order
            {
                Id = 1,
                TotalAmount = 220,
                IsPaid = false,
                paymentDetails = new PaymentDetails
                {
                    CreditCardNumber = "1234-5678",
                    Cvv = "555"
                }
            };

            _paymentGatewayMock.Setup(x => x.IsCardValid(order.paymentDetails.CreditCardNumber)).Returns(true);
            _paymentGatewayMock.Setup(x => x.ProcessPayment(order.paymentDetails.CreditCardNumber, order.TotalAmount)).Returns(true);

            var result = _paymentManager.PayForOrder(order, order.paymentDetails.CreditCardNumber);

            Assert.IsTrue(result);
            Assert.IsTrue(order.IsPaid);
            _shippingServiceMock.Verify(x => x.GenerateShippingLabel(order), Times.Once);
        }

        //TC_007 Scenariusz: Odmowa banku(Brak środków)
        //Dane: Zamówienie na kwotę 5000.00, karta "1234-5678".

        //Mock(Bramka) :
        //IsCardValid zwraca true.
        //ProcessPayment zwraca false (bank odrzucił).

        //Akcja: _paymentManager.PayForOrder(order, card)
        //Oczekiwany wynik:
        //Metoda zwraca false.
        //Pole order.IsPaid pozostaje false.

        [Test]
        public void Payment_end_with_fail_with_not_enough_credit()
        {
            var order = new Order
            {
                Id = 1,
                TotalAmount = 5000,
                IsPaid = false,
                paymentDetails = new PaymentDetails
                {
                    CreditCardNumber = "1234-5678",
                    Cvv = "555"
                }
            };

            _paymentGatewayMock.Setup(x => x.IsCardValid(order.paymentDetails.CreditCardNumber)).Returns(true);
            _paymentGatewayMock.Setup(x => x.ProcessPayment(order.paymentDetails.CreditCardNumber, order.TotalAmount)).Returns(false);

            var result = _paymentManager.PayForOrder(order, order.paymentDetails.CreditCardNumber);

            Assert.IsFalse(result);
            Assert.IsFalse(order.IsPaid);
            _shippingServiceMock.Verify(x => x.GenerateShippingLabel(order), Times.Never);
        }

        //TC_008 Scenariusz: Próba płatności skradzioną/nieważną kartą
        //Dane: Zamówienie na kwotę 50.00, karta "0000-INVALID".

        //Mock(Bramka) :
        //IsCardValid zwraca false.

        //Akcja: PaymentManager.PayForOrder(order, card)
        //Oczekiwany wynik:
        //Metoda zwraca false.
        //Metoda ProcessPayment NIGDY nie została wywołana.

        [Test]
        public void Payment_end_with_fail_with_invalid_credit_card()
        {
            var order = new Order
            {
                Id = 1,
                TotalAmount = 5000,
                IsPaid = false,
                paymentDetails = new PaymentDetails
                {
                    CreditCardNumber = "0000-INVALID",
                    Cvv = "555"
                }
            };

            _paymentGatewayMock.Setup(x => x.IsCardValid(order.paymentDetails.CreditCardNumber)).Returns(false);
            _paymentGatewayMock.Setup(x => x.ProcessPayment(order.paymentDetails.CreditCardNumber, order.TotalAmount)).Returns(true);

            var result = _paymentManager.PayForOrder(order, order.paymentDetails.CreditCardNumber);

            Assert.IsFalse(result);
            
            _paymentGatewayMock.Verify(x => x.ProcessPayment(order.paymentDetails.CreditCardNumber, order.TotalAmount), Times.Never);
            _shippingServiceMock.Verify(x => x.GenerateShippingLabel(order), Times.Never);
        }
    }
}
