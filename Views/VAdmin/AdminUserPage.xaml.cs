using AstroBoy.ViewModels.VAdmin;

namespace AstroBoy.Views.VAdmin;

public partial class AdminUserPage : ContentPage
{
	public AdminUserPage()
	{
		InitializeComponent();
        BindingContext = new AdminUsersViewModel();
    }

    private async void OnAddUserClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddUserPage());
    }
}