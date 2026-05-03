using System.Windows.Input;
using AstroBoy.Services;
using AstroBoy.Utils;
using AstroBoy.ViewModels.Base;
using AstroBoy.Views.Auth;
using AstroBoy.Views.Owner;
using AstroBoy.Views.VCustomer;
using AdminModel = AstroBoy.Models.Admin;
using CustomerModel = AstroBoy.Models.Customer;
using OwnerModel = AstroBoy.Models.Owner;

namespace AstroBoy.ViewModels.Auth;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    public string? _Email { get; set; }
    public string? Password { get; set; }
    public string? ErrorMessage { get; set; }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    public LoginViewModel()
    {
        _authService = new AuthService();
        GoToRegisterCommand = new Command(OnGoToRegister);
        LoginCommand = new Command(LoginClick);
    }

    private async void OnGoToRegister()
    {
        var nav = Application.Current.MainPage?.Navigation;

        if (nav != null)
        {
            await nav.PushAsync(new RegisterPage());
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Navigation not available", "OK");
        }
    }

    private async void LoginClick()
    {
        System.Diagnostics.Debug.WriteLine($"VM INPUT: '{_Email}' | '{Password}'");
        var user = _authService.Login(_Email!, Password!);

        if (user == null)
        {
            ErrorMessage = "Invalid credentials";
            OnPropertyChanged(nameof(ErrorMessage));
            return;
        }

        if (user is AdminModel)
        {
            Application.Current!.Windows[0].Page = new AdminShell();
        }
        else if (user is OwnerModel owner)
        {
            await Application.Current!.Windows[0].Page!.Navigation.PushAsync(new OwnerDashboardPage(owner));
        }
        else if (user is CustomerModel)
        {
            // Simpan sesi user aktif sebelum pindah halaman
            SessionUser.Set(user);

            // Arahkan ke Shell khusus Customer
            Application.Current!.Windows[0].Page = new CustomerAppShell();
        }
    }
}