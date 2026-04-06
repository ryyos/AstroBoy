public class OrderItem
{
    public required string ItemId { get; set; }
    public required string ItemName { get; set; }
    public required int UnitPrice { get; set; }
    public required int Quantity { get; set; }

    public void GetSubtotal()
    {

    }
}