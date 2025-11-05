namespace CoinPay.Api.Models;

public class ExchangeConnection
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int UserId1 { get; set; } // EF Core shadow property - actual FK to Users.Id
    public string ExchangeName { get; set; } = string.Empty; // "whitebit"
    public string ApiKeyEncrypted { get; set; } = string.Empty;
    public string ApiSecretEncrypted { get; set; } = string.Empty;
    public string? EncryptionKeyId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastValidatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}
