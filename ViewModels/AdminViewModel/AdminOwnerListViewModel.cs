using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.Views.VAdmin;

namespace AstroBoy.ViewModels.AdminViewModel
{
    internal class AdminOwnerListViewModel
    {
        public ObservableCollection<Owner> Owners { get; set; }
        public ICommand ViewStoresCommand { get; }

        public AdminOwnerListViewModel()
        {
            ViewStoresCommand = new Command<Owner>(OnViewStores);

            Owners = new ObservableCollection<Owner>(
                new OwnerService().GetAllOwners()
            );
        }

        private async void OnViewStores(Owner owner)
        {
            Console.WriteLine($"CLICK OWNER ID: {owner.Id}");
            Console.WriteLine($"CLICK EMAIL ID: {owner.Email}");
            await Task.Delay(50);
            await Shell.Current.GoToAsync($"{nameof(AdminOwnerStoresPage)}?OwnerId={owner.Id}");
        }
    }
}
