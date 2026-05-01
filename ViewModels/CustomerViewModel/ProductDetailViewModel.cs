using System.Windows.Input;
using AstroBoy.Utils;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.CustomerViewModel;

public class ProductDetailViewModel : BaseViewModel
{
    private readonly ProductDisplay _product;

    public string ProductName => _product.ProductName;
    public string StoreName => _product.StoreName;
    public string Category => string.IsNullOrWhiteSpace(_product.Category) ? "-" : _product.Category;
    public string PriceFormatted => $"Rp {_product.Price:N0}";
    public string StockText => $"Stok: {_product.Stock}";
    public string ImageSource => _product.ImageSource;

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(QuantityLabel));
        }
    }

    public string QuantityLabel => _quantity.ToString();

    private bool _isToastVisible;
    public bool IsToastVisible
    {
        get => _isToastVisible;
        set { _isToastVisible = value; OnPropertyChanged(); }
    }

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }

    public ProductDetailViewModel(ProductDisplay product)
    {
        _product = product;

        // Sync qty from CartBag
        var entry = CartBag.Items.FirstOrDefault(
            e => e.ProductName == product.ProductName && e.StoreName == product.StoreName);
        _quantity = entry?.Qty ?? 0;

        AddToCartCommand = new Command(AddToCart);
        RemoveFromCartCommand = new Command(RemoveFromCart);
    }

    private async void AddToCart()
    {
        if (_quantity >= _product.Stock) return;

        _quantity++;
        _product.Quantity = _quantity;
        CartBag.Add(_product.ItemId, _product.ProductName, _product.StoreName,
                    _product.StoreId, _product.Price, _product.ImageSource, _product.Stock);
        OnPropertyChanged(nameof(QuantityLabel));

        IsToastVisible = true;
        await Task.Delay(2000);
        IsToastVisible = false;
    }

    private void RemoveFromCart()
    {
        if (_quantity <= 0) return;

        _quantity--;
        _product.Quantity = _quantity;
        CartBag.Decrement(_product.ProductName, _product.StoreName);
        OnPropertyChanged(nameof(QuantityLabel));
    }
}
