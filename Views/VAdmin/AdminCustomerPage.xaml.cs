using AstroBoy.ViewModels.AdminViewModel;

namespace AstroBoy.Views.VAdmin;

public partial class AdminCustomerPage : ContentPage
{
    public AdminCustomerPage()
    {
        InitializeComponent();
        BindingContext = new AdminCustomerViewModel();
    }

    private async void OnAddCustomerClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCustomerPage());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminCustomerViewModel vm)
        {
            Console.WriteLine("masuk ke if dan load");
            vm.LoadCustomers();
        }
        else
        {
            Console.WriteLine("tidak masuk ke if dan load");
        }
    }
}