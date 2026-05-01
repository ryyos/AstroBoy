using System.Collections.ObjectModel;
using System.Windows.Input;
using AstroBoy.Utils;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.CustomerViewModel;

/// <summary>
/// ViewModel untuk OrderHistoryPage.
/// Membaca dari OrderHistory dan menampilkan list order terbaru di atas.
/// </summary>
public class OrderViewModel : BaseViewModel
{
    // ── Daftar order (terbaru di atas) ────────────────────────────────────────
    private ObservableCollection<OrderRecord> _orders = new();

    public ObservableCollection<OrderRecord> Orders
    {
        get => _orders;
        private set
        {
            _orders = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasOrders));
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    // ── State kosong / isi ────────────────────────────────────────────────────
    public bool HasOrders => Orders.Count > 0;
    public bool IsEmpty => Orders.Count == 0;

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>Tombol empty state — navigasi ke Products (CustomerHomePage).</summary>
    public ICommand GoToHomeCommand { get; }

    public OrderViewModel()
    {
        GoToHomeCommand = new Command(async () =>
            await Shell.Current.GoToAsync("//Products"));

        // Muat data awal saat konstruksi
        RefreshOrders();
    }

    // ── Refresh dari OrderHistory ─────────────────────────────────────────────

    /// <summary>
    /// Dipanggil dari OnAppearing setiap kali halaman dibuka
    /// agar list selalu sinkron dengan data terbaru.
    /// </summary>
    public void RefreshOrders()
    {
        // Balik urutan: terbaru di atas
        var reversed = OrderHistory.Orders.Reverse().ToList();
        Orders = new ObservableCollection<OrderRecord>(reversed);
    }
}
