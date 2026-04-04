public abstract class User
{
    public Guid Id { get; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public abstract bool Login(string email, string password);

    public abstract bool Register(string name, string email, string password);

    public abstract bool Logout();
}