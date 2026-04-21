using System.Collections.ObjectModel;
using System.Windows.Input;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.CustomerViewModel;

// ── Helper: chip filter toko ──────────────────────────────────────────────────
/// <summary>
/// Merepresentasikan satu chip filter toko di bagian atas halaman.
/// Mendukung binding warna berdasarkan status IsSelected.
/// </summary>
public class StoreFilterItem : BaseViewModel
{
    private bool _isSelected;

    public string Name { get; init; } = string.Empty;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ChipBackground));
            OnPropertyChanged(nameof(ChipTextColor));
        }
    }

    // Chip aktif = biru (#3E64FF), tidak aktif = abu (#E5E7EB)
    public Color ChipBackground => _isSelected
        ? Color.FromArgb("#3E64FF")
        : Color.FromArgb("#E5E7EB");

    public Color ChipTextColor => _isSelected
        ? Colors.White
        : Color.FromArgb("#1F2937");
}

// ── Helper: data produk untuk tampilan UI ─────────────────────────────────────
/// <summary>
/// Merepresentasikan satu produk yang ditampilkan di grid.
/// Qty (jumlah di keranjang) bersifat observable agar badge dan counter update realtime.
/// </summary>
public class ProductDisplay : BaseViewModel
{
    private int _quantity;

    public string ProductName { get; init; } = string.Empty;
    public string StoreName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageSource { get; init; } = string.Empty;
    public int Stock { get; init; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(QuantityLabel));
        }
    }

    // Label counter di antara tombol + dan -
    public string QuantityLabel => _quantity.ToString();

    // Harga terformat: Rp 8.000.000
    public string PriceFormatted => $"Rp {Price:N0}";
}

// ── StoreViewModel ────────────────────────────────────────────────────────────
/// <summary>
/// ViewModel utama untuk CustomerHomePage.
/// Mengelola daftar produk, filter toko, pencarian, dan logika keranjang.
/// </summary>
public class StoreViewModel : BaseViewModel
{
    // ── Data master semua produk (in-memory) ──────────────────────────────────
    private readonly List<ProductDisplay> _allProducts = new();

    // ── Collections yang di-bind ke UI ───────────────────────────────────────
    public ObservableCollection<ProductDisplay> FilteredProducts { get; } = new();
    public ObservableCollection<StoreFilterItem> StoreFilters { get; } = new();

    // ── Search ────────────────────────────────────────────────────────────────
    private string _searchQuery = string.Empty;

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged();
            ApplyFilter(); // filter realtime saat mengetik
        }
    }

    // ── Filter toko yang dipilih ──────────────────────────────────────────────
    private string _selectedStore = "Semua";

    public string SelectedStore
    {
        get => _selectedStore;
        set
        {
            _selectedStore = value;
            OnPropertyChanged();
            UpdateChipSelection(); // update warna chip
            ApplyFilter();
        }
    }

    // ── Cart badge ────────────────────────────────────────────────────────────
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

    // ── Toast notifikasi "Ditambahkan ke keranjang" ───────────────────────────
    private bool _isToastVisible;
    private string _toastMessage = string.Empty;

    public bool IsToastVisible
    {
        get => _isToastVisible;
        set { _isToastVisible = value; OnPropertyChanged(); }
    }

    public string ToastMessage
    {
        get => _toastMessage;
        set { _toastMessage = value; OnPropertyChanged(); }
    }

    // ── Commands ──────────────────────────────────────────────────────────────
    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand SelectStoreFilterCommand { get; }

    public StoreViewModel()
    {
        // Inisialisasi commands
        AddToCartCommand = new Command<ProductDisplay>(AddToCart);
        RemoveFromCartCommand = new Command<ProductDisplay>(RemoveFromCart);
        SelectStoreFilterCommand = new Command<string>(SelectStoreFilter);
        GoToCartCommand = new Command(async () =>
            await Shell.Current.GoToAsync("CartPage"));

        // Setup awal
        LoadDummyData();
        BuildStoreFilters();
        ApplyFilter();
    }

    // ── Dummy data ────────────────────────────────────────────────────────────
    /// <summary>
    /// Data produk in-memory. Akan diganti koneksi service/DB di iterasi berikutnya.
    /// </summary>
    private void LoadDummyData()
    {
        _allProducts.AddRange(new[]
        {
            // Toko Elektronik
            new ProductDisplay
            {
                ProductName = "Laptop ASUS",
                StoreName   = "Toko Elektronik",
                Price       = 8_000_000,
                ImageSource = "asus_leptop.png",
                Stock       = 10
            },
            new ProductDisplay
            {
                ProductName = "Mouse Wireless",
                StoreName   = "Toko Elektronik",
                Price       = 150_000,
                ImageSource = "mouse_warlees.png",
                Stock       = 50
            },
            // Toko Fashion
            new ProductDisplay
            {
                ProductName = "Jeans Pria",
                StoreName   = "Toko Fashion",
                Price       = 250_000,
                ImageSource = "jeans.png",
                Stock       = 30
            },
            new ProductDisplay
            {
                ProductName = "Kaos Polos",
                StoreName   = "Toko Fashion",
                Price       = 85_000,
                ImageSource = "kaos_polos.png",
                Stock       = 100
            },
        });
    }

    // ── Build chip filter ─────────────────────────────────────────────────────
    /// <summary>
    /// Membangun list chip filter: "Semua" + nama unik setiap toko dari data produk.
    /// </summary>
    private void BuildStoreFilters()
    {
        // Chip "Semua" selalu ada di posisi pertama dan aktif by default
        StoreFilters.Add(new StoreFilterItem { Name = "Semua", IsSelected = true });

        var storeNames = _allProducts.Select(p => p.StoreName).Distinct();
        foreach (var name in storeNames)
            StoreFilters.Add(new StoreFilterItem { Name = name, IsSelected = false });
    }

    // ── Filter logic ──────────────────────────────────────────────────────────
    /// <summary>
    /// Menerapkan filter berdasarkan SelectedStore DAN SearchQuery secara bersamaan.
    /// </summary>
    private void ApplyFilter()
    {
        var result = _allProducts.AsEnumerable();

        // Filter berdasarkan toko yang dipilih
        if (_selectedStore != "Semua")
            result = result.Where(p => p.StoreName == _selectedStore);

        // Filter berdasarkan kata kunci (contains, ignore case)
        if (!string.IsNullOrWhiteSpace(_searchQuery))
            result = result.Where(p =>
                p.ProductName.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));

        FilteredProducts.Clear();
        foreach (var product in result)
            FilteredProducts.Add(product);
    }

    // ── Update warna chip ─────────────────────────────────────────────────────
    private void UpdateChipSelection()
    {
        foreach (var chip in StoreFilters)
            chip.IsSelected = chip.Name == _selectedStore;
    }

    // ── Select toko dari chip ─────────────────────────────────────────────────
    private void SelectStoreFilter(string storeName)
    {
        SelectedStore = storeName;
    }

    // ── Cart: tambah produk ───────────────────────────────────────────────────
    private async void AddToCart(ProductDisplay product)
    {
        if (product is null) return;
        if (product.Quantity >= product.Stock) return; // batas stok

        product.Quantity++;
        CartCount++;

        // Tampilkan toast "Ditambahkan ke keranjang" selama 2 detik
        ToastMessage = $"✓ {product.ProductName} ditambahkan ke keranjang";
        IsToastVisible = true;
        await Task.Delay(2000);
        IsToastVisible = false;
    }

    // ── Cart: kurangi produk ──────────────────────────────────────────────────
    private void RemoveFromCart(ProductDisplay product)
    {
        if (product is null) return;
        if (product.Quantity <= 0) return; // minimum 0

        product.Quantity--;
        CartCount--;
    }
}
