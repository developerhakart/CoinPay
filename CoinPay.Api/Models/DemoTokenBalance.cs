using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoinPay.Api.Models;

/// <summary>
/// Represents demo token balances for users (DUSDT, DBTC)
/// Demo tokens are test tokens provided by WhiteBIT for testing investment features
/// </summary>
public class DemoTokenBalance
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// User who owns this demo token balance
    /// </summary>
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Token symbol (DUSDT or DBTC)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string TokenSymbol { get; set; } = string.Empty;

    /// <summary>
    /// Current balance of demo tokens
    /// </summary>
    [Column(TypeName = "decimal(18, 8)")]
    public decimal Balance { get; set; }

    /// <summary>
    /// Total amount of demo tokens ever issued to this user
    /// </summary>
    [Column(TypeName = "decimal(18, 8)")]
    public decimal TotalIssued { get; set; }

    /// <summary>
    /// Total amount of demo tokens used in investments
    /// </summary>
    [Column(TypeName = "decimal(18, 8)")]
    public decimal TotalInvested { get; set; }

    /// <summary>
    /// Whether this demo token is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the demo token balance was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last time the balance was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Notes or metadata about this demo token balance
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}
