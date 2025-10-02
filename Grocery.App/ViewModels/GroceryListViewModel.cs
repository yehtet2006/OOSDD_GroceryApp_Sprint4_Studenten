using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using Grocery.Core;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        private readonly IClientService _clientService;

        public Client? Client { get; set; } // <- deze property is nodig voor binding
        public GroceryListViewModel(IGroceryListService groceryListService, IClientService clientService) 
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _clientService = clientService;
            Client = clientService.Get(3);
            GroceryLists = new(_groceryListService.GetAll());
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }

        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            var currentClient = _clientService.Get(3);

            if (currentClient == null)
                return;

            if (currentClient.Role == Role.Admin)
            { 
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }
    }
}
