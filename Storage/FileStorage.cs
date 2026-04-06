
public interface FileStorage : IStorage<string>
{
    new string Save()
    {
        return "";
    }
    public new bool Delete()
    {
        return true;
    }
}