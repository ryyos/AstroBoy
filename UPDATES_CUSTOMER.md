# AstroBoy — Riwayat Update Customer Flow (Willy / Rio)

---

## Feat: Customer Flow — Products Page (CustomerHomePage)

### Services

- **`Services/StoreService.cs`** — Ditambahkan method baru:
  ```csharp
  public List<Store> GetAllStores() => _stores.ToList();
  ```
  Digunakan oleh `StoreViewModel` untuk mengambil semua produk dari semua toko sekaligus.

---

### ViewModels

#### `ViewModels/CustomerViewModel/StoreViewModel.cs` _(baru — ditulis dari kosong)_

Berisi **3 class** dalam satu file:

**1. `StoreFilterItem : BaseViewModel`**

- Merepresentasikan satu chip filter toko di bagian atas halaman
- Property `IsSelected` (observable) yang mengubah warna chip secara reaktif:
  - Aktif → background biru `#3E64FF`, teks putih
  - Nonaktif → background abu `#E5E7EB`, teks gelap

**2. `ProductDisplay : BaseViewModel`**

- Wrapper data produk untuk tampilan UI
- Properties: `ProductName`, `StoreName`, `Price`, `ImageSource`, `Stock`
- `Quantity` (observable) — jumlah item di keranjang, default `0`
- Computed: `QuantityLabel` (string counter), `PriceFormatted` (format `Rp x.xxx`)

**3. `StoreViewModel : BaseViewModel`**

| Property           | Tipe                                    | Keterangan                                     |
| ------------------ | --------------------------------------- | ---------------------------------------------- |
| `FilteredProducts` | `ObservableCollection<ProductDisplay>`  | Produk yang ditampilkan (hasil filter)         |
| `StoreFilters`     | `ObservableCollection<StoreFilterItem>` | Chip filter toko                               |
| `SearchQuery`      | `string`                                | Input search, auto-trigger filter saat berubah |
| `SelectedStore`    | `string`                                | Toko yang dipilih, auto-trigger filter         |
| `CartCount`        | `int`                                   | Total quantity semua item di keranjang         |
| `HasCartItems`     | `bool`                                  | `true` jika CartCount > 0 (untuk badge)        |
| `CartBadgeLabel`   | `string`                                | Angka CartCount sebagai string                 |
| `IsToastVisible`   | `bool`                                  | Kontrol visibilitas toast notifikasi           |
| `ToastMessage`     | `string`                                | Isi pesan toast                                |

| Command                    | Parameter            | Aksi                                                |
| -------------------------- | -------------------- | --------------------------------------------------- |
| `AddToCartCommand`         | `ProductDisplay`     | Tambah qty +1, max = Stock; tampilkan toast 2 detik |
| `RemoveFromCartCommand`    | `ProductDisplay`     | Kurangi qty -1, min = 0                             |
| `SelectStoreFilterCommand` | `string` (nama toko) | Set `SelectedStore`, update chip & filter           |
| `GoToCartCommand`          | —                    | Navigasi ke `CartPage` via Shell                    |

**Filter Logic:**

- Jika `SearchQuery` kosong DAN `SelectedStore` = "Semua" → tampilkan semua
- Jika `SearchQuery` ada → filter `ProductName` (contains, ignore case)
- Jika `SelectedStore` bukan "Semua" → filter by `StoreName`
- Keduanya bisa aktif bersamaan

**Toast Notifikasi:**

- Saat `AddToCart` dipanggil, muncul pesan `"✓ [Nama Produk] ditambahkan ke keranjang"`
- Toast otomatis hilang setelah **2 detik** (menggunakan `async/await Task.Delay`)

**Dummy Data (in-memory):**

```
Toko Elektronik:
  - Laptop ASUS     | Rp 8.000.000 | asus_leptop.png   | Stok: 10
  - Mouse Wireless  | Rp 150.000   | mouse_warlees.png | Stok: 50

Toko Fashion:
  - Jeans Pria      | Rp 250.000   | jeans.png         | Stok: 30
  - Kaos Polos      | Rp 85.000    | kaos_polos.png    | Stok: 100
```

---

### Views

#### `Views/VCustomer/CustomerHomePage.xaml` _(diperbarui)_

Layout mengikuti wireframe dengan 4 baris (`Auto, Auto, Auto, *`):

| Baris | Komponen         | Keterangan                                                                      |
| ----- | ---------------- | ------------------------------------------------------------------------------- |
| 0     | **Toast Banner** | Latar hijau `#22C55E`, muncul saat item ditambahkan ke keranjang                |
| 1     | **Search Bar**   | Input dengan icon `search.svg`, binding `SearchQuery`, filter realtime          |
| 2     | **Filter Chip**  | `ScrollView` horizontal, `BindableLayout` binding `StoreFilters`, warna reaktif |
| 3     | **Product Grid** | `CollectionView` 2 kolom, setiap card: gambar, nama, toko, harga, tombol +/-    |

**Setiap Product Card berisi:**

- Gambar produk full-width, sudut atas rounded (`RoundRectangle CornerRadius="12,12,0,0"`)
- Nama produk (FontSize 14, bold, `#1F2937`)
- Nama toko (FontSize 12, abu `#6B7280`)
- Harga format Rp (bold, biru `#3E64FF`)
- Tombol `-` dan `+` bulat (CornerRadius 16, biru `#3E64FF`)
- Counter quantity di tengah

**Catatan XAML:**

- `Shell.TitleView` dihapus — navbar sepenuhnya dikelola `CustomerAppShell`
- Binding tombol +/- menggunakan `{x:Reference pageRoot}` untuk mengakses command dari ViewModel induk

#### `Views/VCustomer/CustomerHomePage.xaml.cs` _(diperbarui)_

```csharp
// Hanya satu baris logic sesuai MVVM
BindingContext = new StoreViewModel();
```

---

### Bug Fix

- **`AdminOwnerStoresPageViewModel.cs`** — `_service.GetAll()` diganti menjadi `_service.GetAllStores()` karena method lama tidak ada
- **`AdminOwnerStoresPageViewModel.cs`** — `OwnerId` diubah dari `int` ke `string` agar tipe-nya konsisten dengan `Store.OwnerId` yang bertipe `string`
- **`CustomerHomePage.xaml`** — Dihapus sisa kode lama (CollectionView duplikat) yang menyebabkan error `There are multiple root elements`

---

## Catatan Arsitektur Customer

| Aspek           | Detail                                                           |
| --------------- | ---------------------------------------------------------------- |
| Framework       | .NET MAUI, .NET 10, C# 14                                        |
| Pattern         | MVVM — `BaseViewModel : INotifyPropertyChanged`                  |
| Navigasi        | `CustomerAppShell` (Shell + Flyout)                              |
| Data            | In-memory dummy data di `StoreViewModel`                         |
| Cart (saat ini) | In-memory per sesi, disimpan di `Quantity` tiap `ProductDisplay` |
| Toast           | `async/await Task.Delay(2000)` — auto dismiss                    |
| Namespace alias | Tidak diperlukan di Customer flow                                |

---

## Status Halaman VCustomer

| Halaman            | Status     | Keterangan                                             |
| ------------------ | ---------- | ------------------------------------------------------ |
| `CustomerHomePage` | ✅ Selesai | Products grid + search + filter + cart counter + toast |
| `CartPage`         | 🔲 Belum   | Tampilkan isi keranjang, total, checkout               |
| `StorePage`        | 🔲 Belum   | Placeholder                                            |
| `StoreDetailPage`  | 🔲 Belum   | Placeholder                                            |
| `ProfilePage`      | 🔲 Belum   | Placeholder                                            |
| `OrderHistoryPage` | 🔲 Belum   | Placeholder                                            |
