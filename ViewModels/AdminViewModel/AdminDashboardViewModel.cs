using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;
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
        private readonly DashboardService _dashboardService;

        public int TotalCustomers { get; set; }
        public int TotalStores { get; set; }
        public int TotalOwners { get; set; }
        public int TotalItems { get; set; }


        public string TodaySummary { get; set; }

        public string TopStoreName { get; set; }
        public string TopStoreStats { get; set; }

        public string TopOwnerName { get; set; }
        public string TopOwnerStats { get; set; }

        public List<Item> RecentItems { get; set; }

        public AdminDashboardViewModel()
        {
            _customerService = new CustomerService();
            _storeService = new StoreService();
            _ownerService = new OwnerService();
            _itemService = new ItemService();
            _dashboardService = new DashboardService(
                _itemService, _storeService, _ownerService
            );

            LoadData();
        }

        private void LoadData()
        {
            TotalCustomers = _customerService.GetTotalCustomers();
            TotalStores = _storeService.GetTotalStores();
            TotalOwners = _ownerService.GetTotalOwners();
            TotalItems = _itemService.GetTotalItems();

            TodaySummary = _dashboardService.GetTodaySummary();

            var topStore = _dashboardService.GetTopStore();
            TopStoreName = topStore.name;
            TopStoreStats = topStore.stats;

            var topOwner = _dashboardService.GetTopOwner();
            TopOwnerName = topOwner.name;
            TopOwnerStats = topOwner.stats;
        }
    }
}
