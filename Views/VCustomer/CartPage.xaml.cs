using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class CartPage : ContentPage
{
	public CartPage()
	{
		InitializeComponent();
		BindingContext = new CartViewModel();
	}
}