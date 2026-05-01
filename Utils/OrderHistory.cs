namespace AstroBoy.Utils;

/// <summary>
/// Satu item dalam sebuah record order.
/// </summary>
public class OrderItemRecord
{
    public string ProductName { get; set; } = string.Empty;
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
    public string StoreName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public List<OrderItemRecord> Items { get; set; } = new();
    public decimal Total { get; set; }

    // ── Computed ──────────────────────────────────────────────────────────────
    public string ShortId => OrderId.Length >= 6 ? OrderId[..6].ToUpper() : OrderId.ToUpper();
    public string OrderLabel => $"Order #{ShortId}";
    public string TotalFormatted => $"Rp {Total:N0}";
    public string DateFormatted => OrderDate.ToString("dd MMM yyyy, HH:mm");
    public string StoreSummary => string.IsNullOrWhiteSpace(StoreName) ? "Toko" : StoreName;
    public string StatusText => Status == "Completed" ? "✅ Selesai" : "🕐 Pending";
    public Color StatusBadgeBackground => Status == "Completed"
        ? Color.FromArgb("#DCFCE7")
        : Color.FromArgb("#FEF9C3");
    public Color StatusTextColor => Status == "Completed"
        ? Color.FromArgb("#16A34A")
        : Color.FromArgb("#CA8A04");
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
