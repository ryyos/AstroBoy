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
                        if(reader["role"].ToString() == "admin")
                        {
                            return new Admin
                            (
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!,
                                Id: reader["id"].ToString()!
                            );
                        } else if (reader["role"].ToString() == "owner")
                        {
                            return new Owner
                            (
                                Id: reader["id"].ToString()!,
                                name: reader["name"].ToString()!,
                                email: reader["email"].ToString()!,
                                password: reader["password"].ToString()!,
                                role: reader["role"].ToString()!
                            );
                        } else if (reader["role"].ToString() == "customer")
                        {
                            return new Customer
                            (
                                Id: reader["id"].ToString()!,
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
    }
}