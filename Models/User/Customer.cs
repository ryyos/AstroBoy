namespace AstroBoy.Models;

public class Customer : User
{
    public Customer(string name, string email, string password, string role = "customer", string? id = null) : base(name, email, password, role, id)
    {
    }
}