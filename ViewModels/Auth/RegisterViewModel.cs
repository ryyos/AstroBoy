using System.Windows.Input;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;
using AstroBoy.Views.Auth;

public class RegisterViewModel : BaseViewModel
{

    public string? name { get; set; }
    public string? email { get; set; }
    public string? password { get; set; }


    private readonly AuthService _authService;
    public List<string> Roles { get; } = new() { "customer", "owner" };
    private string _role;

    public string Role
    {
        get => _role;
        set
        {
            _role = value;
            OnPropertyChanged();
        }
    }
    public ICommand RegisterCommand { get; }

    public RegisterViewModel()
    {
        _authService = new AuthService();
        RegisterCommand = new Command(OnRegister);
    }

    private async void OnRegister()
    {
        if (string.IsNullOrEmpty(Role))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Pilih role terlebih dahulu", "OK");
            return;
        }

        var success = _authService.Register(name, email, password, Role);

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Success", "Register berhasil", "OK");

            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Username sudah ada", "OK");
        }
    }
}