using AstroBoy.ViewModels.AdminViewModel;

namespace AstroBoy.Views.VAdmin;
public partial class AdminStoreItemsPage : ContentPage
{
    public AdminStoreItemsPage()
    {
        InitializeComponent();
        BindingContext = new AdminStoreItemsPageViewModel();
    }
}