using CoinPay.Api.Models;

namespace CoinPay.Api.DTOs;

/// <summary>
/// Request for getting swap quote
/// </summary>
public class GetSwapQuoteRequest
{
    public string FromToken { get; set; } = string.Empty;
    public string ToToken { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Slippage { get; set; } = 1.0m;
}

/// <summary>
/// Response for swap quote
/// </summary>
public class SwapQuoteResponse
{
    public string FromToken { get; set; } = string.Empty;
    public string FromTokenSymbol { get; set; } = string.Empty;
    public string ToToken { get; set; } = string.Empty;
    public string ToTokenSymbol { get; set; } = string.Empty;
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal PlatformFeePercentage { get; set; }
    public string EstimatedGas { get; set; } = string.Empty;
    public decimal EstimatedGasCost { get; set; }
    public decimal PriceImpact { get; set; }
    public decimal SlippageTolerance { get; set; }
    public decimal MinimumReceived { get; set; }
    public DateTime QuoteValidUntil { get; set; }
    public string Provider { get; set; } = string.Empty;
}

/// <summary>
/// Request for executing swap
/// </summary>
public class ExecuteSwapRequest
{
    public string FromToken { get; set; } = string.Empty;
    public string ToToken { get; set; } = string.Empty;
    public decimal FromAmount { get; set; }
    public decimal SlippageTolerance { get; set; } = 1.0m;
    public Guid? QuoteId { get; set; } // Optional, for quote verification
}

/// <summary>
/// Response for swap execution
/// </summary>
public class SwapExecutionResponse
{
    public Guid SwapId { get; set; }
    public string? TransactionHash { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FromToken { get; set; } = string.Empty;
    public string FromTokenSymbol { get; set; } = string.Empty;
    public string ToToken { get; set; } = string.Empty;
    public string ToTokenSymbol { get; set; } = string.Empty;
    public decimal FromAmount { get; set; }
    public decimal ExpectedToAmount { get; set; }
    public decimal MinimumReceived { get; set; }
    public decimal PlatformFee { get; set; }
    public string EstimatedConfirmationTime { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Response for swap history
/// </summary>
public class SwapHistoryResponse
{
    public List<SwapHistoryItem> Swaps { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Individual swap history item
/// </summary>
public class SwapHistoryItem
{
    public Guid Id { get; set; }
    public string FromToken { get; set; } = string.Empty;
    public string FromTokenSymbol { get; set; } = string.Empty;
    public string ToToken { get; set; } = string.Empty;
    public string ToTokenSymbol { get; set; } = string.Empty;
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal PlatformFee { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? TransactionHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

/// <summary>
/// Response for swap details
/// </summary>
public class SwapDetailsResponse
{
    public Guid Id { get; set; }
    public string FromToken { get; set; } = string.Empty;
    public string FromTokenSymbol { get; set; } = string.Empty;
    public string ToToken { get; set; } = string.Empty;
    public string ToTokenSymbol { get; set; } = string.Empty;
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal PlatformFeePercentage { get; set; }
    public string? GasUsed { get; set; }
    public decimal? GasCost { get; set; }
    public decimal SlippageTolerance { get; set; }
    public decimal? PriceImpact { get; set; }
    public decimal MinimumReceived { get; set; }
    public string DexProvider { get; set; } = string.Empty;
    public string? TransactionHash { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}
