namespace CoinPay.Api.Models;

public class Transaction
{
    public int Id { get; set; }
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Type { get; set; } = "Payment"; // Payment, Refund, Transfer
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
    public string? SenderName { get; set; }
    public string? ReceiverName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
