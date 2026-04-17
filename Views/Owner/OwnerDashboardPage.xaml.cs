using AstroBoy.Models;
using AstroBoy.ViewModels.OwnerViewModel;
using OwnerUser = AstroBoy.Models.Owner;

namespace AstroBoy.Views.Owner;

public partial class OwnerDashboardPage : ContentPage
{
    private readonly OwnerDashboardViewModel _vm;
    private readonly OwnerUser _owner;

    public OwnerDashboardPage(OwnerUser owner)
    {
        InitializeComponent();
        _owner = owner;
        _vm = new OwnerDashboardViewModel(owner);
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.RefreshStores();
    }

    private async void OnStoreTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Store store)
            await Navigation.PushAsync(new OwnerStoreDetailPage(store));
    }

    private async void OnAddStoreClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OwnerStoreFormPage(_owner.Id));
    }

    private async void OnEditStoreClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Store store)
            await Navigation.PushAsync(new OwnerStoreFormPage(_owner.Id, store));
    }

    private async void OnDeleteStoreClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Store store)
        {
            bool confirm = await DisplayAlert("Hapus Toko", $"Hapus \"{store.Name}\"? Semua item di toko ini juga akan dihapus.", "Ya", "Tidak");
            if (confirm)
                _vm.DeleteStore(store);
        }
    }
}