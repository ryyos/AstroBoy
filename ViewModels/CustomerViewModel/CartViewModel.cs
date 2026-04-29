using System.Collections.ObjectModel;
using System.Windows.Input;
using AstroBoy.Utils;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.CustomerViewModel;

/// <summary>
/// Merepresentasikan satu item di CartPage — observable agar Qty dan subtotal update realtime.
/// </summary>
public class CartItemViewModel : BaseViewModel
{
    private int _qty;

    public string ProductName { get; }
    public string StoreName { get; }
    public decimal Price { get; }
    public string ImageSource { get; }
    public int MaxStock { get; }

    public int Qty
    {
        get => _qty;
        set
        {
            _qty = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(QtyLabel));
            OnPropertyChanged(nameof(SubtotalFormatted));
        }
    }

    public string QtyLabel => _qty.ToString();
    public string PriceFormatted => $"Rp {Price:N0}";
    public string SubtotalFormatted => $"Rp {Price * Qty:N0}";

    public CartItemViewModel(CartBagEntry entry)
    {
        ProductName = entry.ProductName;
        StoreName = entry.StoreName;
        Price = entry.Price;
        ImageSource = entry.ImageSource;
        MaxStock = entry.MaxStock;
        _qty = entry.Qty;
    }
}

/// <summary>
/// ViewModel untuk CartPage.
/// Membaca data dari CartBag, mengelola perubahan qty per item, dan proses checkout.
/// </summary>
public class CartViewModel : BaseViewModel
{
    public ObservableCollection<CartItemViewModel> CartItems { get; } = new();

    // ── Total harga ───────────────────────────────────────────────────────────
    private decimal _total;

    public decimal Total
    {
        get => _total;
        set
        {
            _total = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalFormatted));
        }
    }

    public string TotalFormatted => $"Rp {Total:N0}";

    // ── State kosong/isi ──────────────────────────────────────────────────────
    private bool _hasItems;

    public bool HasItems
    {
        get => _hasItems;
        set
        {
            _hasItems = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    public bool IsEmpty => !HasItems;

    // ── Commands ──────────────────────────────────────────────────────────────
    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand CheckoutCommand { get; }

    public CartViewModel()
    {
        IncrementCommand = new Command<CartItemViewModel>(Increment);
        DecrementCommand = new Command<CartItemViewModel>(Decrement);
        RemoveCommand = new Command<CartItemViewModel>(Remove);
        CheckoutCommand = new Command(async () => await Checkout());

        LoadFromBag();
    }

    // ── Load data dari CartBag ────────────────────────────────────────────────
    private void LoadFromBag()
    {
        CartItems.Clear();
        foreach (var entry in CartBag.Items)
            CartItems.Add(new CartItemViewModel(entry));

        RecalcTotal();
        HasItems = CartItems.Count > 0;
    }

    private void RecalcTotal() =>
        Total = CartItems.Sum(i => i.Price * i.Qty);

    // ── Increment qty (+1) ────────────────────────────────────────────────────
    private void Increment(CartItemViewModel item)
    {
        if (item is null || item.Qty >= item.MaxStock) return;

        item.Qty++;
        CartBag.Add(item.ProductName, item.StoreName, item.Price,
                    item.ImageSource, item.MaxStock);
        RecalcTotal();
    }

    // ── Decrement qty (-1, min 1 — hapus via tombol Hapus) ───────────────────
    private void Decrement(CartItemViewModel item)
    {
        if (item is null || item.Qty <= 1) return;

        item.Qty--;
        CartBag.Decrement(item.ProductName, item.StoreName);
        RecalcTotal();
    }

    // ── Hapus item sepenuhnya ─────────────────────────────────────────────────
    private void Remove(CartItemViewModel item)
    {
        if (item is null) return;

        CartBag.Remove(item.ProductName, item.StoreName);
        CartItems.Remove(item);
        RecalcTotal();
        HasItems = CartItems.Count > 0;
    }

    // ── Checkout ──────────────────────────────────────────────────────────────
    private async Task Checkout()
    {
        if (!HasItems) return;

        bool confirmed = await Shell.Current.DisplayAlertAsync(
            "Konfirmasi Pesanan",
            $"Total: {TotalFormatted}\nLanjutkan pesanan ini?",
            "Ya, Checkout",
            "Batal");

        if (!confirmed) return;

        // Buat record order dari isi cart saat ini, sebelum cart dikosongkan
        var record = new OrderRecord
        {
            Items = CartBag.Items.Select(i => new OrderItemRecord
            {
                ProductName = i.ProductName,
                StoreName = i.StoreName,
                ImageSource = i.ImageSource,
                Price = i.Price,
                Qty = i.Qty
            }).ToList(),
            Total = CartBag.Items.Sum(i => i.Price * i.Qty)
        };

        // Simpan ke riwayat order
        OrderHistory.Add(record);

        CartBag.Clear();
        CartItems.Clear();
        RecalcTotal();
        HasItems = false;

        await Shell.Current.DisplayAlertAsync(
            "Pesanan Berhasil",
            "Terima kasih! Pesanan Anda sedang diproses.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}
