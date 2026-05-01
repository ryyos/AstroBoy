using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.AdminViewModel
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        private readonly CustomerService _customerService;
        private readonly StoreService _storeService;
        private readonly OwnerService _ownerService;
        private readonly ItemService _itemService;

        public int TotalCustomers { get; set; }
        public int TotalStores { get; set; }
        public int TotalOwners { get; set; }
        public int TotalItems { get; set; }

        public AdminDashboardViewModel()
        {
            _customerService = new CustomerService();
            _storeService = new StoreService();
            _ownerService = new OwnerService();
            _itemService = new ItemService();

            LoadData();
        }

        private void LoadData()
        {
            TotalCustomers = _customerService.GetTotalCustomers();
            TotalStores = _storeService.GetTotalStores();
            TotalOwners = _ownerService.GetTotalOwners();
            TotalItems = _itemService.GetTotalItems();
        }
    }
}
