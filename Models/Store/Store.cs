namespace AstroBoy.Models;

public class Store
{
    public required string StoreId { get; set; }
    public required string OwnerId { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public List<Item> Items { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}
