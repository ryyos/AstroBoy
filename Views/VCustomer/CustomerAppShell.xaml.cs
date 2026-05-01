using AstroBoy.Utils;

namespace AstroBoy.Views.VCustomer;

public partial class CustomerAppShell : Shell
{
    public string CustomerName { get; private set; } = "Customer";
    public string CustomerInitial { get; private set; } = "C";
    public string CustomerSubtitle { get; private set; } = "Alo, Welcome back! 👋";

    public CustomerAppShell()
    {
        // Isi properti header SEBELUM InitializeComponent agar binding langsung terbaca
        var user = SessionUser.Current;
        var name = user?.Name ?? "Customer";
        CustomerName = name;
        CustomerInitial = name.Length > 0 ? name[0].ToString().ToUpper() : "C";
        CustomerSubtitle = $"Alo, {name.Split(' ')[0]}! Welcome back! 👋";

        InitializeComponent();

        // Jadikan Shell sendiri sebagai BindingContext sehingga FlyoutHeader bisa binding
        BindingContext = this;

        // Daftarkan route navigasi
        Routing.RegisterRoute(nameof(StoreDetailPage), typeof(StoreDetailPage));
        Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));
        Routing.RegisterRoute(nameof(OrderHistoryPage), typeof(OrderHistoryPage));
        Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
    }

    private async void OnCartClicked(object sender, EventArgs e)
    {
        await GoToAsync(nameof(CartPage));
    }
}