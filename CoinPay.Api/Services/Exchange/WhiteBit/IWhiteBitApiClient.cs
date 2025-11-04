using CoinPay.Api.DTOs.Exchange;

namespace CoinPay.Api.Services.Exchange.WhiteBit;

/// <summary>
/// Interface for WhiteBit API client
/// </summary>
public interface IWhiteBitApiClient
{
    /// <summary>
    /// Test connection with provided credentials
    /// </summary>
    Task<bool> TestConnectionAsync(string apiKey, string apiSecret);

    /// <summary>
    /// Get account balance
    /// </summary>
    Task<WhiteBitBalanceResponse> GetBalanceAsync(string apiKey, string apiSecret);

    /// <summary>
    /// Get available investment plans
    /// </summary>
    Task<WhiteBitPlansResponse> GetInvestmentPlansAsync(string apiKey, string apiSecret);

    /// <summary>
    /// Get all investments for account
    /// </summary>
    Task<WhiteBitInvestmentsResponse> GetInvestmentsAsync(string apiKey, string apiSecret);

    /// <summary>
    /// Create new investment
    /// </summary>
    Task<WhiteBitCreateInvestmentResponse> CreateInvestmentAsync(
        string apiKey,
        string apiSecret,
        string planId,
        decimal amount);

    /// <summary>
    /// Close/withdraw investment
    /// </summary>
    Task<WhiteBitCloseInvestmentResponse> CloseInvestmentAsync(
        string apiKey,
        string apiSecret,
        string investmentId);

    /// <summary>
    /// Get deposit address for asset
    /// </summary>
    Task<WhiteBitDepositAddressResponse> GetDepositAddressAsync(
        string apiKey,
        string apiSecret,
        string asset);
}
