using AstroBoy.ViewModels.AdminViewModel;

namespace AstroBoy.Views.VAdmin;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();
        BindingContext = new AdminDashboardViewModel();
    }
}