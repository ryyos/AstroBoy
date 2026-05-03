public class ShoppingCart
{
    public required string Id { get; set; }
    public required string CustomerId { get; set; }
    public List<CartItem>? cartItems;
}