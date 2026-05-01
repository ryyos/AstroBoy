namespace AstroBoy.Views.VCustomer;

public partial class CustomerAppShell : Shell
{
    public CustomerAppShell()
    {
        InitializeComponent();

        // Daftarkan route untuk StoreDetailPage
        Routing.RegisterRoute(nameof(StoreDetailPage), typeof(StoreDetailPage));

        // Daftarkan route untuk CartPage
        Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));

        // Daftarkan route untuk OrderHistoryPage (navigasi dari ProfilePage)
        Routing.RegisterRoute(nameof(OrderHistoryPage), typeof(OrderHistoryPage));
    }

    private async void OnCartClicked(object sender, EventArgs e)
    {
        await GoToAsync(nameof(CartPage));
    }
}