using System.Windows.Input;
using AstroBoy.Utils;
using AstroBoy.ViewModels.Base;
using AstroBoy.Views.Auth;
using AstroBoy.Views.VCustomer;

namespace AstroBoy.ViewModels.CustomerViewModel;

/// <summary>
/// ViewModel untuk ProfilePage.
/// Membaca data dari SessionUser dan mengelola aksi logout.
/// </summary>
public class ProfileViewModel : BaseViewModel
{
    // ── Data profil dari SessionUser ──────────────────────────────────────────
    public string Name => SessionUser.Current?.Name ?? "Guest";
    public string Email => SessionUser.Current?.Email ?? "-";
    public string Role => "Customer";
    public string Avatar => "profil_icon.png";

    // ── Saldo ─────────────────────────────────────────────────────────────────
    public decimal Balance => SessionUser.Current?.Balance ?? 0;
    public string BalanceFormatted => $"Rp {Balance:N0}";

    // ── Commands ──────────────────────────────────────────────────────────────
    public ICommand GoToOrderHistoryCommand { get; }
    public ICommand LogoutCommand { get; }

    public ProfileViewModel()
    {
        // Navigasi ke OrderHistoryPage via registered route
        GoToOrderHistoryCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(OrderHistoryPage)));

        LogoutCommand = new Command(async () => await Logout());
    }

    // ── Logout ────────────────────────────────────────────────────────────────
    private async Task Logout()
    {
        bool confirmed = await Shell.Current.DisplayAlertAsync(
            "Logout",
            "Apakah kamu yakin ingin keluar?",
            "Ya, Logout",
            "Batal");

        if (!confirmed) return;

        // Bersihkan semua shared state
        SessionUser.Clear();
        CartBag.Clear();
        OrderHistory.Clear();

        // Kembali ke halaman Login
        Application.Current!.Windows[0].Page = new NavigationPage(new LoginPage());
    }
}

