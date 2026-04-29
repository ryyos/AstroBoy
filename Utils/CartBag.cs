namespace AstroBoy.Utils;

/// <summary>
/// Satu entry produk yang ada di keranjang belanja.
/// </summary>
public class CartBagEntry
{
    public string ProductName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageSource { get; set; } = string.Empty;
    public int MaxStock { get; set; }
    public int Qty { get; set; }

    public decimal Subtotal => Price * Qty;
    public string PriceFormatted => $"Rp {Price:N0}";
    public string SubtotalFormatted => $"Rp {Subtotal:N0}";
}

/// <summary>
/// Static in-memory shared cart state.
/// Ditulis oleh StoreViewModel (tambah/kurangi dari HomePage)
/// dan dibaca/dikelola oleh CartViewModel (CartPage).
/// </summary>
public static class CartBag
{
    private static readonly List<CartBagEntry> _items = new();

    public static IReadOnlyList<CartBagEntry> Items => _items.AsReadOnly();
    public static int TotalCount => _items.Sum(i => i.Qty);

    /// <summary>
    /// Tambah 1 unit produk. Jika sudah ada entrynya, increment qty (max = maxStock).
    /// </summary>
    public static void Add(string productName, string storeName,
                           decimal price, string imageSource, int maxStock)
    {
        var existing = _items.FirstOrDefault(
            i => i.ProductName == productName && i.StoreName == storeName);

        if (existing is not null)
            existing.Qty = Math.Min(existing.Qty + 1, maxStock);
        else
            _items.Add(new CartBagEntry
            {
                ProductName = productName,
                StoreName = storeName,
                Price = price,
                ImageSource = imageSource,
                MaxStock = maxStock,
                Qty = 1
            });
    }

    /// <summary>
    /// Kurangi 1 unit produk. Jika Qty menjadi 0, entry dihapus otomatis.
    /// </summary>
    public static void Decrement(string productName, string storeName)
    {
        var entry = _items.FirstOrDefault(
            i => i.ProductName == productName && i.StoreName == storeName);

        if (entry is null) return;

        entry.Qty--;
        if (entry.Qty <= 0)
            _items.Remove(entry);
    }

    /// <summary>Hapus satu produk sepenuhnya dari keranjang.</summary>
    public static void Remove(string productName, string storeName)
    {
        var entry = _items.FirstOrDefault(
            i => i.ProductName == productName && i.StoreName == storeName);

        if (entry is not null)
            _items.Remove(entry);
    }

    /// <summary>Kosongkan seluruh keranjang.</summary>
    public static void Clear() => _items.Clear();
}
