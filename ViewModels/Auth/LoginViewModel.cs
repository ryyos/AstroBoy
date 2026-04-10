using System.Windows.Input;
using AstroBoy.Services;
using AstroBoy.Views.Owner;
using AstroBoy.Views.VCustomer;
using AstroBoy.ViewModels.Base;

using AdminUser = AstroBoy.Models.Admin;
using OwnerUser = AstroBoy.Models.Owner;
using CustomerUser = AstroBoy.Models.Customer;

namespace AstroBoy.ViewModels.Auth;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ErrorMessage { get; set; }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        _authService = new AuthService();
        LoginCommand = new Command(LoginClick);
    }

    private async void LoginClick()
    {
        var user = _authService.Login(Username!, Password!);

        if (user == null)
        {
            ErrorMessage = "Invalid credentials";
            OnPropertyChanged(nameof(ErrorMessage));
            return;
        }

        if (user is AdminUser)
        {
            Application.Current.MainPage = new AdminShell();
        }
        else if (user is OwnerUser owner)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new OwnerDashboardPage(owner));
        }
        else if (user is CustomerUser)
        {
            // Arahkan ke Shell khusus Customer
            Application.Current!.MainPage = new CustomerAppShell();
        }
    }
}