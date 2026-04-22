namespace AstroBoy.Models;

public abstract class Staff : User
{
    protected Staff(string name, string email, string password, string role, string? Id = null) : base(name, email, password, role, Id)
    {
    }

    public Guid StafId { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; set; }
}