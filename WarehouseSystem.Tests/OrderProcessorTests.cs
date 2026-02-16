using Moq;
using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Tests
{
    public class OrderProcessorTests
    {
        private Mock<ICustomerValidator> _mockValidator;
        private Mock<IInventoryService> _mockInventory;
        private Mock<IPricingService> _mockPricing;
        private Mock<IDiscountService> _mockDiscount;
        private Mock<ITaxService> _mockTax;
        private Mock<IOrderRepository> _mockRepo;
        private Mock<INotificationService> _mockNotification;
        private Mock<IShippingService> _mockShipping;

        private OrderProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _mockValidator = new Mock<ICustomerValidator>();
            _mockInventory = new Mock<IInventoryService>();
            _mockPricing = new Mock<IPricingService>();
            _mockDiscount = new Mock<IDiscountService>();
            _mockTax = new Mock<ITaxService>();
            _mockRepo = new Mock<IOrderRepository>();
            _mockNotification = new Mock<INotificationService>();
            _mockShipping = new Mock<IShippingService>();

            _processor = new OrderProcessor(
                _mockValidator.Object,
                _mockInventory.Object,
                _mockPricing.Object,
                _mockDiscount.Object,
                _mockTax.Object,
                _mockRepo.Object,
                _mockNotification.Object,
                _mockShipping.Object
            );
        }

        //TC_001. Scenariusz: Zamównie z³o¿one pomyœlnie(Happy Path)
        //Dane: Zamówienie na kwotê 100.00, karta kredytowa "1234-5678".

        //Mock(Validator) : Mock(Inventory) : Mock(Pricing) : Mock(Discount) : Mock(Tax) 
        //Validate zwraca true, IsAvailable zwraca true
        //CalculateSubtotal zwraca 100, ApplyDiscount zwraca 100,  CalculateTax zwraca 23

        //Akcja: _processor.ProcessOrder(customer, items)
        //Oczekiwany wynik:
        //Metoda zwraca obiekt Order.
        //Pole result.TotalAmount powinno byæ równe 123
        //Metody Save oraz SendOrderConfirmation powinny siê wykonaæ.

        [Test]
        public void ProcessOrder_should_process_correctly_when_data_is_valid()
        {
            var customer = new Customer { Name = "Test User", Email = "test@test.com", Age = 25 };
            var shippingDetails = new ShippingDetails { TotalWeight = 5, DestinationCountry = "Germany" };
            var items = new List<OrderItem> 
            { 
                new OrderItem 
                { 
                    Quantity = 1,
                    Product = new Product
                    {
                        Id = 1,
                        Name = "Laptop",
                        BasePrice = 100m
                    }
                } 
            };

            _mockValidator.Setup(x => x.Validate(customer)).Returns(true); //Customer is valid
            _mockInventory.Setup(x => x.IsAvailable(1, It.IsAny<int>())).Returns(true); //Product is avalible
            _mockPricing.Setup(x => x.CalculateSubtotal(items)).Returns(100m); //Cost of basket is 100
            _mockDiscount.Setup(x => x.ApplyDiscount(customer, 100m)).Returns(100m);//No discount
            _mockTax.Setup(x => x.CalculateTax(100m, It.IsAny<decimal>())).Returns(23m); //Tax 23

            var result = _processor.ProcessOrder(customer, items, shippingDetails);

            Assert.IsNotNull(result, "Order should be created");
            Assert.That(result.TotalAmount, Is.EqualTo(123m), "Total amount should be eaqule to 123");

            _mockRepo.Verify(x => x.Save(It.IsAny<Order>()), Times.Once, "Order should be saved");
            _mockNotification.Verify(x => x.SendOrderConfirmation(customer, result), Times.Once, "Email should be send");
        }

        //TC_002 Scenariusz: Zamównie nie zostaje z³o¿one klient nie przeszed³ weryfikacji

        //Mock(Validator) : Mock(Inventory) : Mock(Pricing) : Mock(Discount) : Mock(Tax) 
        //Validate zwraca false,
        //IsAvailable zwraca true
        //CalculateSubtotal zwraca 100, ApplyDiscount zwraca 100,  CalculateTax zwraca 23

        //Akcja: _processor.ProcessOrder(customer, items)
        //Oczekiwany wynik:
        //Metoda powinna wyrzuciæ wyj¹tek typu ArgumentException o wiadomoœci "Invalid customer."
        //Metody Save oraz SendOrderConfirmation niepowinny siê wykonaæ.

        [Test]
        public void ProcessOrder_should_throw_expection_when_customer_is_invalid()
        {
            var customer = new Customer { Name = "", Email = "test@test.com", Age = 25 };
            var shippingDetails = new ShippingDetails { TotalWeight = 5, DestinationCountry = "Germany" };
            var items = new List<OrderItem>
            {
                new OrderItem
                {
                    Quantity = 1,
                    Product = new Product
                    {
                        Id = 1,
                        Name = "Laptop",
                        BasePrice = 100m
                    }
                }
            };

            _mockValidator.Setup(x => x.Validate(customer)).Returns(false); //Customer is invalid
            _mockInventory.Setup(x => x.IsAvailable(1, 1)).Returns(true); //Product is available
            _mockPricing.Setup(x => x.CalculateSubtotal(items)).Returns(100m); //Cost of basket is 100
            _mockDiscount.Setup(x => x.ApplyDiscount(customer, 100m)).Returns(100m); //No discount
            _mockTax.Setup(x => x.CalculateTax(100m, It.IsAny<decimal>())).Returns(23m); //Tax 23

            var expection = Assert.Throws<ArgumentException>(() =>
                _processor.ProcessOrder(customer, items, shippingDetails)
            );

            Assert.That(expection.Message, Is.EqualTo("Invalid customer."));
            _mockRepo.Verify(x => x.Save(It.IsAny<Order>()), Times.Never, "Order shouldn't be saved.");
            _mockNotification.Verify(x => x.SendOrderConfirmation(customer, It.IsAny<Order>()), Times.Never, "Email shouldn't be send.");
        }

        //TC_003 Scenariusz: Zamównie nie zostaje z³o¿one klient nie przeszed³ weryfikacji

        //Mock(Validator) : Mock(Inventory) : Mock(Pricing) : Mock(Discount) : Mock(Tax) 
        //Validate zwraca true,
        //IsAvailable zwraca false
        //CalculateSubtotal zwraca 100, ApplyDiscount zwraca 100,  CalculateTax zwraca 23

        //Akcja: _processor.ProcessOrder(customer, items)
        //Oczekiwany wynik:
        //Metoda powinna wyrzuciæ wyj¹tek typu InvalidOperationException o wiadomoœci $"Product {items[0].Product.Name} is out of stock."
        //Metody Save oraz SendOrderConfirmation niepowinny siê wykonaæ.

        [Test]
        public void ProcessOrder_should_throw_expection_when_product_is_not_available()
        {
            var customer = new Customer { Name = "Jan Kowalski", Email = "test@test.com", Age = 25 };
            var shippingDetails = new ShippingDetails { TotalWeight = 5, DestinationCountry = "Germany" };
            var items = new List<OrderItem>
            {
                new OrderItem
                {
                    Quantity = 0,
                    Product = new Product
                    {
                        Id = 1,
                        Name = "Laptop",
                        BasePrice = 100m
                    }
                }
            };

            _mockValidator.Setup(x => x.Validate(customer)).Returns(true); //Customer is valid
            _mockInventory.Setup(x => x.IsAvailable(1, 0)).Returns(false); //Product is not available
            _mockPricing.Setup(x => x.CalculateSubtotal(items)).Returns(100m); //Cost of basket is 100
            _mockDiscount.Setup(x => x.ApplyDiscount(customer, 100m)).Returns(100m); //No discount
            _mockTax.Setup(x => x.CalculateTax(100m, It.IsAny<decimal>())).Returns(23m); //Tax 23

            var expection = Assert.Throws<InvalidOperationException>(() =>
                _processor.ProcessOrder(customer, items, shippingDetails)
            );

            Assert.That(expection.Message, Is.EqualTo($"Product {items[0].Product.Name} is out of stock."));
            _mockRepo.Verify(x => x.Save(It.IsAny<Order>()), Times.Never, "Order shouldn't be saved.");
            _mockNotification.Verify(x => x.SendOrderConfirmation(customer, It.IsAny<Order>()), Times.Never, "Email shouldn't be send.");

        }

        //TC_004 Scenariusz: Nie zostaje wys³ana informacja do klienta przez b³¹d bazy danych

        //Mock(Validator) : Mock(Inventory)
        //Validate zwraca true,
        //IsAvailable zwraca true
        //CalculateSubtotal zwraca 100, ApplyDiscount zwraca 100,  CalculateTax zwraca 23

        //Akcja: _processor.ProcessOrder(customer, items)
        //Oczekiwany wynik:
        //Metoda SendOrderConfirmation niepowinna siê wykonaæ.

        [Test]
        public void ProcessOrder_should_not_send_email_when_database_throws_exception()
        {
            var customer = new Customer { Name = "Test User", Email = "test@test.com", Age = 25 };
            var shippingDetails = new ShippingDetails { TotalWeight = 5, DestinationCountry = "Germany" };
            var items = new List<OrderItem> { new OrderItem { Quantity = 1, Product = new Product { Id = 1, BasePrice = 100m } } };

            _mockValidator.Setup(x => x.Validate(customer)).Returns(true);
            _mockInventory.Setup(x => x.IsAvailable(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            // Symulujemy awariê bazy danych!
            _mockRepo.Setup(x => x.Save(It.IsAny<Order>())).Throws(new Exception("Database connection lost"));

            Assert.Throws<Exception>(() => _processor.ProcessOrder(customer, items, shippingDetails));

            _mockNotification.Verify(x => x.SendOrderConfirmation(It.IsAny<Customer>(), It.IsAny<Order>()), Times.Never);
        }

        //TC_005 Scenariusz: Sprawdzenie czy podatek poprawnie jest obliczany po zni¿ce

        //Mock(Validator) : Mock(Inventory) : Mock(Pricing) : Mock(Discount) : Mock(Tax) 
        //Validate zwraca true,
        //IsAvailable zwraca true
        //CalculateSubtotal zwraca 100, ApplyDiscount zwraca 80, 

        //Akcja: _processor.ProcessOrder(customer, items)
        //Oczekiwany wynik:
        //CalculateTax powinno wykonaæ siê raz z watoœci¹ 80.00

        [Test]
        public void ProcessOrder_should_calculate_tax_based_on_discounted_amount()
        {
            var customer = new Customer { Name = "VIP", Age = 30, IsVip = true};
            var shippingDetails = new ShippingDetails { TotalWeight = 5, DestinationCountry = "Germany" };
            var items = new List<OrderItem> { new OrderItem { Quantity = 1, Product = new Product { Id = 1 } } };

            _mockValidator.Setup(x => x.Validate(customer)).Returns(true);
            _mockInventory.Setup(x => x.IsAvailable(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            _mockPricing.Setup(x => x.CalculateSubtotal(items)).Returns(100m);
            _mockDiscount.Setup(x => x.ApplyDiscount(customer, 100m)).Returns(80m);

            _processor.ProcessOrder(customer, items, shippingDetails);

            _mockTax.Verify(x => x.CalculateTax(80m, It.IsAny<decimal>()), Times.Once);
        }
    }
}