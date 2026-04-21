using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class CustomerHomePage : ContentPage
{
    public CustomerHomePage()
    {
        InitializeComponent();
        // Semua logic ada di StoreViewModel (MVVM)
        BindingContext = new StoreViewModel();
    }
}

