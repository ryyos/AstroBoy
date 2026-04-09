namespace AstroBoy.Models;

public class Owner : Staff
{
    public Owner(string name, string email, string password, string role) : base(name, email, password, role)
    {
    }
}