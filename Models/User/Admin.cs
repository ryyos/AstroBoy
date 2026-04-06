namespace AstroBoy.Models;

public class Admin : Staff
{
    public Admin(string name, string email, string password) : base(name, email, password)
    {
    }
}