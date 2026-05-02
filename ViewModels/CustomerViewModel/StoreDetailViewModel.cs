using System.Collections.ObjectModel;
using System.Windows.Input;
using AstroBoy.Utils;
using AstroBoy.ViewModels.Base;
using AstroBoy.Views.VCustomer;

namespace AstroBoy.ViewModels.CustomerViewModel;

/// <summary>
/// ViewModel untuk StoreDetailPage.
/// Menerima StoreDisplay dari navigasi, mengelola produk toko tersebut
/// dan sinkronisasi keranjang via CartBag.
/// </summary>
public class StoreDetailViewModel : BaseViewModel
{
    private readonly StoreDisplay _store;

    // ── Binding ke UI ─────────────────────────────────────────────────────────
    public string StoreName => _store.StoreName;
    public string StoreImage => _store.StoreImage;
    public string ProductSectionLabel => $"Produk Tersedia ({_store.ProductCount})";

    // Produk ditampilkan langsung dari StoreDisplay (observable melalui ProductDisplay)
    public IReadOnlyList<ProductDisplay> Products => _store.Products;

    // ── CartCount badge (sync dengan CartBag) ─────────────────────────────────
    private int _cartCount;

    public int CartCount
    {
        get => _cartCount;
        set
        {
            _cartCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasCartItems));
            OnPropertyChanged(nameof(CartBadgeLabel));
        }
    }

    public bool HasCartItems => _cartCount > 0;
    public string CartBadgeLabel => _cartCount.ToString();

    // ── Commands ──────────────────────────────────────────────────────────────
    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand OpenProductDetailCommand { get; }

    public StoreDetailViewModel(StoreDisplay store)
    {
        _store = store;

        AddToCartCommand = new Command<ProductDisplay>(AddToCart);
        RemoveFromCartCommand = new Command<ProductDisplay>(RemoveFromCart);
        GoToCartCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(CartPage)));
        OpenProductDetailCommand = new Command<ProductDisplay>(async product =>
        {
            if (product is null) return;
            await Shell.Current.Navigation.PushAsync(new ProductDetailPage(product));
        });

        // Sinkronkan qty awal dari CartBag
        RefreshFromBag();
    }

    // ── Tambah produk ke keranjang ────────────────────────────────────────────
    private void AddToCart(ProductDisplay product)
    {
        if (product is null) return;
        if (product.Quantity >= product.Stock) return;

        product.Quantity++;
        CartBag.Add(product.ItemId, product.ProductName, product.StoreName, product.StoreId,
                    product.Price, product.ImageSource, product.Stock);
        CartCount = CartBag.TotalCount;
    }

    // ── Kurangi produk dari keranjang ─────────────────────────────────────────
    private void RemoveFromCart(ProductDisplay product)
    {
        if (product is null) return;
        if (product.Quantity <= 0) return;

        product.Quantity--;
        CartBag.Decrement(product.ProductName, product.StoreName);
        CartCount = CartBag.TotalCount;
    }

    // ── Sinkronkan ulang qty dari CartBag ─────────────────────────────────────
    /// <summary>
    /// Dipanggil dari StoreDetailPage.OnAppearing agar qty produk
    /// selalu sesuai dengan CartBag (misal setelah kembali dari CartPage).
    /// </summary>
    public void RefreshFromBag()
    {
        foreach (var product in _store.Products)
        {
            var entry = CartBag.Items.FirstOrDefault(
                e => e.ProductName == product.ProductName && e.StoreName == product.StoreName);
            product.Quantity = entry?.Qty ?? 0;
        }
        CartCount = CartBag.TotalCount;
    }
}
