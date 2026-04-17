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
    }

    private async void OnCartClicked(object sender, EventArgs e)
    {
        await GoToAsync(nameof(CartPage));
    }
}