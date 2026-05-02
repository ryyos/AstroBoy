using System.Collections.ObjectModel;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.OwnerViewModel;

public class OwnerStoreDetailViewModel : BaseViewModel
{
    private readonly StoreService _storeService;

    public Store Store { get; }
    public ObservableCollection<Item> Items { get; } = new();

    public OwnerStoreDetailViewModel(Store store)
    {
        _storeService = new StoreService();
        Store = store;
        RefreshItems();
    }

    public void RefreshItems()
    {
        Items.Clear();
        foreach (var item in _storeService.GetFreshItemsByStoreId(Store.StoreId!))
            Items.Add(item);
    }

    public void DeleteItem(Item item)
    {
        _storeService.DeleteItem(item.Id, Store.StoreId);
        Items.Remove(item);
    }
}
