namespace AstroBoy.Models;

public class Admin : Staff
{
    public Admin(string name, string email, string password, string role = "admin", string? id = null) : base(name, email, password, role, id)
    {
    }
}