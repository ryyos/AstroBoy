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
│  ├─ Id: str
│  ├─ Name: str
│  ├─ Email: str
│  └─ Password: str
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
│   │  ├─ StaffId: str
│   │  ├─ HireDate: str
│   │  └─ IsActive: bool
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
│   └── Owner : Staff
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
│  ├─ Id: str
│  ├─ Name: str
│  ├─ Address: str
│  ├─ Phone: int
│  ├─ OwnerId: str
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
│  ├─ Id: str
│  ├─ Name: str
│  ├─ Price: float
│  ├─ Stock: int
│  ├─ Category: str
│  └─ StoreId: str
│
└─ Methods
   ├─ UpdateStock(quantity)
   │   → Stock = Stock + quantity
   │
   └─ ChangePrice(newPrice)
       → Price = newPrice

ItemCart
├─ Properties
│  ├─ ItemId: str
│  └─ Quantity: int
│
└─ Methods
   └─ UpdateQuantity(qty)
       → Quantity = qty

ShoppingCart
├─ Properties
│  ├─ Id: str
│  ├─ CustomerId: str
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
│  ├─ Id: str
│  ├─ CustomerId: str
│  ├─ StoreId: str
│  ├─ CreatedAt: str
│  ├─ Status: str
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

Payment
├─ Properties
│  ├─ Id: str
│  ├─ OrderId: str
│  ├─ Amount: str
│  ├─ Method: str
│  ├─ Status: str
│  └─ PaidAt: str
│
└─ Methods
   ├─ Pay(method)
   │   → Method()
   │   → Status()
   │   → PaidAt()
   │
   └─ MarkAsFailed()
       → Status = "Failed"

OrderItem
├─ Properties
│  ├─ ItemId: str
│  ├─ ItemName: str
│  ├─ UnitPrice: int
│  └─ Quantity: int
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

FileStorage : IStorage
└─ Responsibility
   ├─ Save → serialize object ke file
   ├─ Load → deserialize dari file
   └─ Delete → hapus file
```
