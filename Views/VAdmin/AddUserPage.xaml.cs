using AstroBoy.ViewModels.Admin;

namespace AstroBoy.Views.VAdmin;

public partial class AddUserPage : ContentPage
{
    public AddUserPage()
    {
        InitializeComponent();
        BindingContext = new AddUserViewModel();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddUserClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddUserPage());
    }
}