public class Order
{
    public required string Id { get; set; }
    public required string CustomerId { get; set; }
    public required string StoreId { get; set; }
    public required string CreatedAt { get; set; }
    public required string Status { get; set; }
    public List<OrderItem>? orderItems { get; set; }
}