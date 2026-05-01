using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class CustomerHomePage : ContentPage
{
    public CustomerHomePage()
    {
        InitializeComponent();
        // Semua logic ada di StoreViewModel (MVVM)
        BindingContext = new StoreViewModel();
    }

    // Sinkronkan ulang CartCount dan Quantity produk saat halaman muncul kembali
    // (misal setelah user kembali dari CartPage dan menghapus item di sana)
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StoreViewModel vm)
            vm.RefreshFromBag();
    }
}

