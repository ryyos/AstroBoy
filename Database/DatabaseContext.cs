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
                var query = @"SELECT id, name, email, password, role 
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
                        if (reader["role"].ToString() == "admin")
                        {
                            return new Admin
                            (
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!,
                                id: reader["id"].ToString()!
                            );
                        }
                        else if (reader["role"].ToString() == "owner")
                        {
                            return new Owner
                            (
                                id: reader["id"].ToString()!,
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!
                            );
                        }
                        else if (reader["role"].ToString() == "customer")
                        {
                            return new Customer
                            (
                                id: reader["id"].ToString()!,
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!
                            );
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
                            password: reader["password"].ToString()!,
                            id: reader["id"].ToString()!
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
    }
}