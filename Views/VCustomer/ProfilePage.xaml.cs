using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		// Semua logic ada di ViewModel — sesuai pola MVVM
		BindingContext = new ProfileViewModel();
	}
}
