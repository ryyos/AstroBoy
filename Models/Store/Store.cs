namespace AstroBoy.Models;

public class Store
{
    public string? StoreId { get; set; }
    public string? OwnerId { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public List<Item>? Items { get; set; } = new();
    public List<Order>? Orders { get; set; } = new();
}
