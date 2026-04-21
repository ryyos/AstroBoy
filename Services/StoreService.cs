using AstroBoy.Models;

namespace AstroBoy.Services;

public class StoreService
{
    private static readonly List<Store> _stores = new()
    {
        new Store
        {
            StoreId = "store-001",
            OwnerId = "owner",
            Name = "Toko Elektronik Jaya",
            Address = "Jl. Sudirman No.1",
            Phone = "08123456789",
            Items = new List<Item>
            {
                new Item { Id = Guid.NewGuid(), Name = "Laptop ASUS", Price = 8500000, Stock = 10, Category = "Elektronik", StoreId = "store-001" },
                new Item { Id = Guid.NewGuid(), Name = "Mouse Wireless", Price = 150000, Stock = 50, Category = "Aksesoris", StoreId = "store-001" },
            }
        },
        new Store
        {
            StoreId = "store-002",
            OwnerId = "owner",
            Name = "Toko Fashion Keren",
            Address = "Jl. Gatot Subroto No.5",
            Phone = "08987654321",
            Items = new List<Item>
            {
                new Item { Id = Guid.NewGuid(), Name = "Kaos Polos", Price = 75000, Stock = 100, Category = "Pakaian", StoreId = "store-002" },
                new Item { Id = Guid.NewGuid(), Name = "Celana Jeans", Price = 250000, Stock = 30, Category = "Pakaian", StoreId = "store-002" },
            }
        }
    };

    public int GetTotalStores() => _stores.Count;

    public List<Store> GetAllStores() => _stores.ToList();

    public List<Store> GetStoresByOwner(string ownerId)
        => _stores.Where(s => s.OwnerId == ownerId).ToList();

    public Store? GetStoreById(string storeId)
        => _stores.FirstOrDefault(s => s.StoreId == storeId);

    public void AddItem(string storeId, Item item)
        => GetStoreById(storeId)?.Items.Add(item);

    public void UpdateItem(Item updatedItem)
    {
        foreach (var store in _stores)
        {
            var item = store.Items.FirstOrDefault(i => i.Id == updatedItem.Id);
            if (item == null) continue;
            item.Name = updatedItem.Name;
            item.Price = updatedItem.Price;
            item.Stock = updatedItem.Stock;
            item.Category = updatedItem.Category;
            return;
        }
    }

    public void DeleteItem(Guid itemId, string storeId)
    {
        var store = GetStoreById(storeId);
        var item = store?.Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null) store!.Items.Remove(item);
    }
}
