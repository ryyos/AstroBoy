<br>
<h1 align="center" >ASTROBOY FILE STRUKTUR</h1>
<br>

```
AstroBoy
│
├── App.xaml
├── AppShell.xaml
├── MauiProgram.cs
│
├── Views/                     ← UI
│   ├── LoginPage.xaml
│   ├── DashboardPage.xaml
│   ├── StoreListPage.xaml
│   ├── StoreDetailPage.xaml
│   ├── CartPage.xaml
│   └── OrderHistoryPage.xaml
│
├── ViewModels/                ← logic UI
│   ├── LoginViewModel.cs
│   ├── StoreViewModel.cs
│   ├── CartViewModel.cs
│   └── OrderViewModel.cs
│
├── Models/                    ← Domain Classes
│   ├── User/
│   │   ├── User.cs
│   │   ├── Staff.cs
│   │   ├── Admin.cs
│   │   ├── StoreOwner.cs
│   │   └── Customer.cs
│   │
│   ├── Store/
│   │   └── Store.cs
│   │
│   ├── Item/
│   │   └── Item.cs
│   │
│   ├── Cart/
│   │   ├── ShoppingCart.cs
│   │   └── CartItem.cs
│   │
│   └── Order/
│       ├── Order.cs
│       └── OrderItem.cs
│
├── Services/                  ← Main Logic
│   ├── AuthService.cs
│   ├── StoreService.cs
│   ├── CartService.cs
│   ├── OrderService.cs
│   └── SalesService.cs
│
├── Storage/                   ← Storage
│   ├── IStorage.cs
│   ├── SQLiteStorage.cs
│   └── FileStorage.cs
│
├── Database/
│   └── DatabaseContext.cs     ← SQLite
│
├── DTO/                      ← opsional
│   └── DTO*.cs
│
└── Utils/
    └── *
```

<br>
<h1 align="center" >ASTROBOY CLASS STRUKTUR</h1>
<br>

```
User (abstract)
├─ Properties
│  ├─ Id
│  ├─ Name
│  ├─ Email
│  └─ Password
│
├─ Methods
│  ├─ Login()
│  │   → [DB User]
│  │
│  └─ Logout()
│      → [DB User]
│
├── Staff (abstract) : User
│   ├─ Properties
│   │  ├─ StaffId
│   │  ├─ HireDate
│   │  └─ IsActive
│   │
│   ├─ Methods
│   │  └─ ViewDashboard()
│   │      → [DB Store] | [DB Product] | [DB Transaction]
│   │
│   ├── Admin : Staff
│   │   └─ Methods
│   │      ├─ ViewAllOwners()
│   │      │   → [Db Owner]
│   │      │
│   │      ├─ ViewAllStores()
│   │      │   → [DB Store]
│   │      │
│   │      ├─ ViewStoreDetails(storeId)
│   │      │   → [DB Store]
│   │      │
│   │      └─ ViewSalesReport(storeId)
│   │          → [DB Store]
│   │
│   └── StoreOwner : Staff
│       ├─ Properties
│       │  └─ List<Store> Stores
│       │
│       └─ Methods
│          ├─ CreateStore(Store store)
│          │   → Stores.Add(store)
│          │
│          ├─ ViewOwnStores()
│          │   → [Stores]
│          │
│          ├─ AddItemToStore(storeId, Item item)
│          │   → Store = [DB Store]
│          │   → Store.AddItem(item)
│          │
│          └─ ViewStoreSales(storeId)
│              → Store.GetOrders()
│
└── Customer : User
    ├─ Properties
    │  └─ ShoppingCart Cart
    │
    └─ Methods
       ├─ AddItemToCart(itemId, qty)
       │   → Cart.AddItem(itemId, qty)
       │
       ├─ RemoveItemFromCart(itemId)
       │   → Cart.RemoveItem(itemId)
       │
       ├─ Checkout()
       │   → [Cart.Items]
       │
       └─ ViewOrderHistory()
           → [DB Transaction]

Store
├─ Properties
│  ├─ Id
│  ├─ Name
│  ├─ Address
│  ├─ Phone
│  ├─ OwnerId
│  ├─ List<Item> Items
│  └─ List<Order> Orders
│
└─ Methods
   ├─ AddItem(Item item)
   │   → Items.Add(item)
   │
   ├─ RemoveItem(itemId)
   │   → Items.Remove(item)
   │
   ├─ GetItems()
   │   → return Items
   │
   └─ GetOrders()
       → return Orders

Item
├─ Properties
│  ├─ Id
│  ├─ Name
│  ├─ Price
│  ├─ Stock
│  ├─ Category
│  └─ StoreId
│
└─ Methods
   ├─ UpdateStock(quantity)
   │   → Stock = Stock + quantity
   │
   └─ ChangePrice(newPrice)
       → Price = newPrice

ItemCart
├─ Properties
│  ├─ ItemId
│  └─ Quantity
│
└─ Methods
   └─ UpdateQuantity(qty)
       → Quantity = qty

ShoppingCart
├─ Properties
│  ├─ Id
│  ├─ CustomerId
│  └─ List<ItemCart> Items
│
└─ Methods
   ├─ AddItem(itemId, qty)
   │   → Items.Add(itemCard)
   │
   ├─ RemoveItem(itemId)
   │   → Items.Remove(itemCard)
   │
   ├─ ClearCart()
   │   → Items.Clear()
   │
   └─ GetTotal()
       → SUM(Items)

Order
├─ Properties
│  ├─ Id
│  ├─ CustomerId
│  ├─ StoreId
│  ├─ CreatedAt
│  ├─ Status
│  └─ List<OrderItem> Items
│
└─ Methods
   ├─ AddOrderItem(OrderItem item)
   │   → Items.Add(item)
   │
   ├─ GetTotal()
   │   → OrderItem.GetSubtotal()
   │
   └─ CompleteOrder()
       → Status = "Completed"

OrderItem
├─ Properties
│  ├─ ItemId
│  ├─ ItemName
│  ├─ UnitPrice
│  └─ Quantity
│
└─ Methods
   └─ GetSubtotal()
       → return UnitPrice * Quantity

IStorage (interface)
└─ Methods
   ├─ Save<T>(any)
   │
   ├─ Load<T>(id)
   │
   └─ Delete<T>(id)

SQLiteStorage : IStorage
└─ Responsibility
   ├─ Save → [INSERT / UPDATE SQLite]
   ├─ Load → [SELECT SQLite]
   └─ Delete → [DELETE SQLite]

FileStorageStrategy : IStorageStrategy
└─ Responsibility
   ├─ Save → serialize object ke file
   ├─ Load → deserialize dari file
   └─ Delete → hapus file
```
