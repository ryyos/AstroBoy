using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.Views.VAdmin;

[QueryProperty(nameof(OwnerId), "OwnerId")]
public class AdminOwnerStoresViewModel
{
    private readonly StoreService _service;
    private readonly ItemService _itemService;

    private string ownerId;
    public int ItemCount { get; set; }
    public string OwnerId
    {
        get => ownerId;
        set
        {
            ownerId = value;
            System.Diagnostics.Debug.WriteLine($"OwnerId SET: {ownerId}");
            LoadData();
        }
    }

    public ObservableCollection<Store> Stores { get; set; }

    public ICommand ViewItemsCommand => new Command<Store>(async (store) =>
    {
        await Shell.Current.GoToAsync($"{nameof(AdminStoreItemsPage)}?StoreId={store.StoreId}");
    });

    public AdminOwnerStoresViewModel()
    {
        Console.WriteLine($"OwnerId: {OwnerId}");
        _service = new StoreService();
        _itemService = new ItemService();
        Stores = new ObservableCollection<Store>();
    }

    public void LoadData()
    {
        var data = _service.GetStoresByOwner(OwnerId);

        Stores.Clear();
        foreach (var item in data)
        {
            item.ItemCount = _itemService.GetItemsByStore(item.StoreId!).Count;
            Stores.Add(item);
        }

    }
}
