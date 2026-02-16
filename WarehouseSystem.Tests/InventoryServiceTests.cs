using Moq;
using System.Reflection;
using WarehouseSystem.Interfaces;
using WarehouseSystem.Services;

namespace WarehouseSystem.Tests
{
    public class InventoryServiceTests
    {
        private Mock<INotificationService> _mockNotificationService;
        private InventoryService _inventoryService;
        private FieldInfo _fieldInfo;

        [SetUp]
        public void Setup()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _inventoryService = new InventoryService(_mockNotificationService.Object);
            _fieldInfo = typeof(InventoryService).GetField("_stock", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        //TC_009 Scenariusz: Próba pobrania towaru, którego nie ma(Exception)
        //Dane: Produkt ID = 1, Stan = 5, Próba pobrania=10
        //Akcja: _inventoryService.ReduceStock()
        //Oczekiwany wynik: Rzucony wyjątek InvalidOperationException("Not enough stock").

        [Test]
        public void Inventory_should_throw_exception_with_not_enough_stock() 
        {
            int id = 1, quantity = 5, amountToReduce = 10;

            _inventoryService.AddStock(id, quantity);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                
                _inventoryService.ReduceStock(id, amountToReduce)
            );

            Assert.That(exception.Message, Is.EqualTo("Not enough stock."));
        }

        //TC_010 Scenariusz: Pobranie towaru poniżej progu alarmowego(Alert)
        //Dane: Produkt ID = 1, Stan = 10.Pobieramy 8 sztuk(zostaną 2).
        //Akcja: _inventoryService.ReduceStock()
        //Oczekiwany wynik: Metoda ReduceStock powinna wywołać SendLowStockAlert w serwisie powiadomień. powinny zostać tylko dwie sztuki

        [Test]
        public void Inventory_should_call_notification()
        {
            int id = 2, quantity = 10, amountToReduce = 8;

            _inventoryService.AddStock(id, quantity);

            _inventoryService.ReduceStock(id, amountToReduce);

            var stock = (Dictionary<int, int>)_fieldInfo.GetValue(_inventoryService);

            Assert.That(stock[id], Is.EqualTo(2));
            _mockNotificationService.Verify(n => n.SendLowStockAlert(id), Times.Once, "Alert should be send.");
        }
    }
}
