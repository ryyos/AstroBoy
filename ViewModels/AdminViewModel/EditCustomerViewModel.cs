using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.AdminViewModel
{
    [QueryProperty(nameof(UserId), "Id")]
    class EditCustomerViewModel : BaseViewModel
    {
        private readonly CustomerService _service;
        public ICommand SaveCommand => new Command(OnSave);
        public string UserId { get; set; }
        public Customer Customer { get; set; }

        public EditCustomerViewModel(CustomerService service)
        {
            _service = service;
        }

        public void LoadData()
        {
            Customer = _service.GetById(UserId);
        }


        private async void OnSave()
        {
            _service.Update(Customer);
            await Shell.Current.GoToAsync("..");
        }
    }
}
