namespace AstroBoy.Models;

public abstract class Staff : User
{
    protected Staff(string name, string email, string password, string role) : base(name, email, password, role)
    {
    }

    public Guid StafId { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; set; }
}