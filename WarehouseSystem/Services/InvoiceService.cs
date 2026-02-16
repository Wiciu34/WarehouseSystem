using WarehouseSystem.Interfaces;
using WarehouseSystem.Models;

namespace WarehouseSystem.Services
{
    public class InvoiceService : IInvoiceService
    {
        public Invoice GenerateInvoice(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.TotalAmount <= 0) throw new InvalidOperationException("Cannot generate invoice for zero amount.");

            return new Invoice
            {
                InvoiceNumber = $"INV-{DateTime.Now.Year}-{order.Id}",
                OrderId = order.Id,
                FinalAmount = order.TotalAmount,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
