using AstroBoy.Models;

namespace AstroBoy.Utils;

/// <summary>
/// Menyimpan data user yang sedang login — single source of truth untuk sesi aktif.
/// Diisi saat login berhasil, dikosongkan saat logout.
/// </summary>
public static class SessionUser
{
    /// <summary>User yang sedang login. Null jika belum login.</summary>
    public static User? Current { get; private set; }

    /// <summary>Set user aktif setelah login berhasil.</summary>
    public static void Set(User user) => Current = user;

    /// <summary>Hapus sesi saat logout.</summary>
    public static void Clear() => Current = null;
}
