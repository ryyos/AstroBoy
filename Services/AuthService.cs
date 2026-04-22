using System.Linq.Expressions;
using AstroBoy.Models;
using Database;

namespace AstroBoy.Services;

public class AuthService
{
    private DatabaseContext db = new DatabaseContext();
    public AuthService()
    {
    }

    public User? Login(string email, string password)
    {
        var user = db.GetUser(email, password);
        if (user != null) 
            return user;
        return null;
    }

    public bool Register(string name, string email, string password, string role)
    {
        try
        {
            if(role == "admin")
            {
                db.InsertUser(
                    new Admin(
                        name: name,
                        email: email,
                        password: password
                    )
                );
            }
            else if (role == "owner")
            {
                db.InsertUser(
                    new Owner(
                        name: name,
                        email: email,
                        password: password
                    )
                );
            }
            else if (role == "customer")
            {
                db.InsertUser(
                   new Customer(
                       name: name,
                       email: email,
                       password: password
                   )
                );
            }

        } catch ( Exception ex ) {
            Console.WriteLine(ex.Message);
            return false;
        }
        return true;
    }
} 