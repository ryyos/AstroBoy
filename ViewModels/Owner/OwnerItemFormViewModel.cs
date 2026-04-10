using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.Owner;

public class OwnerItemFormViewModel : BaseViewModel
{
    private readonly StoreService _storeService;
    private readonly string _storeId;
    private readonly Item? _existingItem;

    public string Title => _existingItem == null ? "Tambah Item" : "Edit Item";

    public string Name { get; set; } = string.Empty;
    public string PriceText { get; set; } = string.Empty;
    public string StockText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public string? ErrorMessage { get; private set; }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ICommand SaveCommand { get; }

    public OwnerItemFormViewModel(string storeId, Item? existingItem = null)
    {
        _storeService = new StoreService();
        _storeId = storeId;
        _existingItem = existingItem;

        if (existingItem != null)
        {
            Name = existingItem.Name;
            PriceText = existingItem.Price.ToString();
            StockText = existingItem.Stock.ToString();
            Category = existingItem.Category;
        }

        SaveCommand = new Command(Save);
    }

    private async void Save()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(PriceText) || string.IsNullOrWhiteSpace(StockText))
        {
            ErrorMessage = "Semua field wajib diisi";
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasError));
            return;
        }

        if (!float.TryParse(PriceText, out var price) || !int.TryParse(StockText, out var stock))
        {
            ErrorMessage = "Harga dan stok harus berupa angka";
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasError));
            return;
        }

        if (_existingItem == null)
        {
            var newItem = new Item
            {
                Id = Guid.NewGuid(),
                Name = Name,
                Price = price,
                Stock = stock,
                Category = string.IsNullOrWhiteSpace(Category) ? "-" : Category,
                StoreId = _storeId
            };
            _storeService.AddItem(_storeId, newItem);
        }
        else
        {
            _existingItem.Name = Name;
            _existingItem.Price = price;
            _existingItem.Stock = stock;
            _existingItem.Category = string.IsNullOrWhiteSpace(Category) ? "-" : Category;
            _storeService.UpdateItem(_existingItem);
        }

        await Application.Current!.MainPage!.Navigation.PopAsync();
    }
}
