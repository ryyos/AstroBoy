namespace AstroBoy;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void GoToDashboard(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//dashboard");
    }
}