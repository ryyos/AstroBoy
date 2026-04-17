using AstroBoy.ViewModels.AdminViewModel;

namespace AstroBoy.Views.VAdmin;

public partial class AddCustomerPage : ContentPage
{
    public AddCustomerPage()
    {
        InitializeComponent();
        BindingContext = new AddCustomerViewModel();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddUserClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCustomerPage());
    }
}