using System.Collections.ObjectModel;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;
using OwnerUser = AstroBoy.Models.Owner;

namespace AstroBoy.ViewModels.OwnerViewModel;

public class OwnerDashboardViewModel : BaseViewModel
{
    private readonly StoreService _storeService;
    private readonly OwnerUser _owner;

    public string WelcomeMessage => $"Selamat datang, {_owner.Name}";
    public string BalanceFormatted => $"Rp {_owner.Balance:N0}";
    public ObservableCollection<Store> Stores { get; private set; }

    public OwnerDashboardViewModel(OwnerUser owner)
    {
        _owner = owner;
        _storeService = new StoreService();
        Stores = new ObservableCollection<Store>(_storeService.GetStoresByOwner(_owner.Id));
    }

    public void RefreshStores()
    {
        Stores.Clear();
        foreach (var s in _storeService.GetStoresByOwner(_owner.Id))
            Stores.Add(s);
    }

    public void DeleteStore(Store store)
    {
        _storeService.DeleteStore(store.StoreId);
        Stores.Remove(store);
    }
}
