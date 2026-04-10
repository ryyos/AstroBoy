using System;
using System.Collections.Generic;
using System.Text;

using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.Admin
{
    public class AddUserViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Command SaveCommand { get; }

        public AddUserViewModel()
        {
            _userService = new UserService();
            SaveCommand = new Command(OnSave);
        }

        private async void OnSave()
        {
            var newUser = new Customer
            (
                name: Name,
                email: Email,
                password: Password,
                role: "Customer"
            );

            _userService.AddUser(newUser);

            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
