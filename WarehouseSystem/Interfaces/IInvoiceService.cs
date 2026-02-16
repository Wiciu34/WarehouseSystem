using WarehouseSystem.Models;

namespace WarehouseSystem.Interfaces
{
    public interface IInvoiceService
    {
        Invoice GenerateInvoice(Order order);
    }
}
