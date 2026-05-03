namespace AstroBoy.Models;

public abstract class Staff : User
{
    protected Staff(string name, string email, string password, string role, string? id = null) : base(name, email, password, role, id)
    {
    }
}