using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

/// <summary>
/// StoreDetailPage menerima objek StoreDisplay via Shell QueryProperty
/// dan meneruskannya ke StoreDetailViewModel sebagai BindingContext.
/// </summary>
[QueryProperty(nameof(SelectedStore), "SelectedStore")]
public partial class StoreDetailPage : ContentPage
{
	private StoreDetailViewModel? _vm;

	// Dipanggil oleh Shell navigation sebelum OnAppearing
	public StoreDisplay? SelectedStore
	{
		set
		{
			if (value is null) return;
			_vm = new StoreDetailViewModel(value);
			BindingContext = _vm;

			// Set judul navbar sesuai nama toko
			Title = value.StoreName;
		}
	}

	public StoreDetailPage()
	{
		InitializeComponent();
	}

	// Sinkronkan qty produk dari CartBag setiap kali halaman muncul
	protected override void OnAppearing()
	{
		base.OnAppearing();
		_vm?.RefreshFromBag();
	}
}