
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            if (!productId.HasValue)
            {
                return new List<BoughtProducts>();
            }

            var items = _groceryListItemsRepository
                .GetAll()
                .Where(i => i.ProductId == productId.Value)
                .ToList();
            var result = new List<BoughtProducts>();
            foreach (var item in items)
            {
                var groceryList = _groceryListRepository.Get(item.GroceryListId);
                if (groceryList == null) continue;

                var client = _clientRepository.Get(groceryList.ClientId);
                if (client == null) continue;
                
                var product = _productRepository.Get(item.ProductId);
                if (product == null) continue;
                
                result.Add(new BoughtProducts(client, groceryList, product));
            }

            return result;
        }
    }
}
