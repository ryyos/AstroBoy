using AstroBoy.ViewModels.Auth;

namespace AstroBoy.Views.Auth;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}