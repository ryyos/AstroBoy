using AstroBoy.Models;
using AstroBoy.ViewModels.Owner;
using OwnerUser = AstroBoy.Models.Owner;

namespace AstroBoy.Views.Owner;

public partial class OwnerDashboardPage : ContentPage
{
    public OwnerDashboardPage(OwnerUser owner)
    {
        InitializeComponent();
        BindingContext = new OwnerDashboardViewModel(owner);
    }

    private async void OnStoreSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Store store) return;
        ((CollectionView)sender).SelectedItem = null;
        await Navigation.PushAsync(new OwnerStoreDetailPage(store));
    }
}