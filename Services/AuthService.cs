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
        {
            var owner = new Owner(username, username, password, "owner")
            {
                Balance = 2_500_000
            };
            return owner;
        }

        return new Customer(username, username, password, "customer");
    }
} 