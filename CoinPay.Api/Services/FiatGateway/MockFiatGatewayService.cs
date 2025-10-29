namespace CoinPay.Api.Services.FiatGateway;

/// <summary>
/// Mock Fiat Gateway Service for MVP testing
/// Simulates exchange rates and payout processing without real API calls
/// </summary>
public class MockFiatGatewayService : IFiatGatewayService
{
    private readonly ILogger<MockFiatGatewayService> _logger;
    private static readonly Dictionary<string, PayoutStatusResponse> _mockPayouts = new();

    // Mock exchange rate: 1 USDC = 0.9998 USD (simulating small deviation from 1:1)
    private const decimal MockExchangeRate = 0.9998m;

    // Fee structure
    private const decimal ConversionFeePercent = 0.015m; // 1.5%
    private const decimal PayoutFlatFee = 1.00m; // $1.00 per payout

    public MockFiatGatewayService(ILogger<MockFiatGatewayService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get current exchange rate
    /// </summary>
    public Task<ExchangeRateResponse> GetExchangeRateAsync()
    {
        _logger.LogInformation("MockFiatGateway: Fetching exchange rate");

        var response = new ExchangeRateResponse
        {
            Rate = MockExchangeRate,
            BaseCurrency = "USDC",
            QuoteCurrency = "USD",
            Timestamp = DateTime.UtcNow,
            ValidForSeconds = 30
        };

        return Task.FromResult(response);
    }

    /// <summary>
    /// Calculate conversion preview with fees
    /// </summary>
    public Task<ConversionPreviewResponse> GetConversionPreviewAsync(decimal usdcAmount)
    {
        _logger.LogInformation("MockFiatGateway: Calculating conversion preview for {UsdcAmount} USDC", usdcAmount);

        // Convert USDC to USD
        var usdBeforeFees = usdcAmount * MockExchangeRate;

        // Calculate fees
        var conversionFee = usdBeforeFees * ConversionFeePercent;
        var payoutFee = PayoutFlatFee;
        var totalFees = conversionFee + payoutFee;

        // Net amount after fees
        var netAmount = usdBeforeFees - totalFees;

        var response = new ConversionPreviewResponse
        {
            UsdcAmount = usdcAmount,
            ExchangeRate = MockExchangeRate,
            UsdAmountBeforeFees = usdBeforeFees,
            ConversionFeePercent = ConversionFeePercent * 100, // Convert to percentage
            ConversionFeeAmount = conversionFee,
            PayoutFeeAmount = payoutFee,
            TotalFees = totalFees,
            NetUsdAmount = netAmount,
            ExpiresAt = DateTime.UtcNow.AddSeconds(30)
        };

        return Task.FromResult(response);
    }

    /// <summary>
    /// Initiate mock payout
    /// </summary>
    public Task<PayoutInitiationResponse> InitiatePayoutAsync(PayoutInitiationRequest request)
    {
        _logger.LogInformation("MockFiatGateway: Initiating payout for user {UserId}, amount {UsdcAmount} USDC",
            request.UserId, request.UsdcAmount);

        // Generate mock transaction ID
        var gatewayTxId = $"MOCK_PAYOUT_{Guid.NewGuid():N}";

        // Calculate conversion
        var preview = GetConversionPreviewAsync(request.UsdcAmount).Result;

        // Create response
        var response = new PayoutInitiationResponse
        {
            Success = true,
            GatewayTransactionId = gatewayTxId,
            Status = "pending",
            UsdcAmount = request.UsdcAmount,
            UsdAmount = preview.UsdAmountBeforeFees,
            ExchangeRate = preview.ExchangeRate,
            TotalFees = preview.TotalFees,
            NetAmount = preview.NetUsdAmount,
            EstimatedArrival = DateTime.UtcNow.AddDays(3) // Mock 3 business days
        };

        // Store mock payout status
        _mockPayouts[gatewayTxId] = new PayoutStatusResponse
        {
            GatewayTransactionId = gatewayTxId,
            Status = "pending",
            EstimatedArrival = response.EstimatedArrival,
            StatusDetails = new PayoutStatusDetails
            {
                Stage = "initiated",
                LastUpdated = DateTime.UtcNow,
                Events = new List<PayoutStatusEvent>
                {
                    new PayoutStatusEvent
                    {
                        Event = "INITIATED",
                        Timestamp = DateTime.UtcNow,
                        Description = "Payout initiated"
                    }
                }
            }
        };

        _logger.LogInformation("MockFiatGateway: Payout initiated successfully. Gateway TX ID: {GatewayTxId}", gatewayTxId);

        return Task.FromResult(response);
    }

    /// <summary>
    /// Get mock payout status
    /// </summary>
    public Task<PayoutStatusResponse> GetPayoutStatusAsync(string gatewayTransactionId)
    {
        _logger.LogInformation("MockFiatGateway: Fetching status for transaction {GatewayTxId}", gatewayTransactionId);

        if (_mockPayouts.TryGetValue(gatewayTransactionId, out var status))
        {
            // Simulate status progression based on age
            var age = DateTime.UtcNow - status.StatusDetails!.LastUpdated;

            if (status.Status == "pending" && age.TotalSeconds > 10)
            {
                // After 10 seconds, move to processing
                status.Status = "processing";
                status.StatusDetails.Stage = "converting";
                status.StatusDetails.LastUpdated = DateTime.UtcNow;
                status.StatusDetails.Events.Add(new PayoutStatusEvent
                {
                    Event = "PROCESSING",
                    Timestamp = DateTime.UtcNow,
                    Description = "Converting USDC to USD"
                });
            }
            else if (status.Status == "processing" && age.TotalSeconds > 30)
            {
                // After 30 seconds, mark as completed
                status.Status = "completed";
                status.StatusDetails.Stage = "completed";
                status.CompletedAt = DateTime.UtcNow;
                status.StatusDetails.LastUpdated = DateTime.UtcNow;
                status.StatusDetails.Events.Add(new PayoutStatusEvent
                {
                    Event = "COMPLETED",
                    Timestamp = DateTime.UtcNow,
                    Description = "Payout completed successfully"
                });
            }

            return Task.FromResult(status);
        }

        // Transaction not found
        throw new InvalidOperationException($"Gateway transaction {gatewayTransactionId} not found");
    }

    /// <summary>
    /// Verify mock gateway health
    /// </summary>
    public Task<bool> VerifyGatewayHealthAsync()
    {
        _logger.LogInformation("MockFiatGateway: Verifying gateway health");
        return Task.FromResult(true); // Mock gateway is always healthy
    }
}
