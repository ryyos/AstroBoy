# AstroBoy — Riwayat Update (Farhan)

## Feat: Owner Flow (Login → Kelola Store → Item CRUD)

### Models

- **`Models/Store/Store.cs`** — Ditambahkan `namespace AstroBoy.Models`
- **`Models/Item/Item.cs`** — Ditambahkan `namespace AstroBoy.Models`
- **`Models/Order/Order.cs`** — Ditambahkan `namespace AstroBoy.Models`
- **`Models/Order/OrderItem.cs`** — Ditambahkan `namespace AstroBoy.Models`; implementasi `GetSubtotal()` dikembalikan

### Services

- **`Services/StoreService.cs`** — Ditulis ulang sepenuhnya:
  - Data dummy statis (`private static readonly List<Store> _stores`) dengan 2 toko milik owner
  - Method `GetTotalStores()`, `GetStoresByOwner(ownerId)`
  - Method CRUD item: `GetStoreById(id)`, `AddItem(storeId, item)`, `UpdateItem(storeId, item)`, `DeleteItem(storeId, itemId)`

### ViewModels

- **`ViewModels/Owner/OwnerDashboardViewModel.cs`** _(baru)_ — Menampilkan pesan sambutan dan daftar toko milik owner
- **`ViewModels/Owner/OwnerStoreDetailViewModel.cs`** _(baru)_ — Mengelola daftar item toko dengan `ObservableCollection<Item>`; method `RefreshItems()` dan `DeleteItem(item)`
- **`ViewModels/Owner/OwnerItemFormViewModel.cs`** _(baru)_ — Form tambah/edit item; validasi input (nama, harga, stok, kategori); dual mode (tambah jika `existingItem == null`, edit jika ada); `SaveCommand` + `HasError`

### Views

- **`Views/Owner/OwnerDashboardPage.xaml`** — Diperbarui: `CollectionView` daftar toko dengan navigasi ke detail toko
- **`Views/Owner/OwnerDashboardPage.xaml.cs`** — Constructor menerima `Owner`; handler `OnStoreSelected` navigasi ke `OwnerStoreDetailPage`
- **`Views/Owner/OwnerStoreDetailPage.xaml`** _(baru)_ — Menampilkan nama toko, daftar item (nama, harga, stok, kategori) dengan tombol Edit & Hapus; tombol "Tambah Item"
- **`Views/Owner/OwnerStoreDetailPage.xaml.cs`** _(baru)_ — `OnAppearing` memanggil `RefreshItems()`; handler tambah, edit, hapus item dengan konfirmasi `DisplayAlert`
- **`Views/Owner/OwnerItemFormPage.xaml`** _(baru)_ — Form input: Nama Item, Harga, Stok, Kategori; tampilkan pesan error jika validasi gagal; tombol Simpan
- **`Views/Owner/OwnerItemFormPage.xaml.cs`** _(baru)_ — Constructor menerima `Store` dan opsional `Item` untuk mode edit

### Auth & Navigation

- **`ViewModels/Auth/LoginViewModel.cs`** — Diperbarui: setelah login sebagai Owner, navigasi ke `OwnerDashboardPage(owner)` dengan meneruskan objek `Owner`

### Bug Fix

- Konflik namespace `Owner` (antara `AstroBoy.Views.Owner` / `AstroBoy.ViewModels.Owner` dengan `AstroBoy.Models.Owner`) diselesaikan dengan alias:
  ```csharp
  using OwnerUser = AstroBoy.Models.Owner;
  ```
  Diterapkan di: `LoginViewModel.cs`, `OwnerDashboardViewModel.cs`, `OwnerDashboardPage.xaml.cs`

---

## Bug Fix: Owner Id tidak terbaca dari Database

### Root Cause

- **`Models/User/User.cs`** — Di constructor, `Id = Id ?? ...` hanya mengassign ke parameter lokal, bukan ke property `this.Id`, sehingga `this.Id` selalu `null` setelah login.

### Fix

- **`Models/User/User.cs`** — Diubah dari `Id = Id ?? Encrypts.Md5Hash(...)` menjadi `this.Id = Id ?? Encrypts.Md5Hash(...)`.
- Dampak: Setelah login sebagai owner, `owner.Id` kini terisi dari nilai database (misal `"26"`), sehingga `GetStoresByOwner(owner.Id)` berhasil memfilter toko yang tepat.

---

## Feat: Gambar Produk pada Daftar Item Toko

### Problem

- File gambar item di `Resources/Images/items/` memiliki nama berupa angka (misal `100074560313.jpg`).
- `MauiImage` menolak file yang namanya diawali angka (aturan penamaan Android resource).

### Solusi

- **`AstroBoy.csproj`** — Entry `MauiImage` untuk folder `items/` diganti menjadi `MauiAsset` dengan `LogicalName="items/%(Filename)%(Extension)"` agar gambar di-bundle tanpa batasan nama file.
- **`Utils/ItemImageConverter.cs`** _(baru)_ — `IValueConverter` yang membuka gambar dari app bundle via `FileSystem.OpenAppPackageFileAsync($"items/{id}.jpg")` dan mengembalikan `ImageSource` dari stream.
- **`Views/Owner/OwnerStoreDetailPage.xaml`** — Diperbarui:
  - Tambah namespace `xmlns:utils="clr-namespace:AstroBoy.Utils"`
  - Daftarkan converter di `ContentPage.Resources`
  - Layout item card diubah dari 3 kolom menjadi 4 kolom (`Auto,*,Auto,Auto`)
  - Tambah `<Image>` ukuran 72×72 dengan sudut melengkung di kolom pertama, source binding menggunakan `ItemImageConverter`

---

## Feat: — Fitur Saldo (Balance) untuk Payment

### Models

- **`Models/User/User.cs`** — Ditambahkan properti `Balance`:
  ```csharp
  public decimal Balance { get; set; }
  ```
  Diinisialisasi ke `0` di constructor. Berlaku untuk semua tipe user (Admin, Owner, Customer).

### Services

- **`Services/AuthService.cs`** — Login Owner kini menetapkan saldo dummy:
  ```csharp
  var owner = new Owner(username, username, password, "owner") { Balance = 2_500_000 };
  ```

### ViewModels

- **`ViewModels/Owner/OwnerDashboardViewModel.cs`** — Ditambahkan properti:
  ```csharp
  public string BalanceFormatted => $"Rp {_owner.Balance:N0}";
  ```

### Views

- **`Views/Owner/OwnerDashboardPage.xaml`** — Ditambahkan kartu saldo di atas daftar toko:
  - Label "Saldo Anda" (teks abu-abu kecil)
  - Label nilai saldo terformat (biru, bold, besar) terikat ke `BalanceFormatted`

---

## Catatan Arsitektur

| Aspek           | Detail                                                                                              |
| --------------- | --------------------------------------------------------------------------------------------------- |
| Framework       | .NET MAUI, .NET 10, C# 14                                                                           |
| Pattern         | MVVM — `BaseViewModel : INotifyPropertyChanged`                                                     |
| Navigasi        | `NavigationPage` + `PushAsync/PopAsync` (Owner & Customer); `AppShell` (Admin)                      |
| Data            | SQLite via `DatabaseContext` (persisten); in-memory `_stores` sebagai cache per-session             |
| Tipe Saldo      | `decimal` (presisi finansial)                                                                       |
| Namespace alias | Wajib digunakan di semua file dalam namespace `*.Owner` yang mereferensikan `AstroBoy.Models.Owner` |

---

## Feat: Koneksi Owner ke Database (SQLite)

### Root Cause

- Semua operasi CRUD toko dan item Owner sebelumnya hanya menyimpan data **in-memory** (tidak persisten). Setelah aplikasi di-restart, data hilang.

### Perubahan

#### `Database/DatabaseContext.cs`

Ditambahkan 7 method baru untuk keperluan Owner:

| Method | Keterangan |
| --- | --- |
| `GetItemsForStore(storeId)` | Memuat ulang item satu toko dari DB (public wrapper dari private method) |
| `InsertStore(Store)` | INSERT baris baru ke tabel `stores` |
| `UpdateStore(Store)` | UPDATE nama, alamat, telepon toko di DB |
| `DeleteStore(storeId)` | DELETE toko + cascade DELETE semua item milik toko tersebut |
| `InsertItem(Item)` | INSERT item baru ke tabel `items` |
| `UpdateItem(Item)` | UPDATE nama, harga, stok, kategori item di DB |
| `DeleteItem(itemId)` | DELETE item dari tabel `items` |

#### `Services/StoreService.cs`

Semua method mutasi kini memanggil DB **sebelum** memperbarui in-memory list:

- `AddStore` → `db.InsertStore` + `_stores.Add`
- `UpdateStore` → `db.UpdateStore` + update properti in-memory
- `DeleteStore` → `db.DeleteStore` (cascade) + `_stores.Remove`
- `AddItem` → `db.InsertItem` + `store.Items.Add`
- `UpdateItem` → `db.UpdateItem` + update properti in-memory
- `DeleteItem` → `db.DeleteItem` + `store.Items.Remove`

Ditambahkan method baru:
- `GetFreshItemsByStoreId(storeId)` — reload item langsung dari DB dan memperbarui cache in-memory toko tersebut; dipakai `RefreshItems` di ViewModel agar data selalu sinkron setelah navigasi dari form add/edit item.

#### `ViewModels/OwnerViewModel/OwnerStoreDetailViewModel.cs`

- `RefreshItems()` diubah untuk menggunakan `_storeService.GetFreshItemsByStoreId(Store.StoreId!)` alih-alih `Store.Items` (yang bisa stale setelah navigasi ke halaman form).

#### `ViewModels/OwnerViewModel/OwnerItemFormViewModel.cs`

- ID item baru sebelumnya hardcoded `"68767887"`. Diubah menjadi `DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()` agar setiap item punya ID unik (format 13-digit numerik, konsisten dengan data seed yang ada).

### Ringkasan Alur Data Owner (setelah fix)

```
DB (astroboy.sqlite)
    ↕ load saat StoreService dibuat
_stores (static cache)
    ↕ diperbarui sinkron oleh setiap operasi CRUD
ObservableCollection (UI)
    ↑ diperbarui oleh ViewModel setelah setiap operasi
```

Item detail page kini memanggil `GetFreshItemsByStoreId` (baca ulang DB) setiap kali `OnAppearing`, sehingga perubahan dari halaman form langsung terlihat tanpa perlu restart.
