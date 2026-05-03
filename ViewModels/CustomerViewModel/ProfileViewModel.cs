using System.Windows.Input;
using AstroBoy.Utils;
using AstroBoy.ViewModels.Base;
using AstroBoy.Views.Auth;
using AstroBoy.Views.VCustomer;
using Database;

namespace AstroBoy.ViewModels.CustomerViewModel;

/// <summary>
/// ViewModel untuk ProfilePage.
/// Membaca data dari SessionUser dan mengelola aksi logout dan top up.
/// </summary>
public class ProfileViewModel : BaseViewModel
{
    // ── Data profil dari SessionUser ──────────────────────────────────────────
    public string Name => SessionUser.Current?.Name ?? "Guest";
    public string Email => SessionUser.Current?.Email ?? "-";
    public string Role => "Customer";
    public string Avatar => "profil_icon.png";

    // ── Saldo (observable agar UI update setelah top up) ─────────────────────
    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        set
        {
            _balance = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BalanceFormatted));
        }
    }
    public string BalanceFormatted => $"Rp {Balance:N0}";

    // ── Top Up Overlay state ──────────────────────────────────────────────────
    private bool _isTopUpVisible;
    public bool IsTopUpVisible
    {
        get => _isTopUpVisible;
        set { _isTopUpVisible = value; OnPropertyChanged(); }
    }

    private string _manualAmount = string.Empty;
    public string ManualAmount
    {
        get => _manualAmount;
        set { _manualAmount = value; OnPropertyChanged(); }
    }

    private string _topUpError = string.Empty;
    public string TopUpError
    {
        get => _topUpError;
        set { _topUpError = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasTopUpError)); }
    }
    public bool HasTopUpError => !string.IsNullOrEmpty(_topUpError);

    // ── Commands ──────────────────────────────────────────────────────────────
    public ICommand GoToOrderHistoryCommand { get; }
    public ICommand TopUpCommand { get; }
    public ICommand HideTopUpCommand { get; }
    public ICommand SelectQuickAmountCommand { get; }
    public ICommand ConfirmTopUpCommand { get; }
    public ICommand LogoutCommand { get; }

    public ProfileViewModel()
    {
        _balance = SessionUser.Current?.Balance ?? 0;

        GoToOrderHistoryCommand = new Command(async () =>
            await Shell.Current.GoToAsync(nameof(OrderHistoryPage)));

        TopUpCommand = new Command(() =>
        {
            ManualAmount = string.Empty;
            TopUpError = string.Empty;
            IsTopUpVisible = true;
        });

        HideTopUpCommand = new Command(() =>
        {
            IsTopUpVisible = false;
            TopUpError = string.Empty;
        });

        SelectQuickAmountCommand = new Command<string>(amount =>
        {
            ManualAmount = amount;
            TopUpError = string.Empty;
        });

        ConfirmTopUpCommand = new Command(async () => await ProcessTopUp());
        LogoutCommand = new Command(async () => await Logout());
    }

    /// <summary>Dipanggil dari OnAppearing agar saldo selalu sinkron.</summary>
    public void RefreshBalance() => Balance = SessionUser.Current?.Balance ?? 0;

    // ── Process Top Up ────────────────────────────────────────────────────────
    private async Task ProcessTopUp()
    {
        TopUpError = string.Empty;

        // top up decimal
        var clean = System.Text.RegularExpressions.Regex.Replace(ManualAmount, @"[^\d]", "");
        if (!long.TryParse(clean, out var amountLong) || amountLong <= 0)
        {
            TopUpError = "Masukkan nominal yang valid (contoh: 50000).";
            return;
        }
        var amount = (decimal)amountLong;

        var newBalance = Balance + amount;
        var db = new DatabaseContext();
        db.UpdateUserBalance(SessionUser.Current!.Id, newBalance);

        SessionUser.Current!.Balance = newBalance;
        Balance = newBalance;

        IsTopUpVisible = false;
        ManualAmount = string.Empty;
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

        SessionUser.Clear();
        CartBag.Clear();
        OrderHistory.Clear();

        Application.Current!.Windows[0].Page = new NavigationPage(new LoginPage());
    }
}

