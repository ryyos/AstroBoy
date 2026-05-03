using AstroBoy.Views.Auth;
using AstroBoy.Views.VAdmin;

namespace AstroBoy
{
    public partial class AdminShell : Shell
    {
        public AdminShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EditCustomerPage), typeof(EditCustomerPage));
            Routing.RegisterRoute(nameof(AdminOwnerStoresPage), typeof(AdminOwnerStoresPage));
            Routing.RegisterRoute(nameof(AdminStoreItemsPage), typeof(AdminStoreItemsPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Logout", "Are you sure?", "Yes", "No");

            if (!confirm) return;

            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
