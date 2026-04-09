using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AstroBoy.Models;
using AstroBoy.Services;

namespace AstroBoy.ViewModels.VAdmin
{
    internal class AdminUsersViewModel
    {
        private readonly UserService _userService;
        public ObservableCollection<User> Users { get; set; }

        public AdminUsersViewModel()
        {
            _userService = new UserService();
            Users = new ObservableCollection<User>();

            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = _userService.GetAllUsers();

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }
    }
}
