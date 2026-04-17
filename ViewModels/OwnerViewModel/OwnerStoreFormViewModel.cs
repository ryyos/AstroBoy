using System.Windows.Input;
using AstroBoy.Models;
using AstroBoy.Services;
using AstroBoy.ViewModels.Base;

namespace AstroBoy.ViewModels.OwnerViewModel;

public class OwnerStoreFormViewModel : BaseViewModel
{
    private readonly StoreService _storeService;
    private readonly string _ownerId;
    private readonly Store? _existingStore;

    public string Title => _existingStore == null ? "Tambah Toko" : "Edit Toko";

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string? ErrorMessage { get; private set; }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ICommand SaveCommand { get; }

    public OwnerStoreFormViewModel(string ownerId, Store? existingStore = null)
    {
        _storeService = new StoreService();
        _ownerId = ownerId;
        _existingStore = existingStore;

        if (existingStore != null)
        {
            Name = existingStore.Name;
            Address = existingStore.Address ?? string.Empty;
            Phone = existingStore.Phone ?? string.Empty;
        }

        SaveCommand = new Command(Save);
    }

    private async void Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Nama toko wajib diisi";
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasError));
            return;
        }

        if (_existingStore == null)
        {
            var newStore = new Store
            {
                StoreId = Guid.NewGuid().ToString(),
                OwnerId = _ownerId,
                Name = Name,
                Address = Address,
                Phone = Phone,
            };
            _storeService.AddStore(newStore);
        }
        else
        {
            _existingStore.Name = Name;
            _existingStore.Address = Address;
            _existingStore.Phone = Phone;
            _storeService.UpdateStore(_existingStore);
        }

        await Application.Current!.MainPage!.Navigation.PopAsync();
    }
}
