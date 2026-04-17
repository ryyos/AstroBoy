using AstroBoy.Models;
using AstroBoy.ViewModels.OwnerViewModel;

namespace AstroBoy.Views.Owner;

public partial class OwnerItemFormPage : ContentPage
{
    public OwnerItemFormPage(Store store, Item? existingItem = null)
    {
        InitializeComponent();
        BindingContext = new OwnerItemFormViewModel(store.StoreId, existingItem);
    }
}
