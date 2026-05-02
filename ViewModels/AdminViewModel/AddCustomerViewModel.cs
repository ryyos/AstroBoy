using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.AdminViewModel
{
    public class AddCustomerViewModel : BaseViewModel
    {
        private readonly CustomerService _customerService;

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Command SaveCommand { get; }

        public AddCustomerViewModel()
        {
            _customerService = new CustomerService();
            SaveCommand = new Command(OnSave);
        }

        private async void OnSave()
        {
            var newCustomer = new Customer
            (
                name: Name,
                email: Email,
                password: Password,
                role: "customer"
            );

            _customerService.AddCustomer(newCustomer);
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
