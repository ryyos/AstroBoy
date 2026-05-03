using AstroBoy.Models;
using Microsoft.Maui.Controls;

public class ItemViewModel
{
    private readonly Item _item;

    public ItemViewModel(Item item)
    {
        _item = item;
    }

    public string Id => _item.Id;
    public string Name => _item.Name;
    public float Price => _item.Price;

    public ImageSource ImageSource =>
        ImageSource.FromStream(() =>
        {
            var fileName = $"{Id}.jpg";

            System.Diagnostics.Debug.WriteLine($"TRY LOAD: {fileName}");

            var stream = FileSystem
                .OpenAppPackageFileAsync(fileName)
                .GetAwaiter()
                .GetResult();

            return stream;
        });
}