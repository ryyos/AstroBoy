using AstroBoy.Models;
using AstroBoy.ViewModels.OwnerViewModel;

namespace AstroBoy.Views.Owner;

public partial class OwnerStoreDetailPage : ContentPage
{
    private readonly OwnerStoreDetailViewModel _vm;

    public OwnerStoreDetailPage(Store store)
    {
        InitializeComponent();
        _vm = new OwnerStoreDetailViewModel(store);
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.RefreshItems();
    }

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OwnerItemFormPage(_vm.Store));
    }

    private async void OnEditItemClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Item item)
            await Navigation.PushAsync(new OwnerItemFormPage(_vm.Store, item));
    }

    private async void OnDeleteItemClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Item item)
        {
            bool confirm = await DisplayAlert("Hapus Item", $"Hapus \"{item.Name}\"?", "Ya", "Tidak");
            if (confirm)
                _vm.DeleteItem(item);
        }
    }
}
