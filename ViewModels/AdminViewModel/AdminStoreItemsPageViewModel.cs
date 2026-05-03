using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Views.VAdmin;

namespace AstroBoy.ViewModels.AdminViewModel
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using AstroBoy.Services;
    using AstroBoy.ViewModels.Base;

    [QueryProperty(nameof(StoreId), "StoreId")]
    public class AdminStoreItemsPageViewModel : BaseViewModel
    {
        private readonly ItemService _itemService;

        public ObservableCollection<ItemViewModel> Items { get; set; } = new();

        public AdminStoreItemsPageViewModel()
        {
            _itemService = new ItemService();
        }

        private string _storeId;
        public string StoreId
        {
            get => _storeId;
            set
            {
                _storeId = value;
                LoadItems();
            }
        }

        private void LoadItems()
        {
            var data = _itemService.GetAllItems()
                                   .Where(i => i.StoreId == StoreId);

            Items.Clear();

            foreach (var item in data)
                Items.Add(new ItemViewModel(item));
        }
    }
}
