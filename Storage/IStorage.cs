public interface IStorage<T>
{
    public T Save();
    public bool Delete();
}