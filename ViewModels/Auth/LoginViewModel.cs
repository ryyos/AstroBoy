using System.Windows.Input;
using AstroBoy.Services;
using AstroBoy.Models;
using AstroBoy.Views.VAdmin;
using AstroBoy.Views.Owner;
using AstroBoy.Views.Customer;
using AstroBoy.ViewModels.Base;

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

        if (user is Admin)
        {
            Application.Current.MainPage = new AppShell();
            //await Application.Current.MainPage.Navigation.PushAsync(new AdminDashboardPage());
        }
        else if (user is Owner)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new OwnerDashboardPage());
        }
        else if (user is Customer)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CustomerHomePage());
        }
    }
}