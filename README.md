<h1 align="center" >WELCOME TO ASTROBOY</h1>

> ASTROBOY adalah sebuah aplikasi yang dikembangkan sebagai project akhir kelompok pada mata kuliah **Pemrograman Berorientasi Objek (PBO)**.  
> Project ini bertujuan untuk menerapkan konsep-konsep utama OOP seperti **encapsulation, inheritance, polymorphism, dan abstraction** ke dalam sebuah aplikasi nyata yang terstruktur dan mudah dikembangkan.

## Tujuan Pengembangan

- Menerapkan prinsip Pemrograman Berorientasi Objek secara praktis
- Melatih kerja sama tim dalam pengembangan software
- Membiasakan penggunaan version control (Git & GitHub)
- Menghasilkan aplikasi yang modular, terstruktur, dan mudah dipelihara

## Feature

- Bahasa Pemrograman: **C#**
- Paradigma: **Object-Oriented Programming (OOP)**
- Framework: **.NET Multi-platform App UI (.NET MAUI)**
- Version Control: **Git & GitHub**
- Tools Pendukung: **Visual Studio Code**

---

## Customer Flow

Fitur yang sudah tersedia untuk role **Customer** (dikerjakan oleh Willy & Rio):

### Halaman Selesai

#### `CustomerHomePage` ✅

- Grid produk 2 kolom dari semua toko
- Search realtime berdasarkan nama produk
- Filter chip berdasarkan nama toko
- Tombol `+` / `−` per produk dengan batas stok
- Badge cart di navbar (muncul jika ada item)
- Toast notifikasi 2 detik saat produk ditambahkan ke keranjang

#### `CartPage` ✅

- Daftar item keranjang dengan gambar, nama, toko, harga satuan
- Tombol `+` / `−` per item (qty minimum 1)
- Tombol `✕` untuk menghapus item sepenuhnya
- Subtotal per item dan total belanja keseluruhan
- Empty state jika keranjang kosong
- Tombol **CHECKOUT** dengan dialog konfirmasi → order tersimpan ke riwayat → keranjang dikosongkan → notifikasi sukses
- Sinkronisasi otomatis: badge dan qty produk di `CustomerHomePage` diperbarui saat kembali dari `CartPage`

#### `StorePage` ✅

- Daftar semua toko dalam card (gambar, nama, jumlah produk)
- Search realtime berdasarkan nama toko
- Tap tombol **Lihat** → navigasi ke `StoreDetailPage`

#### `StoreDetailPage` ✅

- Banner toko full-width dengan gradient overlay dan nama toko
- Grid produk 2 kolom khusus satu toko
- Tombol `+` / `−` per produk, sinkron dengan `CartBag`
- Badge cart di navbar, sinkron saat kembali dari `CartPage`

#### `ProfilePage` ✅

- Avatar profil bulat dengan border dan shadow
- Nama lengkap dan email customer
- Card saldo dengan gradient biru
- Info card: nama, email, role
- Shortcut ke `OrderHistoryPage`
- Tombol **LOGOUT** — membersihkan sesi, cart, dan riwayat order lalu kembali ke `LoginPage`

#### `OrderHistoryPage` ✅

- Daftar riwayat pesanan setelah checkout berhasil
- Urutan terbaru di atas
- Setiap order card menampilkan: ID order, tanggal & jam, nama toko, list item (gambar, nama, qty, subtotal), total, badge **✅ Selesai**
- Empty state dengan tombol kembali ke halaman produk
- Refresh otomatis setiap kali halaman dibuka

### Arsitektur Customer

| Aspek             | Detail                                          |
| ----------------- | ----------------------------------------------- |
| Pattern           | MVVM — `BaseViewModel : INotifyPropertyChanged` |
| Navigasi          | `CustomerAppShell` (Shell + Flyout)             |
| Shared Cart State | `Utils/CartBag.cs` (static in-memory)           |
| Session User      | `Utils/SessionUser.cs` (static in-memory)       |
| Riwayat Order     | `Utils/OrderHistory.cs` (static in-memory)      |
| Data              | In-memory dummy data di `StoreViewModel`        |
| Toast             | `async/await Task.Delay(2000)` — auto dismiss   |

### Dummy Login Customer

| Field    | Value                 |
| -------- | --------------------- |
| Username | `customer`            |
| Password | `123`                 |
| Nama     | John Doe              |
| Email    | customer@astroboy.com |
| Saldo    | Rp 500.000            |

---

## Developer

- **Willy Lengkong** – 01086250001
- **Farhan Febrian Nauval** – 01086250016
- **Rio Dwi Saputra** – 01086250012

## Dosen Pembimbing

**Kusno Prasetya**  
Dosen Mata Kuliah Pemrograman Berorientasi Objek  
Universitas Pelita Harapan

---
