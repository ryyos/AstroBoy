public abstract class Staff : User
{
    public Guid StafId { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; set; }
}