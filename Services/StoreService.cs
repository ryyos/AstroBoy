using Database;
using AstroBoy.Models;

namespace AstroBoy.Services;

public class StoreService
{
    private DatabaseContext db;
    private static List<Store>? _stores;

    public StoreService()
    {
        db = new DatabaseContext();
        _stores = db.GetAllStores();
    }

    public int GetTotalStores() => _stores!.Count;

    public List<Store> GetAllStores() => _stores!.ToList();

    public List<Store> GetStoresByOwner(string ownerId)
        => _stores!.Where(s => s.OwnerId == ownerId).ToList();

    public Store? GetStoreById(string storeId)
        => _stores!.FirstOrDefault(s => s.StoreId == storeId);

    // Reload items langsung dari DB (dipakai setelah form add/edit item)
    public List<Item> GetFreshItemsByStoreId(string storeId)
    {
        var freshItems = db.GetItemsForStore(storeId);
        var store = GetStoreById(storeId);
        if (store != null) store.Items = freshItems;
        return freshItems;
    }

    public void AddItem(string storeId, Item item)
    {
        db.InsertItem(item);
        GetStoreById(storeId)?.Items!.Add(item);
    }

    public void UpdateItem(Item updatedItem)
    {
        db.UpdateItem(updatedItem);
        foreach (var store in _stores!)
        {
            var item = store!.Items!.FirstOrDefault(i => i.Id == updatedItem.Id);
            if (item == null) continue;
            item.Name = updatedItem.Name;
            item.Price = updatedItem.Price;
            item.Stock = updatedItem.Stock;
            item.Category = updatedItem.Category;
            return;
        }
    }

    public void DeleteItem(string itemId, string storeId)
    {
        db.DeleteItem(itemId);
        var store = GetStoreById(storeId);
        var item = store?.Items!.FirstOrDefault(i => i.Id == itemId);
        if (item != null) store!.Items!.Remove(item);
    }

    public void AddStore(Store store)
    {
        db.InsertStore(store);
        _stores!.Add(store);
    }

    public void UpdateStore(Store updatedStore)
    {
        db.UpdateStore(updatedStore);
        var store = _stores!.FirstOrDefault(s => s.StoreId == updatedStore.StoreId);
        if (store == null) return;
        store.Name = updatedStore.Name;
        store.Address = updatedStore.Address;
        store.Phone = updatedStore.Phone;
    }

    public void DeleteStore(string storeId)
    {
        db.DeleteStore(storeId);
        var store = _stores!.FirstOrDefault(s => s.StoreId == storeId);
        if (store != null) _stores!.Remove(store);
    }
}
