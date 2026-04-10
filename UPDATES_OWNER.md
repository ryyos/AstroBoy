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
- **`ViewModels/Owner/OwnerDashboardViewModel.cs`** *(baru)* — Menampilkan pesan sambutan dan daftar toko milik owner
- **`ViewModels/Owner/OwnerStoreDetailViewModel.cs`** *(baru)* — Mengelola daftar item toko dengan `ObservableCollection<Item>`; method `RefreshItems()` dan `DeleteItem(item)`
- **`ViewModels/Owner/OwnerItemFormViewModel.cs`** *(baru)* — Form tambah/edit item; validasi input (nama, harga, stok, kategori); dual mode (tambah jika `existingItem == null`, edit jika ada); `SaveCommand` + `HasError`

### Views
- **`Views/Owner/OwnerDashboardPage.xaml`** — Diperbarui: `CollectionView` daftar toko dengan navigasi ke detail toko
- **`Views/Owner/OwnerDashboardPage.xaml.cs`** — Constructor menerima `Owner`; handler `OnStoreSelected` navigasi ke `OwnerStoreDetailPage`
- **`Views/Owner/OwnerStoreDetailPage.xaml`** *(baru)* — Menampilkan nama toko, daftar item (nama, harga, stok, kategori) dengan tombol Edit & Hapus; tombol "Tambah Item"
- **`Views/Owner/OwnerStoreDetailPage.xaml.cs`** *(baru)* — `OnAppearing` memanggil `RefreshItems()`; handler tambah, edit, hapus item dengan konfirmasi `DisplayAlert`
- **`Views/Owner/OwnerItemFormPage.xaml`** *(baru)* — Form input: Nama Item, Harga, Stok, Kategori; tampilkan pesan error jika validasi gagal; tombol Simpan
- **`Views/Owner/OwnerItemFormPage.xaml.cs`** *(baru)* — Constructor menerima `Store` dan opsional `Item` untuk mode edit

### Auth & Navigation
- **`ViewModels/Auth/LoginViewModel.cs`** — Diperbarui: setelah login sebagai Owner, navigasi ke `OwnerDashboardPage(owner)` dengan meneruskan objek `Owner`

### Bug Fix
- Konflik namespace `Owner` (antara `AstroBoy.Views.Owner` / `AstroBoy.ViewModels.Owner` dengan `AstroBoy.Models.Owner`) diselesaikan dengan alias:
  ```csharp
  using OwnerUser = AstroBoy.Models.Owner;
  ```
  Diterapkan di: `LoginViewModel.cs`, `OwnerDashboardViewModel.cs`, `OwnerDashboardPage.xaml.cs`

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

| Aspek | Detail |
|---|---|
| Framework | .NET MAUI, .NET 10, C# 14 |
| Pattern | MVVM — `BaseViewModel : INotifyPropertyChanged` |
| Navigasi | `NavigationPage` + `PushAsync/PopAsync` (Owner & Customer); `AppShell` (Admin) |
| Data | In-memory dummy data, `static readonly` di `StoreService` |
| Tipe Saldo | `decimal` (presisi finansial) |
| Namespace alias | Wajib digunakan di semua file dalam namespace `*.Owner` yang mereferensikan `AstroBoy.Models.Owner` |
