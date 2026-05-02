using System;
using System.Collections.Generic;
using AstroBoy.Models;
using AstroBoy.Utils;
using Microsoft.Data.Sqlite;

namespace Database
{
    public class DatabaseContext
    {
        private readonly string dbPath;

        public DatabaseContext()
        {
            dbPath = Path.Combine(FileSystem.AppDataDirectory, "astroboy.sqlite");

            Console.WriteLine($"DB PATH: {dbPath}");
        }
        public async Task InitDatabaseAsync()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "astroboy.sqlite");

            System.Diagnostics.Debug.WriteLine($"DB PATH: {dbPath}");

            if (File.Exists(dbPath))
            {
                System.Diagnostics.Debug.WriteLine("DELETING OLD DB...");
                File.Delete(dbPath);
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync("astroboy.sqlite");
            using var fileStream = File.Create(dbPath);

            await stream.CopyToAsync(fileStream);

            System.Diagnostics.Debug.WriteLine("DB REPLACED");
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={dbPath}");
        }
        public void InsertUser(User user)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var query = "INSERT INTO users (name, email, password, role) VALUES (@name, @email, @password, @role)";
                var command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@role", "customer");

                command.ExecuteNonQuery();
            }
        }

        public User? GetUser(string email, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var debugCmd = new SqliteCommand("SELECT email, password, role FROM users", connection);
                var debugReader = debugCmd.ExecuteReader();

                while (debugReader.Read())
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"DB USER: {debugReader["email"]} | {debugReader["password"]} | {debugReader["role"]}"
                    );
                }
                System.Diagnostics.Debug.WriteLine($"LOGIN TRY: '{email}' | '{password}'");
                var query = @"SELECT id, name, email, password, role, balance 
                      FROM users 
                      WHERE email = @email AND password = @password 
                      LIMIT 1";

                Console.WriteLine($"Executing query: {query} with email: {email} and password: {password}");
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var balance = reader["balance"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(reader["balance"]);

                        if (reader["role"].ToString() == "admin")
                        {
                            return new Admin
                            (
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!,
                                Id: reader["id"].ToString()!
                            )
                            { Balance = balance };
                        }
                        else if (reader["role"].ToString() == "owner")
                        {
                            return new Owner
                            (
                                Id: reader["id"].ToString()!,
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!
                            )
                            { Balance = balance };
                        }
                        else if (reader["role"].ToString() == "customer")
                        {
                            return new Customer
                            (
                                Id: reader["id"].ToString()!,
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!
                            )
                            { Balance = balance };
                        }
                    }
                }
            }

            return null;
        }

        public List<string> GetUsers()
        {
            var users = new List<string>();

            using (var connection = GetConnection())
            {
                connection.Open();

                var query = "SELECT * FROM users";
                var command = new SqliteCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string data = $"ID: {reader["id"]}, Name: {reader["name"]}, Email: {reader["email"]}";
                    users.Add(data);
                }
            }

            return users;
        }

        public List<Customer> GetAllCustomers()
        {
            var users = new List<Customer>();

            using (var connection = GetConnection())
            {
                connection.Open();

                var query = "SELECT * FROM users WHERE role = 'customer'";
                var command = new SqliteCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(
                        new Customer(
                            name: reader["name"].ToString()!,
                            email: reader["email"].ToString()!,
                            password: reader["password"].ToString()!
                        )
                    );
                }
            }

            return users;
        }

        public List<Owner> GetAllOwners()
        {
            var users = new List<Owner>();

            using (var connection = GetConnection())
            {
                connection.Open();

                var query = "SELECT * FROM users WHERE role = 'owner'";
                var command = new SqliteCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(
                        new Owner(
                            name: reader["name"].ToString()!,
                            email: reader["email"].ToString()!,
                            password: reader["password"].ToString()!
                        )
                    );
                }
            }

            return users;
        }

        // 🔹 Delete User
        public void DeleteUser(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var query = "DELETE FROM users WHERE id = @id";
                var command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        // 🔹 Update User
        public void UpdateUser(int id, string name, string email, string password, string role)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var query = @"UPDATE users 
                              SET name = @name, email = @email, password = @password, role = @role 
                              WHERE id = @id";

                var command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@role", role);

                command.ExecuteNonQuery();
            }
        }

        private List<Item> GetItemsByStoreId(SqliteConnection connection, string storeId)
        {
            var items = new List<Item>();

            var query = "SELECT * FROM items WHERE store_id = @storeId";
            var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@storeId", storeId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new Item
                    {
                        Id = reader["id"].ToString()!,
                        Name = reader["name"].ToString()!,
                        Price = Convert.ToSingle(reader["price"]),
                        Stock = Convert.ToInt32(reader["stock"]),
                        Category = reader["category"].ToString()!,
                        StoreId = reader["store_id"].ToString()!
                    });
                }
            }

            return items;
        }

        private List<Order> GetOrdersByStoreId(SqliteConnection connection, string storeId)
        {
            var orders = new List<Order>();

            var query = "SELECT * FROM orders WHERE store_id = @storeId";
            var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@storeId", storeId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var order = new Order
                    {
                        Id = reader["id"].ToString()!,
                        CustomerId = reader["customer_id"].ToString()!,
                        StoreId = reader["store_id"].ToString()!,
                        CreatedAt = reader["created_at"].ToString()!,
                        Status = reader["status"].ToString()!
                    };

                    order.OrderItems = GetOrderItems(connection, order.Id);

                    orders.Add(order);
                }
            }

            return orders;
        }

        private List<OrderItem> GetOrderItems(SqliteConnection connection, string orderId)
        {
            var items = new List<OrderItem>();

            var query = "SELECT * FROM order_items WHERE order_id = @orderId";
            var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@orderId", orderId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new OrderItem
                    {
                        ItemId = reader["item_id"].ToString()!,
                        ItemName = reader["item_name"].ToString()!,
                        UnitPrice = Convert.ToInt32(reader["unit_price"]),
                        Quantity = Convert.ToInt32(reader["quantity"])
                    });
                }
            }

            return items;
        }

        public List<Store> GetAllStores()
        {
            var stores = new List<Store>();

            using (var connection = GetConnection())
            {
                connection.Open();

                var storeQuery = "SELECT * FROM stores";
                var storeCmd = new SqliteCommand(storeQuery, connection);

                using (var reader = storeCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var store = new Store
                        {
                            StoreId = reader["store_id"].ToString()!,
                            OwnerId = reader["owner_id"].ToString()!,
                            Name = reader["name"].ToString()!,
                            Address = reader["address"]?.ToString(),
                            Phone = reader["phone"]?.ToString()
                        };

                        store.Items = GetItemsByStoreId(connection, store.StoreId);
                        store.Orders = GetOrdersByStoreId(connection, store.StoreId);

                        stores.Add(store);
                    }
                }
            }

            return stores;
        }

        public List<Item> GetAllItems()
        {
            var items = new List<Item>();

            using (var connection = GetConnection())
            {
                connection.Open();

                var itemQuery = "SELECT * FROM items";
                var storeCmd = new SqliteCommand(itemQuery, connection);

                using (var reader = storeCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Item
                        {
                            Id = reader["id"].ToString()!,
                            Name = reader["name"].ToString()!,
                            Price = int.Parse(reader["price"].ToString()!),
                            Stock = int.Parse(reader["stock"].ToString()!),
                            Category = reader["category"].ToString()!,
                            StoreId = reader["store_id"].ToString()!,
                        };

                        items.Add(item);
                    }
                }
            }

            return items;
        }

        // ── Update user balance in DB ─────────────────────────────────────────
        public void UpdateUserBalance(string userId, decimal newBalance)
        {
            using var connection = GetConnection();
            connection.Open();
            var cmd = new SqliteCommand("UPDATE users SET balance = @balance WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("@balance", newBalance);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }

        // ── Get user balance from DB ───────────────────────────────────────────
        public decimal GetUserBalance(string userId)
        {
            using var connection = GetConnection();
            connection.Open();
            var cmd = new SqliteCommand("SELECT balance FROM users WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("@id", userId);
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        // ── Insert Order ──────────────────────────────────────────────────────
        public void InsertOrder(string orderId, string customerId, string storeId, string status, string createdAt)
        {
            using var connection = GetConnection();
            connection.Open();
            var cmd = new SqliteCommand(
                "INSERT INTO orders (id, customer_id, store_id, status, created_at) VALUES (@id, @cid, @sid, @status, @cat)",
                connection);
            cmd.Parameters.AddWithValue("@id", orderId);
            cmd.Parameters.AddWithValue("@cid", customerId);
            cmd.Parameters.AddWithValue("@sid", storeId);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@cat", createdAt);
            cmd.ExecuteNonQuery();
        }

        // ── Insert Order Item ─────────────────────────────────────────────────
        public void InsertOrderItem(string orderId, string itemId, string itemName, int unitPrice, int quantity)
        {
            using var connection = GetConnection();
            connection.Open();
            var cmd = new SqliteCommand(
                "INSERT INTO order_items (order_id, item_id, item_name, unit_price, quantity) VALUES (@oid, @iid, @iname, @price, @qty)",
                connection);
            cmd.Parameters.AddWithValue("@oid", orderId);
            cmd.Parameters.AddWithValue("@iid", itemId);
            cmd.Parameters.AddWithValue("@iname", itemName);
            cmd.Parameters.AddWithValue("@price", unitPrice);
            cmd.Parameters.AddWithValue("@qty", quantity);
            cmd.ExecuteNonQuery();
        }

        // ── Get Orders by Customer (with items + store name) ─────────────────
        public List<AstroBoy.Utils.OrderRecord> GetOrdersByCustomer(string customerId)
        {
            var records = new List<AstroBoy.Utils.OrderRecord>();

            using var connection = GetConnection();
            connection.Open();

            var cmd = new SqliteCommand(
                @"SELECT o.id, o.store_id, o.status, o.created_at, s.name as store_name
                  FROM orders o
                  LEFT JOIN stores s ON o.store_id = s.store_id
                  WHERE o.customer_id = @cid
                  ORDER BY o.created_at DESC",
                connection);
            cmd.Parameters.AddWithValue("@cid", customerId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var orderId = reader["id"].ToString()!;
                var record = new AstroBoy.Utils.OrderRecord
                {
                    OrderId = orderId,
                    StoreName = reader["store_name"]?.ToString() ?? "-",
                    Status = reader["status"].ToString()!,
                    OrderDate = DateTime.TryParse(reader["created_at"].ToString(), out var dt) ? dt : DateTime.Now,
                    Items = GetOrderItemRecords(connection, orderId)
                };
                record.Total = record.Items.Sum(i => i.Price * i.Qty);
                records.Add(record);
            }

            return records;
        }

        private List<AstroBoy.Utils.OrderItemRecord> GetOrderItemRecords(SqliteConnection connection, string orderId)
        {
            var items = new List<AstroBoy.Utils.OrderItemRecord>();

            var cmd = new SqliteCommand(
                "SELECT item_id, item_name, unit_price, quantity FROM order_items WHERE order_id = @oid",
                connection);
            cmd.Parameters.AddWithValue("@oid", orderId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new AstroBoy.Utils.OrderItemRecord
                {
                    ProductName = reader["item_name"].ToString()!,
                    ImageSource = reader["item_id"].ToString()!,
                    Price = Convert.ToDecimal(reader["unit_price"]),
                    Qty = Convert.ToInt32(reader["quantity"])
                });
            }

            return items;
        }
    }
}