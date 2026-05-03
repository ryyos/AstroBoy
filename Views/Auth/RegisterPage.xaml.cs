namespace AstroBoy.Views.Auth;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = new RegisterViewModel(); // Menghubungkan ke logic asli
    }
}