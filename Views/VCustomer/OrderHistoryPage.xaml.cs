using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class OrderHistoryPage : ContentPage
{
    private readonly OrderViewModel _vm;

    public OrderHistoryPage()
    {
        InitializeComponent();
        // Semua logic ada di ViewModel — sesuai pola MVVM
        _vm = new OrderViewModel();
        BindingContext = _vm;
    }

    /// <summary>
    /// Refresh data setiap kali halaman dibuka
    /// agar list selalu menampilkan order terbaru.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.RefreshOrders();
    }
}
