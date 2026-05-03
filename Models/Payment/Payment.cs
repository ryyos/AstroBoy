public class Payment
{
    public required string PaymentId { get; set; }
    public required string OrderId { get; set; }
    public required float Amount { get; set; }
    public required string Method { get; set; }
    public required string Status { get; set; }
    public required string PaidAt { get; set; }
}