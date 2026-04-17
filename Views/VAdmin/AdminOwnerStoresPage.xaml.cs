using AstroBoy.ViewModels.AdminViewModel;

namespace AstroBoy.Views.VAdmin;

public partial class AdminOwnerStoresPage : ContentPage
{
    public AdminOwnerStoresPage()
    {
        InitializeComponent();
        BindingContext = new AdminOwnerStoresViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as AdminOwnerStoresViewModel)?.LoadData();
    }
}