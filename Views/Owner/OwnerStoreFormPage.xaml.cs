using AstroBoy.Models;
using AstroBoy.ViewModels.OwnerViewModel;

namespace AstroBoy.Views.Owner;

public partial class OwnerStoreFormPage : ContentPage
{
    public OwnerStoreFormPage(string ownerId, Store? existingStore = null)
    {
        InitializeComponent();
        BindingContext = new OwnerStoreFormViewModel(ownerId, existingStore);
    }
}
