using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.VAdmin
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly StoreService _storeService;
        private readonly OrderService _orderService;
        private readonly ItemService _itemService;

        public int TotalUsers { get; set; }
        public int TotalStores { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItems { get; set; }

        public AdminDashboardViewModel()
        {
            _userService = new UserService();
            _storeService = new StoreService();
            _orderService = new OrderService();
            _itemService = new ItemService();

            LoadData();
        }

        private void LoadData()
        {
            TotalUsers = _userService.GetTotalUsers();
            TotalStores = _storeService.GetTotalStores();
            TotalOrders = _orderService.GetTotalOrders();
            TotalItems = _itemService.GetTotalItems();
        }
    }
}
