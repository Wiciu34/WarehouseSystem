using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly ICustomerValidator _customerValidator;
        private readonly IInventoryService _inventoryService;
        private readonly IPricingService _pricingService;
        private readonly IDiscountService _discountService;
        private readonly ITaxService _taxService;
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;
        private readonly IShippingService _shippingService;

        public OrderProcessor(
           ICustomerValidator customerValidator,
           IInventoryService inventoryService,
           IPricingService pricingService,
           IDiscountService discountService,
           ITaxService taxService,
           IOrderRepository orderRepository,
           INotificationService notificationService,
           IShippingService shippingService)
        {
            _customerValidator = customerValidator;
            _inventoryService = inventoryService;
            _pricingService = pricingService;
            _discountService = discountService;
            _taxService = taxService;
            _orderRepository = orderRepository;
            _notificationService = notificationService;
            _shippingService = shippingService;

        }
        public Order ProcessOrder(Customer customer, List<OrderItem> items, ShippingDetails shippingDetails)
        {
            if (!_customerValidator.Validate(customer))
            {
                throw new ArgumentException("Invalid customer.");
            }

            foreach (var item in items)
            {
                if (!_inventoryService.IsAvailable(item.Product.Id, item.Quantity))
                {
                    throw new InvalidOperationException($"Product {item.Product.Name} is out of stock.");
                }
            }

            decimal subtotal = _pricingService.CalculateSubtotal(items);
            decimal afterDiscount = _discountService.ApplyDiscount(customer, subtotal);
            decimal tax = _taxService.CalculateTax(afterDiscount, 0.23m);
            decimal shippingCost = _shippingService.CalculateShippingCost(shippingDetails.TotalWeight, shippingDetails.DestinationCountry);
            decimal total = afterDiscount + tax + shippingCost;

            var order = new Order
            {
                Customer = customer,
                Items = items,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                IsPaid = false,
            };

            foreach (var item in items)
            {
                _inventoryService.ReduceStock(item.Product.Id, item.Quantity);
            }

            _orderRepository.Save(order);

            _notificationService.SendOrderConfirmation(customer, order);

            return order;
        }
    }
}
