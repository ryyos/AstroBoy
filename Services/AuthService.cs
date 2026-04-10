using AstroBoy.Models;

namespace AstroBoy.Services;

public class AuthService
{
    public User Login(string username, string password)
    {
        if (username == "admin" && password == "123")
            // role ntar dapet dari database
            return new Admin(username, username, password, "admin");

        if (username == "owner" && password == "123")
            return new Owner(username, username, password, "owner");

        return new Customer(username, username, password, "customer");
    }
} 