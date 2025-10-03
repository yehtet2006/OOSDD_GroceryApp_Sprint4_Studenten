using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Grocery.Core;

namespace TestCore
{
    public class BoughtProductsServiceTests
    {
        private Mock<IGroceryListItemsRepository> _groceryListItemsRepoMock;
        private Mock<IGroceryListRepository> _groceryListRepoMock;
        private Mock<IClientRepository> _clientRepoMock;
        private Mock<IProductRepository> _productRepoMock;
        private BoughtProductsService _service;

        [SetUp]
        public void Setup()
        {
            _groceryListItemsRepoMock = new Mock<IGroceryListItemsRepository>();
            _groceryListRepoMock = new Mock<IGroceryListRepository>();
            _clientRepoMock = new Mock<IClientRepository>();
            _productRepoMock = new Mock<IProductRepository>();

            _service = new BoughtProductsService(
                _groceryListItemsRepoMock.Object,
                _groceryListRepoMock.Object,
                _clientRepoMock.Object,
                _productRepoMock.Object
            );
        }

        [Test]
        public void Get_ReturnsEmptyList_WhenProductIdIsNull()
        {
            var result = _service.Get(null);
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Get_ReturnsEmptyList_WhenNoGroceryListItemsMatch()
        {
            _groceryListItemsRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>());
            var result = _service.Get(1);
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Get_ReturnsBoughtProducts_WhenDataIsValid()
        {
            var groceryListItem = new GroceryListItem(1, 2, 3, 1);
            _groceryListItemsRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem> { groceryListItem });

            var groceryList = new GroceryList(2, "List", DateOnly.FromDateTime(DateTime.Today), "red", 5);
            _groceryListRepoMock.Setup(r => r.Get(2)).Returns(groceryList);

            var client = new Client(5, "John", "john@email.com", "pw", Role.None);
            _clientRepoMock.Setup(r => r.Get(5)).Returns(client);

            var product = new Product(3, "Milk", 10);
            _productRepoMock.Setup(r => r.Get(3)).Returns(product);

            var result = _service.Get(3);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Client, Is.EqualTo(client));
            Assert.That(result[0].GroceryList, Is.EqualTo(groceryList));
            Assert.That(result[0].Product, Is.EqualTo(product));
        }

        [Test]
        public void Get_SkipsItems_WhenAnyDependencyReturnsNull()
        {
            var groceryListItem = new GroceryListItem(1, 2, 3, 1);
            _groceryListItemsRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem> { groceryListItem });

            // Simulate missing grocery list
            _groceryListRepoMock.Setup(r => r.Get(2)).Returns((GroceryList)null);

            var result = _service.Get(3);
            Assert.IsEmpty(result);
        }
    }
}