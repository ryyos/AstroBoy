using System.Windows.Input;
using AstroBoy.Services;

namespace AstroBoy.ViewModels;

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
        LoginCommand = new Command(OnLogin);
    }

    private void OnLogin()
    {
        var user = _authService.Login(Username!, Password!);

        if (user == null)
        {
            ErrorMessage = "Invalid credentials";
            OnPropertyChanged(nameof(ErrorMessage));
            return;
        }

    }
}