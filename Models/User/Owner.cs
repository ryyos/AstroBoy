namespace AstroBoy.Models;

public class Owner : Staff
{
    public Owner(string name, string email, string password, string role = "owner", string? id = null) : base(name, email, password, role, id)
    {
    }
}