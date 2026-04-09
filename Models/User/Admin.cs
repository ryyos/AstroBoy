namespace AstroBoy.Models;

public class Admin : Staff
{
    public Admin(string name, string email, string password, string role) : base(name, email, password, role)
    {
    }
}