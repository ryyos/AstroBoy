namespace AstroBoy.Models;

public abstract class Staff : User
{
    protected Staff(string name, string email, string password) : base(name, email, password)
    {
    }

    public Guid StafId { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; set; }
}