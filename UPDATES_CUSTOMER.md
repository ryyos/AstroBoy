# Dokumentasi Role Customer — AstroBoy POS App

> **Tujuan dokumen:** Menjelaskan alur kerja (_flow_) lengkap role Customer dari registrasi hingga checkout, beserta penerapan konsep OOP (Object-Oriented Programming) yang digunakan dalam implementasinya.

---

## Daftar Isi

1. [Arsitektur & Pola Desain](#1-arsitektur--pola-desain)
2. [Hierarki Class & Inheritance](#2-hierarki-class--inheritance)
3. [Alur Flow Customer (End-to-End)](#3-alur-flow-customer-end-to-end)
4. [Penerapan Konsep OOP](#4-penerapan-konsep-oop)
5. [Shared Utilities Customer](#5-shared-utilities-customer)
6. [Ringkasan Method Penting](#6-ringkasan-method-penting)

---

## 1. Arsitektur & Pola Desain

Aplikasi ini menggunakan pola **MVVM (Model-View-ViewModel)** pada framework **.NET MAUI** (C#).

```
┌─────────────┐     data binding     ┌──────────────────┐     memanggil     ┌──────────────┐
│    VIEW      │ ◄──────────────────► │   VIEWMODEL      │ ────────────────► │   SERVICE /  │
│  (.xaml)    │                      │   (.cs)          │                   │   DATABASE   │
└─────────────┘                      └──────────────────┘                   └──────────────┘
```

| Layer                            | Tanggung Jawab                                                         |
| -------------------------------- | ---------------------------------------------------------------------- |
| **View** (`.xaml`)               | Tampilan UI, binding ke ViewModel                                      |
| **ViewModel** (`.cs`)            | Logika presentasi, state, command                                      |
| **Service**                      | Jembatan antara ViewModel dan Database                                 |
| **Database** (`DatabaseContext`) | Akses langsung ke SQLite                                               |
| **Utils**                        | State bersama antar halaman (`CartBag`, `SessionUser`, `OrderHistory`) |

---

## 2. Hierarki Class & Inheritance

### 2.1 Model User — Abstract Class & Inheritance

```
User  (abstract)
├── Customer
├── Admin
└── Owner
```

**`User.cs`** — Base class abstrak:

```csharp
public abstract class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public decimal Balance { get; set; }

    public User(string name, string email, string password, string role, string? Id = null)
    {
        this.Id = Id ?? Encrypts.Md5Hash(name + email);
        // ...
    }
}
```

**`Customer.cs`** — Subclass yang mewarisi `User`:

```csharp
public class Customer : User
{
    public Customer(string name, string email, string password,
                    string role = "customer", string? Id = null)
        : base(name, email, password, role, Id) { }
}
```

> **Konsep OOP:** _Inheritance_ — `Customer` mewarisi semua property (`Id`, `Name`, `Email`, `Balance`, dll) dari `User` tanpa menulis ulang. Constructor `Customer` memanggil `base(...)` untuk mendelegasikan inisialisasi ke parent.

---

### 2.2 ViewModel — Inheritance dari BaseViewModel

```
INotifyPropertyChanged  (interface)
└── BaseViewModel
    ├── LoginViewModel
    ├── RegisterViewModel
    ├── StoreViewModel
    │   └── StoreFilterItem  (juga extends BaseViewModel)
    │   └── ProductDisplay   (juga extends BaseViewModel)
    ├── StoreDetailViewModel
    ├── ProductDetailViewModel
    ├── CartViewModel
    ├── ProfileViewModel
    └── OrderViewModel
```

**`BaseViewModel.cs`**:

```csharp
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
```

> **Konsep OOP:** _Inheritance_ + _Interface_ — Semua ViewModel mewarisi `BaseViewModel` yang mengimplementasikan `INotifyPropertyChanged`. Dengan ini, setiap ViewModel otomatis mendapat kemampuan notifikasi ke UI tanpa menulis ulang boilerplate event.

---

## 3. Alur Flow Customer (End-to-End)

### 3.1 Register

**Entry point:** `RegisterPage.xaml` → `RegisterViewModel` → `AuthService` → `DatabaseContext`

```
User isi form (nama, email, password, role)
        ↓
RegisterCommand dipanggil → OnRegister()
        ↓
AuthService.Register(name, email, password, role)
        ↓
DatabaseContext.InsertUser(new Customer(...))
        ↓
Redirect ke LoginPage
```

**Method yang terlibat:**

| Method                                  | Class               | Fungsi                            |
| --------------------------------------- | ------------------- | --------------------------------- |
| `OnRegister()`                          | `RegisterViewModel` | Validasi input, panggil service   |
| `Register(name, email, password, role)` | `AuthService`       | Buat object Customer, panggil DB  |
| `InsertUser(User user)`                 | `DatabaseContext`   | `INSERT INTO users ...` ke SQLite |

**Code snippet — `AuthService.Register()`:**

```csharp
public bool Register(string name, string email, string password, string role)
{
    try
    {
        if (role == "customer")
        {
            db.InsertUser(new Customer(name: name, email: email, password: password));
        }
        // ... admin, owner
    }
    catch (Exception ex) { return false; }
    return true;
}
```

---

### 3.2 Login

**Entry point:** `LoginPage.xaml` → `LoginViewModel` → `AuthService` → `SessionUser`

```
User isi email & password
        ↓
LoginCommand → LoginClick()
        ↓
AuthService.Login(email, password)
        ↓
DatabaseContext.GetUser(email, password)
        ↓ (query SELECT ke SQLite, return object User)
        ↓
Cek tipe user → is CustomerModel?
        ↓ ya
SessionUser.Set(user)   ← simpan sesi aktif
        ↓
Application.Current.Windows[0].Page = new CustomerAppShell()
```

**Method yang terlibat:**

| Method                     | Class             | Fungsi                                         |
| -------------------------- | ----------------- | ---------------------------------------------- |
| `LoginClick()`             | `LoginViewModel`  | Entry point login, routing berdasarkan role    |
| `Login(email, password)`   | `AuthService`     | Delegasi ke DB                                 |
| `GetUser(email, password)` | `DatabaseContext` | Query DB, kembalikan subclass User yang sesuai |
| `SessionUser.Set(user)`    | `SessionUser`     | Simpan user aktif ke static state              |

**Code snippet — Polymorphism saat routing login:**

```csharp
private async void LoginClick()
{
    var user = _authService.Login(_Email!, Password!);

    if (user is AdminModel)
        Application.Current!.Windows[0].Page = new AdminShell();
    else if (user is OwnerModel owner)
        await Navigation.PushAsync(new OwnerDashboardPage(owner));
    else if (user is CustomerModel)
    {
        SessionUser.Set(user);
        Application.Current!.Windows[0].Page = new CustomerAppShell();
    }
}
```

> **Konsep OOP:** _Polymorphism_ — Method `GetUser()` mengembalikan tipe `User` (base class), namun objek sebenarnya adalah `Customer`, `Admin`, atau `Owner`. Keyword `is` melakukan _type checking_ di runtime untuk menentukan alur navigasi yang tepat.

---

### 3.3 Dashboard Customer — CustomerAppShell

Setelah login, customer masuk ke `CustomerAppShell` yang merupakan **Shell navigation** dengan flyout menu:

| Menu Item       | Halaman            | ViewModel                 |
| --------------- | ------------------ | ------------------------- |
| Products        | `CustomerHomePage` | `StoreViewModel`          |
| Store           | `StorePage`        | `StoreViewModel` (shared) |
| Profile         | `ProfilePage`      | `ProfileViewModel`        |
| Riwayat Pesanan | `OrderHistoryPage` | `OrderViewModel`          |

Cart dapat diakses dari **toolbar icon** di semua halaman.

---

### 3.4 Browse Produk (CustomerHomePage)

**Flow:**

```
CustomerHomePage muncul
        ↓
StoreViewModel constructor
        ↓
StoreService.GetAllStores() → DatabaseContext.GetAllStores()
        ↓
List<ProductDisplay> dibentuk dari data toko + item
        ↓
BuildCategoryFilters() → chip filter kategori dibuat
        ↓
ApplyFilter() → FilteredProducts diisi
        ↓
UI: CollectionView tampil produk + chip filter + search bar
        ↓
User klik [+] → AddToCartCommand → CartBag.Add(...)
```

**Method yang terlibat:**

| Method                   | Class            | Fungsi                                                       |
| ------------------------ | ---------------- | ------------------------------------------------------------ |
| `LoadDummyData()`        | `StoreViewModel` | Load produk dari DB via StoreService                         |
| `BuildCategoryFilters()` | `StoreViewModel` | Bentuk chip filter dari kategori unik                        |
| `ApplyFilter()`          | `StoreViewModel` | Filter produk berdasarkan kategori + search                  |
| `AddToCart(product)`     | `StoreViewModel` | Tambah 1 unit ke CartBag, update badge, tampilkan toast      |
| `RefreshFromBag()`       | `StoreViewModel` | Sinkronkan qty produk dari CartBag (dipanggil `OnAppearing`) |

---

### 3.5 Browse Toko (StorePage → StoreDetailPage)

**Flow:**

```
StorePage → tampilkan list StoreDisplay (card per toko)
        ↓
User tap card toko → OpenStoreCommand → OpenStore(store)
        ↓
Sinkronkan qty produk toko dengan CartBag
        ↓
Shell.GoToAsync(nameof(StoreDetailPage), { "SelectedStore": store })
        ↓
StoreDetailPage menerima data via [QueryProperty]
        ↓
StoreDetailViewModel(store) → Products = store.Products
```

---

### 3.6 Detail Produk (ProductDetailPage)

Customer dapat tap card produk untuk melihat detail. Page ini dibuat via `Navigation.PushAsync(new ProductDetailPage(product))` — passing object `ProductDisplay` langsung lewat constructor.

```csharp
public ProductDetailViewModel(ProductDisplay product)
{
    _product = product;

    // Sync qty awal dari CartBag
    var entry = CartBag.Items.FirstOrDefault(
        e => e.ProductName == product.ProductName && e.StoreName == product.StoreName);
    _quantity = entry?.Qty ?? 0;
}
```

---

### 3.7 Keranjang Belanja (CartPage)

**Flow:**

```
User tap ikon keranjang → CartPage
        ↓
CartViewModel.LoadFromBag()
        ↓
CartBag.Items → buat List<CartItemViewModel>
        ↓
UI: tampil list produk + qty control + total
        ↓
User tap [CHECKOUT] → CheckoutCommand
        ↓
RefreshCheckoutInfo() → IsCheckoutOverlayVisible = true
        ↓
Overlay muncul: tampil Total, Saldo, Sisa/Kekurangan
        ↓
User tap [Konfirmasi] → ConfirmCheckoutCommand → ProcessCheckout()
        ↓
IsSaldoCukup? Ya → InsertOrder + InsertOrderItem + UpdateUserBalance
        ↓
CartBag.Clear() → Toast 2 detik → Navigate back
```

**Method yang terlibat:**

| Method                  | Class           | Fungsi                                                   |
| ----------------------- | --------------- | -------------------------------------------------------- |
| `LoadFromBag()`         | `CartViewModel` | Baca CartBag → buat CartItemViewModel                    |
| `Increment(item)`       | `CartViewModel` | +1 qty item, update CartBag                              |
| `Decrement(item)`       | `CartViewModel` | -1 qty item, update CartBag                              |
| `Remove(item)`          | `CartViewModel` | Hapus item dari cart & CartBag                           |
| `RefreshCheckoutInfo()` | `CartViewModel` | Raise PropertyChanged untuk semua computed balance props |
| `ProcessCheckout()`     | `CartViewModel` | Validasi saldo, insert order ke DB, kurangi balance      |

---

### 3.8 Top Up Saldo (ProfilePage)

**Flow:**

```
User buka Profile → ProfilePage.OnAppearing()
        ↓
vm.RefreshBalance() → Balance = SessionUser.Current.Balance
        ↓
User tap [+ Top Up Saldo] → TopUpCommand
        ↓
IsTopUpVisible = true → overlay muncul
        ↓
User pilih quick amount → SelectQuickAmountCommand("50000")
        → ManualAmount = "50000"
ATAU user ketik manual di Entry → ManualAmount = "150000"
        ↓
User tap [Konfirmasi] → ConfirmTopUpCommand → ProcessTopUp()
        ↓
Regex strip non-digit → long.TryParse → amount (decimal)
        ↓
DatabaseContext.UpdateUserBalance(userId, newBalance)
SessionUser.Current.Balance = newBalance
Balance = newBalance  ← observable, UI update otomatis
        ↓
IsTopUpVisible = false
```

**Code snippet — Parsing aman lintas locale:**

```csharp
private async Task ProcessTopUp()
{
    // Strip semua karakter bukan angka (aman di semua locale device)
    var clean = Regex.Replace(ManualAmount, @"[^\d]", "");
    if (!long.TryParse(clean, out var amountLong) || amountLong <= 0)
    {
        TopUpError = "Masukkan nominal yang valid (contoh: 50000).";
        return;
    }
    var amount = (decimal)amountLong;

    var newBalance = Balance + amount;
    db.UpdateUserBalance(SessionUser.Current!.Id, newBalance);
    SessionUser.Current!.Balance = newBalance;
    Balance = newBalance;  // trigger OnPropertyChanged → UI update
}
```

---

### 3.9 Riwayat Pesanan (OrderHistoryPage)

**Flow:**

```
OrderHistoryPage.OnAppearing()
        ↓
OrderViewModel.RefreshOrders()
        ↓
DatabaseContext.GetOrdersByCustomer(customerId)
        ↓
Query: SELECT orders JOIN stores JOIN order_items WHERE customer_id = @cid
        ↓
List<OrderRecord> → ObservableCollection<OrderRecord> Orders
        ↓
UI: CollectionView tampil tiap order dengan StatusBadge (✅ Selesai / 🕐 Pending)
```

---

## 4. Penerapan Konsep OOP

### 4.1 Encapsulation (Enkapsulasi)

Enkapsulasi adalah pembungkusan data dan method dalam satu unit (class), dengan akses kontrol menggunakan modifier.

**Contoh 1 — Property dengan backing field di ViewModel:**

```csharp
// ProfileViewModel.cs
private decimal _balance;
public decimal Balance
{
    get => _balance;
    set
    {
        _balance = value;
        OnPropertyChanged();             // notifikasi UI
        OnPropertyChanged(nameof(BalanceFormatted));  // computed juga ikut update
    }
}
public string BalanceFormatted => $"Rp {Balance:N0}";
```

> Data `_balance` bersifat `private` — tidak bisa diubah langsung dari luar. Akses hanya melalui property `Balance` yang mengontrol side effect (notifikasi UI).

**Contoh 2 — `SessionUser` melindungi data sesi:**

```csharp
public static class SessionUser
{
    public static User? Current { get; private set; }  // setter private

    public static void Set(User user) => Current = user;
    public static void Clear() => Current = null;
}
```

> `Current` hanya bisa di-set dari dalam class `SessionUser` sendiri (melalui `Set()` dan `Clear()`). Kode di luar hanya bisa membaca.

**Contoh 3 — `CartBag` enkapsulasi list internal:**

```csharp
public static class CartBag
{
    private static readonly List<CartBagEntry> _items = new(); // private

    public static IReadOnlyList<CartBagEntry> Items => _items.AsReadOnly(); // read-only view
    public static int TotalCount => _items.Sum(i => i.Qty);

    public static void Add(...) { ... }
    public static void Decrement(...) { ... }
    public static void Remove(...) { ... }
    public static void Clear() => _items.Clear();
}
```

> `_items` tidak bisa dimanipulasi langsung dari luar — hanya bisa diakses sebagai `IReadOnlyList`. Semua mutasi wajib lewat method yang sudah dikontrol.

---

### 4.2 Inheritance (Pewarisan)

**Contoh 1 — Model User:**

```csharp
// User adalah base class abstrak
public abstract class User
{
    public string Id { get; set; }
    public decimal Balance { get; set; }
    // ... property lain
}

// Customer mewarisi semua dari User
public class Customer : User
{
    public Customer(string name, string email, string password,
                    string role = "customer", string? Id = null)
        : base(name, email, password, role, Id) { }
}
```

> `Customer` tidak perlu mendefinisikan ulang `Id`, `Name`, `Email`, `Balance` — semua sudah ada di `User`.

**Contoh 2 — BaseViewModel:**

```csharp
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

// Semua ViewModel mewarisi BaseViewModel
public class ProfileViewModel : BaseViewModel { ... }
public class CartViewModel    : BaseViewModel { ... }
public class StoreViewModel   : BaseViewModel { ... }
// dst.
```

> Dengan satu kali implementasi di `BaseViewModel`, semua ViewModel langsung mendapat kemampuan notifikasi ke UI tanpa kode berulang.

---

### 4.3 Polymorphism (Polimorfisme)

**Contoh 1 — Runtime type checking pada login:**

```csharp
var user = _authService.Login(_Email!, Password!);
// user bertipe User (base), tapi objek nyatanya bisa Customer, Admin, atau Owner

if (user is AdminModel)
    // → AdminShell
else if (user is OwnerModel owner)
    // → OwnerDashboardPage
else if (user is CustomerModel)
    // → CustomerAppShell
```

> Satu variable `user` bertipe `User`, namun perilaku (halaman yang dituju) berbeda tergantung tipe nyata objek di runtime — inilah _runtime polymorphism_.

**Contoh 2 — `ICommand` sebagai interface polimorfik:**

```csharp
// Di ViewModel, semua command bertipe ICommand
public ICommand TopUpCommand { get; }
public ICommand HideTopUpCommand { get; }
public ICommand ConfirmTopUpCommand { get; }

// Implementasinya bisa berupa Command, AsyncCommand, dsb.
TopUpCommand     = new Command(() => IsTopUpVisible = true);
ConfirmTopUpCommand = new Command(async () => await ProcessTopUp());
```

> UI (XAML) hanya tahu tipe `ICommand` — tidak peduli implementasi di baliknya apakah synchronous atau async.

**Contoh 3 — `DatabaseContext.GetUser()` mengembalikan subtype berbeda:**

```csharp
public User? GetUser(string email, string password)
{
    // ...
    if (role == "admin")    return new Admin(...);
    if (role == "owner")    return new Owner(...);
    if (role == "customer") return new Customer(...);
    return null;
}
```

> Return type adalah `User`, namun objek yang dikembalikan adalah subclass yang berbeda sesuai data di DB.

---

### 4.4 Abstraction (Abstraksi)

**Contoh 1 — `User` sebagai abstract class:**

```csharp
public abstract class User
{
    public string Id { get; set; }
    public decimal Balance { get; set; }
    // ...
}
```

> Tidak ada objek `User` yang bisa dibuat langsung (`new User(...)` tidak valid). Class ini hanya mendefinisikan "blueprint" yang harus diimplementasikan oleh subclass konkret (`Customer`, `Admin`, `Owner`).

**Contoh 2 — `BaseViewModel` mengabstraksi mekanisme notifikasi:**

```csharp
// Kode di ProfileViewModel — cukup panggil OnPropertyChanged()
set
{
    _balance = value;
    OnPropertyChanged();  // detail implementasi INotifyPropertyChanged tersembunyi
}
```

> Developer tidak perlu tahu detail cara `PropertyChanged` event bekerja — cukup panggil `OnPropertyChanged()`.

**Contoh 3 — Service Layer mengabstraksi akses database:**

```csharp
// StoreViewModel tidak perlu tahu SQL — cukup panggil service
private void LoadDummyData()
{
    var storeService = new StoreService();
    var stores = storeService.GetAllStores(); // detail DB tersembunyi
    // ...
}
```

> `StoreService` menyembunyikan query SQL dari ViewModel. ViewModel hanya tahu "minta data toko", bukan "bagaimana cara query SQLite".

---

## 5. Shared Utilities Customer

### 5.1 `SessionUser` — Manajemen Sesi Login

```csharp
public static class SessionUser
{
    public static User? Current { get; private set; }
    public static void Set(User user) => Current = user;
    public static void Clear() => Current = null;
}
```

| Digunakan di                | Tujuan                                                             |
| --------------------------- | ------------------------------------------------------------------ |
| `LoginViewModel`            | `SessionUser.Set(user)` setelah login berhasil                     |
| `ProfileViewModel`          | Baca `Current.Balance`, `Current.Name`, `Current.Email`            |
| `CartViewModel`             | Baca `Current.Balance` untuk validasi checkout; tulis balance baru |
| `OrderViewModel`            | Baca `Current.Id` untuk query order history                        |
| `ProfileViewModel.Logout()` | `SessionUser.Clear()` saat logout                                  |

---

### 5.2 `CartBag` — State Keranjang Belanja

```
StoreViewModel ──────► CartBag.Add() / Decrement()
StoreDetailViewModel ─►           │
ProductDetailViewModel ──────────►│         CartBag (static, in-memory)
                                  │
CartViewModel ◄───────────────────┘  CartBag.Items (baca)
                                     CartBag.Clear() (setelah checkout)
```

`CartBag` adalah **shared state** antar halaman yang berbeda. Karena `.NET MAUI Shell` men-cache instance halaman, CartBag yang bersifat static memastikan data keranjang konsisten meskipun user berpindah-pindah halaman.

---

### 5.3 `OrderHistory` — Riwayat In-Memory

Menyimpan riwayat order dalam sesi aktif. Dikosongkan saat logout.

```csharp
public static class OrderHistory
{
    private static readonly List<OrderRecord> _orders = new();
    public static IReadOnlyList<OrderRecord> Orders => _orders.AsReadOnly();
    public static void Add(OrderRecord order) => _orders.Add(order);
    public static void Clear() => _orders.Clear();
}
```

### 5.4 `OrderRecord` & `OrderItemRecord` — Model Data Order

```csharp
public class OrderRecord
{
    public string OrderId { get; set; }
    public string Status { get; set; } = "Pending";
    public List<OrderItemRecord> Items { get; set; } = new();

    // Computed properties — tidak disimpan di DB, dihitung saat diperlukan
    public string StatusText => Status == "Completed" ? "✅ Selesai" : "🕐 Pending";
    public Color StatusBadgeBackground => Status == "Completed"
        ? Color.FromArgb("#DCFCE7")
        : Color.FromArgb("#FEF9C3");
}
```

---

## 6. Ringkasan Method Penting

### Auth Flow

| Method         | Class               | Signature                                                                       |
| -------------- | ------------------- | ------------------------------------------------------------------------------- |
| `LoginClick()` | `LoginViewModel`    | `private async void LoginClick()`                                               |
| `Login()`      | `AuthService`       | `public User? Login(string email, string password)`                             |
| `OnRegister()` | `RegisterViewModel` | `private async void OnRegister()`                                               |
| `Register()`   | `AuthService`       | `public bool Register(string name, string email, string password, string role)` |
| `GetUser()`    | `DatabaseContext`   | `public User? GetUser(string email, string password)`                           |
| `InsertUser()` | `DatabaseContext`   | `public void InsertUser(User user)`                                             |

### Customer Core Flow

| Method                  | Class              | Signature                                              |
| ----------------------- | ------------------ | ------------------------------------------------------ |
| `LoadDummyData()`       | `StoreViewModel`   | `private void LoadDummyData()`                         |
| `ApplyFilter()`         | `StoreViewModel`   | `private void ApplyFilter()`                           |
| `AddToCart()`           | `StoreViewModel`   | `private async void AddToCart(ProductDisplay product)` |
| `RefreshFromBag()`      | `StoreViewModel`   | `public void RefreshFromBag()`                         |
| `ProcessCheckout()`     | `CartViewModel`    | `private async Task ProcessCheckout()`                 |
| `RefreshCheckoutInfo()` | `CartViewModel`    | `private void RefreshCheckoutInfo()`                   |
| `ProcessTopUp()`        | `ProfileViewModel` | `private async Task ProcessTopUp()`                    |
| `RefreshBalance()`      | `ProfileViewModel` | `public void RefreshBalance()`                         |
| `RefreshOrders()`       | `OrderViewModel`   | `public void RefreshOrders()`                          |

### Database (CustomerContext)

| Method                  | Signature                                                                                              | SQL                                                                   |
| ----------------------- | ------------------------------------------------------------------------------------------------------ | --------------------------------------------------------------------- |
| `UpdateUserBalance()`   | `void UpdateUserBalance(string userId, decimal newBalance)`                                            | `UPDATE users SET balance = @balance WHERE id = @id`                  |
| `InsertOrder()`         | `void InsertOrder(string orderId, string customerId, string storeId, string status, string createdAt)` | `INSERT INTO orders ...`                                              |
| `InsertOrderItem()`     | `void InsertOrderItem(string orderId, string itemId, string itemName, int unitPrice, int quantity)`    | `INSERT INTO order_items ...`                                         |
| `GetOrdersByCustomer()` | `List<OrderRecord> GetOrdersByCustomer(string customerId)`                                             | `SELECT orders JOIN stores JOIN order_items WHERE customer_id = @cid` |
| `GetUserBalance()`      | `decimal GetUserBalance(string userId)`                                                                | `SELECT balance FROM users WHERE id = @id`                            |
