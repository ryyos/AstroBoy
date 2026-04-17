using AstroBoy.Services;
using AstroBoy.ViewModels.AdminViewModel;

namespace AstroBoy.Views.VAdmin;

public partial class EditCustomerPage : ContentPage
{
    public EditCustomerPage()
    {
        InitializeComponent();
        BindingContext = new EditCustomerViewModel(new CustomerService());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as EditCustomerViewModel)?.LoadData();
    }
}