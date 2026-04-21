using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AstroBoy.Models;
using AstroBoy.Services;

namespace AstroBoy.ViewModels.AdminViewModel
{
    [QueryProperty(nameof(OwnerId), "OwnerId")]
    public class AdminOwnerStoresViewModel
    {
        private readonly StoreService _service;

        public string OwnerId { get; set; } = string.Empty;
        public ObservableCollection<Store> Stores { get; set; }

        public AdminOwnerStoresViewModel()
        {
            _service = new StoreService();
            Stores = new ObservableCollection<Store>();
        }

        public void LoadData()
        {
            var data = _service.GetAllStores()
                               .Where(s => s.OwnerId == OwnerId);

            Stores.Clear();
            foreach (var item in data)
                Stores.Add(item);
        }
    }
}
