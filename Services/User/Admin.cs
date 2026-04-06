public class Admin : Staff
{
    public override bool Login(string email, string password)
    {
        throw new NotImplementedException();
    }

    public override bool Logout()
    {
        throw new NotImplementedException();
    }

    public override bool Register(string name, string email, string password)
    {
        throw new NotImplementedException();
    }

    private List<Owner> ViewAllOwners()
    {
        List<Owner> owners = new List<Owner>();
        return owners;
    }
}