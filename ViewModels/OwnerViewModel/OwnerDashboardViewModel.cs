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
    public List<Store> Stores { get; private set; }

    public OwnerDashboardViewModel(OwnerUser owner)
    {
        _owner = owner;
        _storeService = new StoreService();
        Stores = _storeService.GetStoresByOwner(_owner.Id);
    }
}
