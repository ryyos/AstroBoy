using AstroBoy.ViewModels.VAdmin;

namespace AstroBoy.Views.VAdmin;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();
        BindingContext = new AdminDashboardViewModel();
    }
}