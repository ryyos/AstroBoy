using AstroBoy.Models;

namespace AstroBoy.Services;

public class AuthService
{
    public User Login(string username, string password)
    {
        if (username == "admin" && password == "123")
            return new Admin(username, username, password);

        if (username == "owner" && password == "123")
            return new Owner(username, username, password);

        return new Customer(username, username, password);
    }
}