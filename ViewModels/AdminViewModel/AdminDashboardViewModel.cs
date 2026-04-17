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
        private readonly OrderService _orderService;
        private readonly ItemService _itemService;

        public int TotalCustomers { get; set; }
        public int TotalStores { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItems { get; set; }

        public AdminDashboardViewModel()
        {
            _customerService = new CustomerService();
            _storeService = new StoreService();
            _orderService = new OrderService();
            _itemService = new ItemService();

            LoadData();
        }

        private void LoadData()
        {
            TotalCustomers = _customerService.GetTotalCustomers();
            TotalStores = _storeService.GetTotalStores();       
            TotalOrders = _orderService.GetTotalOrders();
            TotalItems = _itemService.GetTotalItems();
        }
    }
}
