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
        }
    }
}
