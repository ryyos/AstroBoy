using System.Windows.Input;
using AstroBoy.Services;
using AstroBoy.Views.Owner;
using AstroBoy.Views.VCustomer;
using AstroBoy.ViewModels.Base;

using AdminModel = AstroBoy.Models.Admin;
using OwnerModel = AstroBoy.Models.Owner;
using CustomerModel = AstroBoy.Models.Customer;

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

        if (user is AdminModel)
        {
            Application.Current.MainPage = new AdminShell();
        }
        else if (user is OwnerModel owner)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new OwnerDashboardPage(owner));
        }
        else if (user is CustomerModel)
        {
            // Arahkan ke Shell khusus Customer
            Application.Current!.MainPage = new CustomerAppShell();
        }
    }
}