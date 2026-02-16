using System.IO;
using WarehouseSystem.Models;
using WarehouseSystem.Services;
using WarehouseSystem.Validators;

namespace WarehouseSystem.Tests
{
    public class ValidatorsTests
    {
        private CustomerValidator _validator;
        private DiscountService _discountService;
        private InvoiceService _invoiceService;
        private ShippingService _shippingService;

        [SetUp]
        public void setup()
        {
            _validator = new CustomerValidator();
            _discountService = new DiscountService();
            _invoiceService = new InvoiceService();
            _shippingService = new ShippingService();
        }
        //TC_011 Scenariusz: Weryfikacja zakończona negatywnie przez zbyt młody wiek
        //Dane: Klient: Imię: Jan Kowalski, Email: "jan.kowalski@test.pl", wiek:  17
        //Akacja: _validator.Validate(customer)
        //Oczekiwany wynik: false

        [Test]
        public void Customer_should_not_be_valid_beacause_of_age()
        {
            var customer = new Customer
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@test.pl",
                Age = 17
            };

            var result = _validator.Validate(customer);

            Assert.IsFalse(result, "Customer should be 18 or above.");
        }
        //TC_012 Scenariusz: Weryfikacja zakończona negatywnie przez błędny email
        //Dane: Klient: Imię: Jan Kowalski, Email: "jan.kowalskitest.pl", wiek:  25
        //Akacja: _validator.Validate(customer)
        //Oczekiwany wynik: false

        [Test]
        public void Customer_sholud_not_be_valid_because_of_incorrect_email()
        {
            var validator = new CustomerValidator();

            var customer = new Customer
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalskitest.pl",
                Age = 25
            };

            var result = _validator.Validate(customer);

            Assert.IsFalse(result, "Customer should have '@' in his email.");
        }

        //TC_013 Scenariusz: Weryfikacja zakończona negatywnie przez błędny email
        //Dane: Klient: Imię: Jan Kowalski, Email: "jan.kowalski@test.pl", wiek:  25
        //Akacja: _validator.Validate(customer)
        //Oczekiwany wynik: false

        [Test]
        public void Customer_should_be_valid()
        {
            var customer = new Customer
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@test.pl",
                Age = 25,
                
            };

            var result = _validator.Validate(customer);

            Assert.IsTrue(result, "Customer is valid.");
        }

        //TC_014 Scenariusz: Naliczanie rabatu dla Klienta VIP
        //Dane: Customer { IsVip = true }, wartość koszyka 100.00
        //Akcja: _discountService.ApplyDiscount()
        //Oczekiwany wynik: Kwota 90.00 (10% zniżki).

        [Test]
        public void Customer_should_have_discount_of_10()
        {

            var customer = new Customer
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@test.pl",
                Age = 25,
                IsVip = true
            };

            decimal subtotal = 100.00m;

            var result = _discountService.ApplyDiscount(customer, subtotal);

            Assert.That(result, Is.EqualTo(90m), "With 10% discount result should be equal 90.");
        }


        //TC_015 Scenariusz: Kumulacja rabatów(VIP + Duże zakupy)
        //Dane: Customer { IsVip = true }, wartość koszyka 2000.00
        //Akcja: _discountService.ApplyDiscount()
        //Oczekiwany wynik: Kwota pomniejszona o 15% (10% za VIP + 5% za kwotę > 1000).

        [Test]
        public void Customer_should_have_discount_of_15()
        {
            var customer = new Customer
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@test.pl",
                Age = 25,
                IsVip = true
            };

            decimal subtotal = 2000.00m;

            var result = _discountService.ApplyDiscount(customer, subtotal);

            Assert.That(result, Is.EqualTo(1700m), "With 15% discount result should be equal 1700.");
        }

        //TC_016 Scenariusz: Zwykły klient, małe zakupy (Brak rabatu)
        //Dane: Customer { IsVip = false }, wartość koszyka 100.00
        //Akcja: _discountService.ApplyDiscount()
        //Oczekiwany wynik: Kwota bez zmian (100.00).

        [Test]
        public void Customer_should_not_have_discount()
        {

            var customer = new Customer
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@test.pl",
                Age = 25,
                IsVip = false
            };

            decimal subtotal = 100.00m;

            var result = _discountService.ApplyDiscount(customer, subtotal);

            Assert.That(result, Is.EqualTo(subtotal), "With no discount result should be equal to subtotal.");
        }

        //TC_017 Scenariusz: Próba wystawienia faktury na 0 zł
        //Dane: Zamówienie z TotalAmount = 0.
        //Akcja: _invoiceService.GenerateInvoice()
        //Oczekiwany wynik: Wyjątek InvalidOperationException.

        [Test]
        public void InvoiceService_should_throw_expection_when_totalAmount_0()
        {
            Order order = new Order
            {
                Customer = new Customer { },
                TotalAmount = 0,
                OrderDate = DateTime.Now,
            };

            Assert.Throws<InvalidOperationException>(() => _invoiceService.GenerateInvoice(order));
        }

        //TC_018 Scenariusz: Poprawne wygenerowanie numeru faktury
        //Dane: Zamówienie ID = 123, Rok = 2026.
        //Akcja: _invoiceService.GenerateInvoice()
        //Oczekiwany wynik: Numer faktury zawiera tekst "INV-2026-123".

        [Test]
        public void InvoiceService_should_generate_invoice_when_order_is_valid()
        {
            Order order = new Order
            {
                Id = 123,
                Customer = new Customer { },
                TotalAmount = 10,
                OrderDate = DateTime.Now,
            };

            var result = _invoiceService.GenerateInvoice(order);

            Assert.That(result.InvoiceNumber, Is.EqualTo("INV-2026-123"));
        }

        //TC_019 Scenariusz: Standardowa przesyłka krajowa
        //Dane: Waga 5 kg, Kraj "Poland"
        //Akcja: CalculateShippingCost(5, "Poland")
        //Oczekiwany wynik: 10.00.

        [Test]
        public void CalculateCost_should_apply_discount_for_poland()
        {
            decimal cost = _shippingService.CalculateShippingCost(5, "Poland");

            Assert.That(cost, Is.EqualTo(10.00m));
        }

        //TC_020 Scenariusz: Ciężka przesyłka zagraniczna(USA)
        //Dane: Waga 15 kg, Kraj "USA".
        //Akcja: CalculateShippingCost(15, "USA")
        //Oczekiwany wynik: 45.00 (15 + 20 + 10).

        [Test]
        public void CalculateCost_should_add_fees_for_heavy_usa_package()
        {
            decimal cost = _shippingService.CalculateShippingCost(15, "USA");

            Assert.That(cost, Is.EqualTo(45.00m));
        }

        //TC_021 Scenariusz: Przesyłka do innego kraju(Europa)
        //Dane: Waga 2 kg, Kraj "Germany".
        //Akcja: CalculateShippingCost(2, "Germany")
        //Oczekiwany wynik: 15.00 (Cena bazowa).

        [Test]
        public void CalculateCost_should_return_base_price_for_standard_eu_package()
        {
            decimal cost = _shippingService.CalculateShippingCost(2, "Germany");

            Assert.That(cost, Is.EqualTo(15.00m));
        }
    }
}
