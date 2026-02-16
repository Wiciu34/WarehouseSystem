using Moq;
using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Tests
{
    public class LoyaltyProgramManagerTests
    {
        private Mock<ILoyaltyExternalApi> _mockApi;
        private Mock<INotificationService> _mockNotification;
        private LoyaltyProgramManager _manager;

        [SetUp]
        public void Setup()
        {
            _mockApi = new Mock<ILoyaltyExternalApi>();
            _mockNotification = new Mock<INotificationService>();
            _manager = new LoyaltyProgramManager(_mockApi.Object, _mockNotification.Object);
        }

        //TC_024 Scenariusz: Klient nie dostaje punktów za zbyt małą wartość zamówienia
        //Dane: Zamównienie na kwotę 49.99 
        //Mock(LoyaltyExternalApi)
        //Akcja: LoyaltyProgramManager.RewartdCustomer
        //Oczekiwany wynik:
        //Metoda AddPoints nie zostaje wykonana

        [Test]
        public void Customer_should_not_get_points_for_small_orders()
        {
            var order = new Order { TotalAmount = 49.99m, Customer = new Customer { Id = 1 } };

           _manager.RewardCustomer(order);

           _mockApi.Verify(x => x.AddPoints(order.Customer.Id, It.IsAny<int>()), Times.Never);
        }

        //TC_025 Scenariusz: Klient dostaje punkty za zamówienie oraz dostaje powiadomienie
        //Dane: Zamównienie na kwotę 150.00 
        //Mock(LoyaltyExternalApi) : Mock(NotificationService)
        //Akcja: LoyaltyProgramManager.RewartdCustomer
        //Oczekiwany wynik:
        //Metoda AddPoints zostaje wykonana
        //Metoda SendOrderConfirmation zostaje wykonana

        [Test]
        public void Customer_should_get_correct_amount_of_points_and_notificaton_should_be_sent()
        {
            var order = new Order { TotalAmount = 150.00m, Customer = new Customer { Id = 2 } };
            _mockApi.Setup(x => x.AddPoints(2, 15)).Returns(true); // Oczekujemy dodania 15 punktów

            _manager.RewardCustomer(order);

            _mockApi.Verify(x => x.AddPoints(2, 15), Times.Once);
            _mockNotification.Verify(x => x.SendOrderConfirmation(order.Customer, order), Times.Once);
        }

        //TC_025 Scenariusz: Zostaje zwrócony wyjątek gdy API zwróci błąd
        //Dane: Zamównienie na kwotę 100.00 
        //Mock(LoyaltyExternalApi) : Mock(NotificationService)
        //Akcja: LoyaltyProgramManager.RewartdCustomer
        //Oczekiwany wynik:
        //RewartdCustomer wyrzuci wyjątek
        //Metoda SendOrderConfirmation nie zostaje wykonana

        [Test]
        public void Exception_should_be_thrown_when_loyalty_api_fails_and_notificaton_should_not_be_sent()
        {
            var order = new Order { TotalAmount = 100m, Customer = new Customer { Id = 4 } };
            _mockApi.Setup(x => x.AddPoints(4, 10)).Returns(false); // API odrzuca dodanie punktów

            Assert.Throws<Exception>(() => _manager.RewardCustomer(order));
            _mockNotification.Verify(x => x.SendOrderConfirmation(order.Customer, order), Times.Never);
        }
    }
}
