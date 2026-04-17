using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;

namespace AstroBoy.ViewModels.AdminViewModel
{
    internal class AdminCustomerViewModel
    {
        public ICommand DeleteCustomerCommand { get; }
        public ICommand EditCustomerCommand { get; }

        private readonly CustomerService _customerService;
        public ObservableCollection<Customer> Customers { get; set; }

        public AdminCustomerViewModel()
        {
            _customerService = new CustomerService();
            Customers = new ObservableCollection<Customer>();

            DeleteCustomerCommand = new Command<Customer>(OnDeleteCustomer);
            EditCustomerCommand = new Command<User>(OnEditCustomer);

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var customers = _customerService.GetAllCustomers();

            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }


        private async void OnDeleteCustomer(User user)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Delete this customer?", "Yes", "No");

            if (!confirm) return;

            _customerService.Delete(user.Id);
            LoadCustomers();
        }

        private async void OnEditCustomer(User user)
        {
            await Shell.Current.GoToAsync($"EditCustomerPage?Id={user.Id}");
        }

    }
}
