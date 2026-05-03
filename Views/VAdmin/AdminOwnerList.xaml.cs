using AstroBoy.ViewModels.AdminViewModel;

namespace AstroBoy.Views.VAdmin;

public partial class AdminOwnerList : ContentPage
{
	public AdminOwnerList()
	{
		InitializeComponent();
		BindingContext = new AdminOwnerListViewModel();
	}
}