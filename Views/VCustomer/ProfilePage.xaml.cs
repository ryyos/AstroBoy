using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel();
	}

	// auto rfresh balance 
	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is ProfileViewModel vm)
			vm.RefreshBalance();
	}
}
