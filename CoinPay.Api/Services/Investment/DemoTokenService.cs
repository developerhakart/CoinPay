using CoinPay.Api.Models;
using CoinPay.Api.Repositories;

namespace CoinPay.Api.Services.Investment;

/// <summary>
/// Service for managing WhiteBIT demo tokens (DUSDT, DBTC)
/// Demo tokens are test tokens provided by WhiteBIT for testing investment features
/// </summary>
public class DemoTokenService : IDemoTokenService
{
    private readonly IDemoTokenRepository _demoTokenRepository;
    private readonly ILogger<DemoTokenService> _logger;

    // Supported demo tokens from WhiteBIT
    private static readonly List<string> SupportedTokens = new() { "DUSDT", "DBTC" };

    public DemoTokenService(
        IDemoTokenRepository demoTokenRepository,
        ILogger<DemoTokenService> logger)
    {
        _demoTokenRepository = demoTokenRepository;
        _logger = logger;
    }

    public async Task<List<DemoTokenBalance>> GetUserBalancesAsync(int userId)
    {
        _logger.LogInformation("Fetching all demo token balances for user {UserId}", userId);
        return await _demoTokenRepository.GetByUserIdAsync(userId);
    }

    public async Task<DemoTokenBalance?> GetBalanceAsync(int userId, string tokenSymbol)
    {
        if (!IsDemoToken(tokenSymbol))
        {
            _logger.LogWarning("Attempted to get balance for unsupported token: {TokenSymbol}", tokenSymbol);
            return null;
        }

        return await _demoTokenRepository.GetByUserAndTokenAsync(userId, tokenSymbol);
    }

    public async Task<DemoTokenBalance> IssueTokensAsync(int userId, string tokenSymbol, decimal amount, string? notes = null)
    {
        if (!IsDemoToken(tokenSymbol))
        {
            throw new ArgumentException($"Token {tokenSymbol} is not a supported demo token", nameof(tokenSymbol));
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        _logger.LogInformation("Issuing {Amount} {TokenSymbol} demo tokens to user {UserId}",
            amount, tokenSymbol, userId);

        var balance = await _demoTokenRepository.IssueDemoTokensAsync(userId, tokenSymbol, amount);

        if (!string.IsNullOrWhiteSpace(notes))
        {
            balance.Notes = notes;
            await _demoTokenRepository.UpsertAsync(balance);
        }

        return balance;
    }

    public async Task<bool> HasSufficientBalanceAsync(int userId, string tokenSymbol, decimal amount)
    {
        if (!IsDemoToken(tokenSymbol))
        {
            _logger.LogWarning("Token {TokenSymbol} is not a demo token", tokenSymbol);
            return false;
        }

        return await _demoTokenRepository.HasSufficientBalanceAsync(userId, tokenSymbol, amount);
    }

    public async Task<bool> DeductBalanceAsync(int userId, string tokenSymbol, decimal amount)
    {
        if (!IsDemoToken(tokenSymbol))
        {
            _logger.LogWarning("Cannot deduct non-demo token: {TokenSymbol}", tokenSymbol);
            return false;
        }

        if (amount <= 0)
        {
            _logger.LogWarning("Deduct amount must be positive. Amount: {Amount}", amount);
            return false;
        }

        var hasSufficient = await HasSufficientBalanceAsync(userId, tokenSymbol, amount);
        if (!hasSufficient)
        {
            _logger.LogWarning("User {UserId} has insufficient {TokenSymbol} balance for deduction of {Amount}",
                userId, tokenSymbol, amount);
            return false;
        }

        return await _demoTokenRepository.DeductBalanceAsync(userId, tokenSymbol, amount);
    }

    public async Task<bool> AddBalanceAsync(int userId, string tokenSymbol, decimal amount)
    {
        if (!IsDemoToken(tokenSymbol))
        {
            _logger.LogWarning("Cannot add non-demo token: {TokenSymbol}", tokenSymbol);
            return false;
        }

        if (amount <= 0)
        {
            _logger.LogWarning("Add amount must be positive. Amount: {Amount}", amount);
            return false;
        }

        return await _demoTokenRepository.AddBalanceAsync(userId, tokenSymbol, amount);
    }

    public bool IsDemoToken(string tokenSymbol)
    {
        return SupportedTokens.Contains(tokenSymbol, StringComparer.OrdinalIgnoreCase);
    }

    public List<string> GetSupportedDemoTokens()
    {
        return new List<string>(SupportedTokens);
    }
}
