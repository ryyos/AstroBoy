using System.Collections.ObjectModel;
using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.CustomerViewModel;

/// <summary>
/// Merepresentasikan satu produk yang ditampilkan di halaman utama customer,
/// termasuk informasi toko asal dan jumlah item di keranjang.
/// </summary>
public class ProductDisplayItem : BaseViewModel
{
    private int _qty;

    public Item Item { get; }
    public string StoreName { get; }

    public string Name => Item.Name;
    public string PriceFormatted => $"Rp {Item.Price:N0}";
    public string StockInfo => $"Stok: {Item.Stock}";

    public int Qty
    {
        get => _qty;
        set
        {
            _qty = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(QtyLabel));
        }
    }

    public string QtyLabel => _qty.ToString();

    public ProductDisplayItem(Item item, string storeName)
    {
        Item = item;
        StoreName = storeName;
        _qty = 0;
    }
}

/// <summary>
/// ViewModel untuk CustomerHomePage.
/// Mengelola daftar semua produk dari semua toko dan logika keranjang belanja.
/// </summary>
public class CartViewModel : BaseViewModel
{
    private readonly StoreService _storeService;

    public ObservableCollection<ProductDisplayItem> Products { get; } = new();

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }

    public CartViewModel()
    {
        _storeService = new StoreService();

        AddToCartCommand = new Command<ProductDisplayItem>(AddToCart);
        RemoveFromCartCommand = new Command<ProductDisplayItem>(RemoveFromCart);

        LoadProducts();
    }

    private void LoadProducts()
    {
        var stores = _storeService.GetAllStores();
        foreach (var store in stores)
        {
            foreach (var item in store.Items)
            {
                Products.Add(new ProductDisplayItem(item, store.Name));
            }
        }
    }

    private void AddToCart(ProductDisplayItem product)
    {
        if (product == null) return;
        if (product.Qty >= product.Item.Stock) return;
        product.Qty++;
    }

    private void RemoveFromCart(ProductDisplayItem product)
    {
        if (product == null) return;
        if (product.Qty <= 0) return;
        product.Qty--;
    }
}
