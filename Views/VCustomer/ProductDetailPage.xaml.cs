using AstroBoy.ViewModels.CustomerViewModel;

namespace AstroBoy.Views.VCustomer;

public partial class ProductDetailPage : ContentPage
{
    public ProductDetailPage(ProductDisplay product)
    {
        InitializeComponent();
        BindingContext = new ProductDetailViewModel(product);
    }
}
