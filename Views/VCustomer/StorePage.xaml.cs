using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class StorePage : ContentPage
{
	public StorePage()
	{
		InitializeComponent();
		BindingContext = new StoreViewModel();
	}

	// Sinkronkan ulang qty toko saat halaman muncul kembali (misal setelah dari CartPage)
	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is StoreViewModel vm)
			vm.RefreshStoresFromBag();
	}
}