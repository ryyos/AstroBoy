namespace AstroBoy.Utils;

/// <summary>
/// Satu item dalam sebuah record order.
/// </summary>
public class OrderItemRecord
{
    public string ProductName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string ImageSource { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Qty { get; set; }

    // ── Computed ──────────────────────────────────────────────────────────────
    public decimal Subtotal => Price * Qty;
    public string SubtotalFormatted => $"Rp {Subtotal:N0}";
    public string QtyLabel => $"x{Qty}";
}

/// <summary>
/// Satu record order yang tersimpan setelah checkout berhasil.
/// </summary>
public class OrderRecord
{
    // ── Data utama ────────────────────────────────────────────────────────────
    public string OrderId { get; set; } = Guid.NewGuid().ToString();
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public List<OrderItemRecord> Items { get; set; } = new();
    public decimal Total { get; set; }

    // ── Computed ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Ringkasan toko: jika 1 toko → nama toko,
    /// jika >1 → "Toko A + n toko lainnya".
    /// </summary>
    public string StoreSummary
    {
        get
        {
            var stores = Items.Select(i => i.StoreName).Distinct().ToList();
            if (stores.Count == 0) return "-";
            if (stores.Count == 1) return stores[0];
            return $"{stores[0]} + {stores.Count - 1} toko lainnya";
        }
    }

    /// <summary>6 karakter pertama OrderId untuk tampilan di UI.</summary>
    public string ShortId => OrderId.Length >= 6
        ? OrderId[..6].ToUpper()
        : OrderId.ToUpper();

    /// <summary>Label lengkap untuk UI: "Order #XXXXXX"</summary>
    public string OrderLabel => $"Order #{ShortId}";

    public string TotalFormatted => $"Rp {Total:N0}";
    public string DateFormatted => OrderDate.ToString("dd MMM yyyy, HH:mm");
}

/// <summary>
/// Static in-memory riwayat order — single source of truth.
/// Diisi oleh CartViewModel saat checkout berhasil.
/// Dikosongkan saat logout.
/// </summary>
public static class OrderHistory
{
    private static readonly List<OrderRecord> _orders = new();

    public static IReadOnlyList<OrderRecord> Orders => _orders.AsReadOnly();

    /// <summary>Tambah satu record order baru.</summary>
    public static void Add(OrderRecord order) => _orders.Add(order);

    /// <summary>Hapus semua riwayat order.</summary>
    public static void Clear() => _orders.Clear();
}
