using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;

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
                new OwnerService()
                    .GetAll().Where(u => u.Role == "Owner")
            );

            ViewStoresCommand = new Command<Owner>(OnViewStores);
        }

        private async void OnViewStores(Owner owner)
        {
            await Shell.Current.GoToAsync($"OwnerStoresPage?OwnerId={owner.Id}");
        }
    }
}
