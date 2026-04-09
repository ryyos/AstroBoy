namespace AstroBoy.Models;

public abstract class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public User(string name, string email, string password, string role)
    {
        this.Name = name;
        this.Email = email;
        this.Password = password;
        this.Id = Email;
        Role = role;
    }
}