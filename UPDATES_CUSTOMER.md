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

## Feat: Customer Flow — CartPage

### Utils

#### `Utils/CartBag.cs` _(baru)_

Static in-memory shared cart state — single source of truth untuk keranjang belanja.

| Member                | Keterangan                                                                                           |
| --------------------- | ---------------------------------------------------------------------------------------------------- |
| `CartBagEntry`        | Data satu produk di keranjang: `ProductName`, `StoreName`, `Price`, `ImageSource`, `MaxStock`, `Qty` |
| `CartBag.Items`       | `IReadOnlyList<CartBagEntry>` — semua item saat ini                                                  |
| `CartBag.TotalCount`  | Total qty seluruh item                                                                               |
| `CartBag.Add()`       | Tambah 1 unit; jika sudah ada, increment (max = MaxStock)                                            |
| `CartBag.Decrement()` | Kurangi 1 unit; jika Qty = 0, entry dihapus otomatis                                                 |
| `CartBag.Remove()`    | Hapus satu produk sepenuhnya                                                                         |
| `CartBag.Clear()`     | Kosongkan seluruh keranjang                                                                          |

### ViewModels

#### `ViewModels/CustomerViewModel/CartViewModel.cs` _(rewrite)_

Berisi **2 class**:

**1. `CartItemViewModel : BaseViewModel`**

- Wrapper observable satu item di CartPage
- Properties: `ProductName`, `StoreName`, `Price`, `ImageSource`, `MaxStock`
- `Qty` (observable) — update `QtyLabel` dan `SubtotalFormatted` secara reaktif

**2. `CartViewModel : BaseViewModel`**

| Property               | Keterangan                                                         |
| ---------------------- | ------------------------------------------------------------------ |
| `CartItems`            | `ObservableCollection<CartItemViewModel>` — list item di keranjang |
| `Total`                | Total harga (decimal), computed dari sum semua subtotal            |
| `TotalFormatted`       | Format `Rp x.xxx`                                                  |
| `HasItems` / `IsEmpty` | Kontrol empty state vs list                                        |

| Command            | Aksi                                                                            |
| ------------------ | ------------------------------------------------------------------------------- |
| `IncrementCommand` | Tambah qty +1 (max = MaxStock), sync CartBag                                    |
| `DecrementCommand` | Kurangi qty -1 (min = 1)                                                        |
| `RemoveCommand`    | Hapus item sepenuhnya dari CartBag + list                                       |
| `CheckoutCommand`  | Dialog konfirmasi → `CartBag.Clear()` → notifikasi sukses → kembali ke HomePage |

#### `ViewModels/CustomerViewModel/StoreViewModel.cs` _(diperbarui)_

- `AddToCart` dan `RemoveFromCart` kini sync ke `CartBag`
- Ditambah method `RefreshFromBag()` — dipanggil dari `CustomerHomePage.OnAppearing`

### Views

#### `Views/VCustomer/CartPage.xaml` _(rewrite)_

Layout 2 baris (`*, Auto`):

| Baris | Komponen                    | Keterangan                                        |
| ----- | --------------------------- | ------------------------------------------------- |
| 0     | **List item / Empty state** | `CollectionView` item keranjang atau pesan kosong |
| 1     | **Footer**                  | Total belanja + tombol CHECKOUT                   |

Setiap item card berisi: gambar, nama, toko, harga satuan, tombol `−`/`+`, subtotal, tombol `✕` hapus.

#### `Views/VCustomer/CustomerHomePage.xaml.cs` _(diperbarui)_

Ditambah `OnAppearing` → memanggil `vm.RefreshFromBag()` agar badge cart dan qty produk sinkron setelah kembali dari CartPage.

---

## Feat: Customer Flow — StorePage & StoreDetailPage

### ViewModels

#### `ViewModels/CustomerViewModel/StoreViewModel.cs` _(diperbarui)_

Ditambahkan class dan logic baru:

**`StoreDisplay`** (class baru)

- Merepresentasikan satu toko: `StoreName`, `StoreImage`, `List<ProductDisplay> Products`
- Computed: `ProductCount`, `ProductLabel` (`"n produk tersedia"`)

**Tambahan di `StoreViewModel`:**

| Property / Method        | Keterangan                                                                  |
| ------------------------ | --------------------------------------------------------------------------- |
| `FilteredStores`         | `ObservableCollection<StoreDisplay>` — hasil filter search toko             |
| `StoreSearchQuery`       | Input search toko, auto-trigger `ApplyStoreFilter()`                        |
| `OpenStoreCommand`       | Navigasi ke `StoreDetailPage` dengan `StoreDisplay` via Shell QueryProperty |
| `BuildStores()`          | Bangun `_allStores` dari `_allProducts` (group by StoreName)                |
| `ApplyStoreFilter()`     | Filter `FilteredStores` by `StoreSearchQuery` (contains, ignore case)       |
| `OpenStore()`            | Sinkronkan qty produk toko dari CartBag, lalu `GoToAsync(StoreDetailPage)`  |
| `RefreshStoresFromBag()` | Sinkronkan qty semua produk di semua `StoreDisplay` dari CartBag            |

#### `ViewModels/CustomerViewModel/StoreDetailViewModel.cs` _(baru)_

ViewModel khusus `StoreDetailPage`, menerima `StoreDisplay` dari konstruktor.

| Property                     | Keterangan                                          |
| ---------------------------- | --------------------------------------------------- |
| `StoreName`, `StoreImage`    | Binding ke banner                                   |
| `ProductSectionLabel`        | `"Produk Tersedia (n)"`                             |
| `Products`                   | `IReadOnlyList<ProductDisplay>` dari `StoreDisplay` |
| `CartCount` / `HasCartItems` | Badge cart                                          |

| Command                 | Aksi                                                       |
| ----------------------- | ---------------------------------------------------------- |
| `AddToCartCommand`      | Tambah qty +1, sync CartBag                                |
| `RemoveFromCartCommand` | Kurangi qty -1, sync CartBag                               |
| `GoToCartCommand`       | Navigasi ke `CartPage`                                     |
| `RefreshFromBag()`      | Dipanggil dari `OnAppearing` — sinkronkan qty dari CartBag |

### Views

#### `Views/VCustomer/StorePage.xaml` _(rewrite)_

| Baris | Komponen       | Keterangan                                                                        |
| ----- | -------------- | --------------------------------------------------------------------------------- |
| 0     | **Search Bar** | Filter toko by nama, binding `StoreSearchQuery`                                   |
| 1     | **List Toko**  | `CollectionView` card toko: gambar (h=160), nama, jumlah produk, tombol **Lihat** |

#### `Views/VCustomer/StoreDetailPage.xaml` _(rewrite)_

| Baris | Komponen          | Keterangan                                                        |
| ----- | ----------------- | ----------------------------------------------------------------- |
| 0     | **Banner toko**   | Full width h=180, gradient overlay gelap, nama toko overlay bawah |
| 1     | **Label section** | `"Produk Tersedia (n)"`                                           |
| 2     | **Product Grid**  | 2 kolom, card identik dengan `CustomerHomePage`                   |

#### `Views/VCustomer/StoreDetailPage.xaml.cs` _(rewrite)_

- Menggunakan `[QueryProperty(nameof(SelectedStore), "SelectedStore")]`
- Set `BindingContext = new StoreDetailViewModel(store)` saat parameter diterima
- Set `Title` navbar sesuai nama toko
- `OnAppearing` → `_vm.RefreshFromBag()`

---

## Feat: Customer Flow — ProfilePage & OrderHistoryPage

### Utils

#### `Utils/SessionUser.cs` _(baru)_

Static in-memory session state — menyimpan data user yang sedang login.

| Member                  | Keterangan                                             |
| ----------------------- | ------------------------------------------------------ |
| `SessionUser.Current`   | `User?` — user aktif saat ini, `null` jika belum login |
| `SessionUser.Set(user)` | Dipanggil saat login berhasil, menyimpan data user     |
| `SessionUser.Clear()`   | Dipanggil saat logout, menghapus sesi                  |

Diisi oleh `LoginViewModel` setelah login sukses sebagai Customer.

#### `Utils/OrderHistory.cs` _(baru)_

Static in-memory riwayat order — menyimpan semua order yang sudah di-checkout.

**`OrderItemRecord`**

| Property            | Keterangan                   |
| ------------------- | ---------------------------- |
| `ProductName`       | Nama produk                  |
| `StoreName`         | Nama toko                    |
| `ImageSource`       | Nama file gambar produk      |
| `Price`             | Harga satuan                 |
| `Qty`               | Jumlah dibeli                |
| `Subtotal`          | `Price * Qty` (computed)     |
| `SubtotalFormatted` | Format `Rp x.xxx` (computed) |
| `QtyLabel`          | Format `"xN"` (computed)     |

**`OrderRecord`**

| Property         | Keterangan                                                       |
| ---------------- | ---------------------------------------------------------------- |
| `OrderId`        | `Guid.NewGuid().ToString()` — ID unik per order                  |
| `OrderDate`      | `DateTime.Now` saat checkout                                     |
| `Items`          | `List<OrderItemRecord>` — semua produk dalam order ini           |
| `Total`          | Total harga keseluruhan                                          |
| `StoreSummary`   | Nama toko; jika >1 toko → `"Toko A + n toko lainnya"` (computed) |
| `ShortId`        | 6 karakter pertama `OrderId` uppercase (computed)                |
| `OrderLabel`     | `"Order #XXXXXX"` — label siap pakai untuk UI (computed)         |
| `TotalFormatted` | Format `Rp x.xxx` (computed)                                     |
| `DateFormatted`  | Format `"dd MMM yyyy, HH:mm"` (computed)                         |

**`OrderHistory` (static class)**

| Member                 | Keterangan                                  |
| ---------------------- | ------------------------------------------- |
| `OrderHistory.Orders`  | `IReadOnlyList<OrderRecord>` — semua order  |
| `OrderHistory.Add()`   | Tambah satu record order baru               |
| `OrderHistory.Clear()` | Hapus semua riwayat (dipanggil saat logout) |

---

### Update File yang Sudah Ada

#### `Services/AuthService.cs` _(diperbarui)_

Ditambahkan dummy customer khusus untuk testing:

```csharp
if (username == "customer" && password == "123")
{
    return new Customer("John Doe", "customer@astroboy.com", password, "customer")
    {
        Balance = 500_000
    };
}
```

#### `ViewModels/Auth/LoginViewModel.cs` _(diperbarui)_

Ditambahkan `SessionUser.Set(user)` sebelum pindah ke `CustomerAppShell`.
Digunakan `Windows[0].Page` (pengganti `MainPage` yang deprecated di .NET 9+):

```csharp
else if (user is CustomerModel)
{
    SessionUser.Set(user);  // ← simpan sesi
    Application.Current!.Windows[0].Page = new CustomerAppShell();  // ← cara baru
}
```

#### `ViewModels/CustomerViewModel/CartViewModel.cs` _(diperbarui)_

`CheckoutCommand` kini menyimpan order ke `OrderHistory` sebelum cart dikosongkan.
Digunakan `DisplayAlertAsync` (pengganti `DisplayAlert` yang deprecated di .NET 9+):

```csharp
var record = new OrderRecord
{
    Items = CartBag.Items.Select(i => new OrderItemRecord { ... }).ToList(),
    Total = CartBag.Items.Sum(i => i.Price * i.Qty)
};
OrderHistory.Add(record);   // ← simpan ke riwayat
CartBag.Clear();

// Dialog menggunakan DisplayAlertAsync (bukan DisplayAlert)
await Shell.Current.DisplayAlertAsync("Pesanan Berhasil", "...", "OK");
```

#### `Views/VCustomer/CustomerAppShell.xaml` _(diperbarui)_

Ditambahkan menu flyout baru dan route baru:

```xaml
<FlyoutItem Title="Riwayat Pesanan" Icon="store_icon.png">
    <ShellContent ContentTemplate="{DataTemplate customer:OrderHistoryPage}"/>
</FlyoutItem>
```

```csharp
Routing.RegisterRoute(nameof(OrderHistoryPage), typeof(OrderHistoryPage));
```

---

### ViewModels

#### `ViewModels/CustomerViewModel/ProfileViewModel.cs` _(baru)_

| Property           | Sumber                                 |
| ------------------ | -------------------------------------- |
| `Name`             | `SessionUser.Current?.Name ?? "Guest"` |
| `Email`            | `SessionUser.Current?.Email ?? "-"`    |
| `Role`             | Hardcoded `"Customer"`                 |
| `Avatar`           | `"profil_icon.png"`                    |
| `Balance`          | `SessionUser.Current?.Balance ?? 0`    |
| `BalanceFormatted` | Format `Rp x.xxx`                      |

| Command                   | Aksi                                                                                                 |
| ------------------------- | ---------------------------------------------------------------------------------------------------- |
| `GoToOrderHistoryCommand` | `Shell.Current.GoToAsync(nameof(OrderHistoryPage))`                                                  |
| `LogoutCommand`           | Dialog konfirmasi → `SessionUser.Clear()` + `CartBag.Clear()` + `OrderHistory.Clear()` → `LoginPage` |

#### `ViewModels/CustomerViewModel/OrderViewModel.cs` _(baru)_

| Property    | Keterangan                                                                                     |
| ----------- | ---------------------------------------------------------------------------------------------- |
| `Orders`    | `ObservableCollection<OrderRecord>` — dari `OrderHistory`, dibalik urutannya (terbaru di atas) |
| `HasOrders` | `true` jika ada order                                                                          |
| `IsEmpty`   | `true` jika tidak ada order                                                                    |

| Command           | Aksi                                                                  |
| ----------------- | --------------------------------------------------------------------- |
| `GoToHomeCommand` | `Shell.Current.GoToAsync("//Products")` — untuk tombol di empty state |

| Method            | Keterangan                                                                                   |
| ----------------- | -------------------------------------------------------------------------------------------- |
| `RefreshOrders()` | Baca ulang dari `OrderHistory`, balik urutan, update `Orders` — dipanggil dari `OnAppearing` |

---

### Views

#### `Views/VCustomer/ProfilePage.xaml` _(rewrite)_

Layout `ScrollView` → `VerticalStackLayout`, background `#F8F9FF`:

| Komponen             | Keterangan                                                                                            |
| -------------------- | ----------------------------------------------------------------------------------------------------- |
| **Avatar**           | `Border` bulat 100×100, `StrokeShape RoundRectangle CornerRadius="50"`, border putih, shadow          |
| **Nama**             | FontSize 20, Bold, `#1F2937`, center                                                                  |
| **Email**            | FontSize 14, `#6B7280`, center                                                                        |
| **Card Saldo**       | `LinearGradientBrush` dari `#3E64FF` ke `#5B7FFF`, rounded 16, label + nominal bold FontSize 28 putih |
| **Info: Nama**       | Card putih rounded 12, label abu + nilai bold                                                         |
| **Info: Email**      | Card putih rounded 12                                                                                 |
| **Info: Role**       | Card putih rounded 12                                                                                 |
| **Shortcut Riwayat** | Card putih rounded 12, `TapGestureRecognizer` → `GoToOrderHistoryCommand`, teks biru + arrow `→`      |
| **Tombol LOGOUT**    | Background `#EF4444`, teks putih bold, rounded 12, full width                                         |

#### `Views/VCustomer/ProfilePage.xaml.cs` _(rewrite)_

```csharp
BindingContext = new ProfileViewModel();
```

#### `Views/VCustomer/OrderHistoryPage.xaml` _(baru)_

Layout `Grid` — dua layer (empty state + CollectionView) dengan `IsVisible` binding:

| Kondisi     | Komponen        | Keterangan                                                           |
| ----------- | --------------- | -------------------------------------------------------------------- |
| `IsEmpty`   | **Empty State** | Icon 📦, teks abu, tombol biru `"Mulai Belanja"` → `GoToHomeCommand` |
| `HasOrders` | **Order Cards** | `CollectionView` dengan `x:DataType="utils:OrderRecord"`             |

**Setiap Order Card (`OrderRecord`) berisi:**

- **Header** background `#F3F4F6`, rounded atas 16: `OrderLabel`, `DateFormatted`, `StoreSummary`
- **Item List** via `BindableLayout.ItemsSource="{Binding Items}"` (`x:DataType="utils:OrderItemRecord"`):
  - Gambar produk 48×48 rounded 8
  - Nama produk + `QtyLabel`
  - `SubtotalFormatted` rata kanan biru `#3E64FF`
- **Separator** `BoxView` 1px
- **Footer**: `TotalFormatted` bold + Badge `"✅ Selesai"` background `#DCFCE7` teks `#16A34A`

#### `Views/VCustomer/OrderHistoryPage.xaml.cs` _(baru)_

```csharp
private readonly OrderViewModel _vm;

public OrderHistoryPage()
{
    InitializeComponent();
    _vm = new OrderViewModel();
    BindingContext = _vm;
}

protected override void OnAppearing()
{
    base.OnAppearing();
    _vm.RefreshOrders();   // refresh setiap kali halaman dibuka
}
```

---

## Catatan Arsitektur Customer

| Aspek             | Detail                                          |
| ----------------- | ----------------------------------------------- |
| Framework         | .NET MAUI, .NET 10, C# 14                       |
| Pattern           | MVVM — `BaseViewModel : INotifyPropertyChanged` |
| Navigasi          | `CustomerAppShell` (Shell + Flyout)             |
| Shared Cart State | `Utils/CartBag.cs` (static in-memory)           |
| Session User      | `Utils/SessionUser.cs` (static in-memory)       |
| Riwayat Order     | `Utils/OrderHistory.cs` (static in-memory)      |
| Data              | In-memory dummy data di `StoreViewModel`        |
| Toast             | `async/await Task.Delay(2000)` — auto dismiss   |

---

## Fix: Deprecation Warning (.NET 9+)

Beberapa API yang digunakan sebelumnya sudah deprecated di .NET MAUI .NET 9+.
Semua diganti ke API baru agar tidak ada warning saat build.

| File                  | API Lama (deprecated)                 | API Baru                                     |
| --------------------- | ------------------------------------- | -------------------------------------------- |
| `LoginViewModel.cs`   | `Application.Current.MainPage = ...`  | `Application.Current!.Windows[0].Page = ...` |
| `ProfileViewModel.cs` | `Application.Current!.MainPage = ...` | `Application.Current!.Windows[0].Page = ...` |
| `CartViewModel.cs`    | `Shell.Current.DisplayAlert(...)`     | `Shell.Current.DisplayAlertAsync(...)`       |

**Penjelasan:**

- `Application.MainPage` → deprecated karena MAUI kini mendukung multi-window. Solusi: gunakan `Windows[0].Page` untuk single-window app.
- `Page.DisplayAlert()` → deprecated, diganti `DisplayAlertAsync()` yang lebih konsisten dengan pola async.

---

## Status Halaman VCustomer

| Halaman            | Status     | Keterangan                                                |
| ------------------ | ---------- | --------------------------------------------------------- |
| `CustomerHomePage` | ✅ Selesai | Products grid + search + filter chip + cart badge + toast |
| `CartPage`         | ✅ Selesai | List item + subtotal + total + checkout dialog            |
| `StorePage`        | ✅ Selesai | Search toko + card toko + navigasi ke StoreDetailPage     |
| `StoreDetailPage`  | ✅ Selesai | Banner toko + product grid 2 kolom + sync CartBag         |
| `ProfilePage`      | ✅ Selesai | Avatar, saldo, info card, shortcut order, logout          |
| `OrderHistoryPage` | ✅ Selesai | Order cards, item list, total, badge selesai, empty state |
