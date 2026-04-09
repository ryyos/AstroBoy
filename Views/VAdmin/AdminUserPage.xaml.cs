using AstroBoy.ViewModels.VAdmin;

namespace AstroBoy.Views.VAdmin;

public partial class AdminUserPage : ContentPage
{
	public AdminUserPage()
	{
		InitializeComponent();
        BindingContext = new AdminUsersViewModel();
    }
}