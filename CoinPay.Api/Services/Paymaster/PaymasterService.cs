using CoinPay.Api.Services.UserOperation;

namespace CoinPay.Api.Services.Paymaster;

/// <summary>
/// Implementation of Circle Paymaster service for gas sponsorship
/// </summary>
public class PaymasterService : IPaymasterService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymasterService> _logger;

    private const string PAYMASTER_HTTP_CLIENT = "CirclePaymaster";

    public PaymasterService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<PaymasterService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetPaymasterDataAsync(UserOperationDto userOp, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting paymaster sponsorship for sender {Sender}", userOp.Sender);

        var paymasterUrl = _configuration["Circle:PaymasterUrl"];

        if (string.IsNullOrEmpty(paymasterUrl))
        {
            _logger.LogWarning("Paymaster URL not configured, using mock paymaster data");
            return GetMockPaymasterData();
        }

        try
        {
            var client = _httpClientFactory.CreateClient(PAYMASTER_HTTP_CLIENT);

            var request = new
            {
                jsonrpc = "2.0",
                id = 1,
                method = "pm_sponsorUserOperation",
                @params = new object[]
                {
                    userOp,
                    new
                    {
                        type = "payg", // Pay-as-you-go sponsorship
                        policyId = _configuration["Circle:PaymasterPolicyId"] ?? "default"
                    }
                }
            };

            var response = await client.PostAsJsonAsync("", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Paymaster request failed with status {StatusCode}", response.StatusCode);
                return GetMockPaymasterData();
            }

            var result = await response.Content.ReadFromJsonAsync<PaymasterResponse>(cancellationToken);

            if (result?.PaymasterAndData == null)
            {
                _logger.LogWarning("Paymaster response missing data, using mock");
                return GetMockPaymasterData();
            }

            _logger.LogInformation("Paymaster sponsorship approved for sender {Sender}", userOp.Sender);

            return result.PaymasterAndData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting paymaster data, using mock");
            return GetMockPaymasterData();
        }
    }

    public async Task<bool> VerifySponsorshipAsync(string userOpHash, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying sponsorship for UserOpHash {UserOpHash}", userOpHash);

        // In a real implementation, query the paymaster service to verify sponsorship
        // For now, always return true (assuming all operations are sponsored)

        await Task.CompletedTask; // Simulate async operation

        return true;
    }

    public async Task<PaymasterStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching paymaster status");

        var paymasterAddress = _configuration["Circle:PaymasterAddress"] ?? "0x0000000000000000000000000000000000000000";

        // In a real implementation, query the paymaster contract for balance and status
        // For now, return mock data

        await Task.CompletedTask; // Simulate async operation

        return new PaymasterStatus
        {
            PaymasterAddress = paymasterAddress,
            Balance = 1000000m, // 1M USDC mock balance
            IsActive = true,
            DailySponsorshipLimit = 10000,
            DailySponsorshipUsed = 42
        };
    }

    #region Helper Methods

    private string GetMockPaymasterData()
    {
        // Mock paymaster data structure for testing
        // Format: <paymaster_address><valid_until><valid_after><signature>

        var paymasterAddress = _configuration["Circle:PaymasterAddress"] ?? "0x0000000000000000000000000000000000000000";

        // Remove 0x prefix if present
        paymasterAddress = paymasterAddress.Replace("0x", "");

        // Valid until (timestamp + 1 hour in hex)
        var validUntil = ((DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600).ToString("X")).PadLeft(64, '0');

        // Valid after (current timestamp in hex)
        var validAfter = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString("X").PadLeft(64, '0');

        // Mock signature (64 bytes)
        var signature = new string('0', 128);

        return "0x" + paymasterAddress.PadLeft(40, '0') + validUntil + validAfter + signature;
    }

    #endregion

    #region Response Models

    private class PaymasterResponse
    {
        public string? PaymasterAndData { get; set; }
        public string? Context { get; set; }
    }

    #endregion
}

/// <summary>
/// Mock Paymaster Service for development/testing
/// Always approves gas sponsorship without external calls
/// </summary>
public class MockPaymasterService : IPaymasterService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MockPaymasterService> _logger;

    public MockPaymasterService(IConfiguration configuration, ILogger<MockPaymasterService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<string> GetPaymasterDataAsync(UserOperationDto userOp, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK] Approving gas sponsorship for sender {Sender}", userOp.Sender);

        var paymasterAddress = _configuration["Circle:PaymasterAddress"] ?? "0x0000000000000000000000000000000000000000";
        paymasterAddress = paymasterAddress.Replace("0x", "");

        var validUntil = ((DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600).ToString("X")).PadLeft(64, '0');
        var validAfter = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString("X").PadLeft(64, '0');
        var signature = new string('0', 128);

        var paymasterData = "0x" + paymasterAddress.PadLeft(40, '0') + validUntil + validAfter + signature;

        return Task.FromResult(paymasterData);
    }

    public Task<bool> VerifySponsorshipAsync(string userOpHash, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK] Sponsorship verified for {UserOpHash}", userOpHash);
        return Task.FromResult(true);
    }

    public Task<PaymasterStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK] Fetching paymaster status");

        return Task.FromResult(new PaymasterStatus
        {
            PaymasterAddress = _configuration["Circle:PaymasterAddress"] ?? "0x0000000000000000000000000000000000000000",
            Balance = 1000000m,
            IsActive = true,
            DailySponsorshipLimit = 10000,
            DailySponsorshipUsed = 0
        });
    }
}
