using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CoinPay.Api.DTOs.Exchange;

namespace CoinPay.Api.Services.Exchange.WhiteBit;

/// <summary>
/// WhiteBit API client implementation with HMAC-SHA256 authentication
/// </summary>
public class WhiteBitApiClient : IWhiteBitApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WhiteBitApiClient> _logger;
    private readonly string _baseUrl;
    private readonly bool _useMockMode;

    public WhiteBitApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<WhiteBitApiClient> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _baseUrl = configuration["WhiteBit:BaseUrl"] ?? "https://whitebit.com";
        _useMockMode = configuration.GetValue<bool>("WhiteBit:UseMockMode", false);

        if (_useMockMode)
        {
            _logger.LogWarning("⚠️ WhiteBit API client running in MOCK MODE - credentials will not be validated against real API");
        }
    }

    public async Task<bool> TestConnectionAsync(string apiKey, string apiSecret)
    {
        try
        {
            // In mock mode, accept any non-empty credentials
            if (_useMockMode)
            {
                var isValid = !string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(apiSecret);

                if (isValid)
                {
                    _logger.LogInformation("Mock mode: Accepting credentials without real API validation");
                }
                else
                {
                    _logger.LogWarning("Mock mode: Rejecting empty credentials");
                }

                return isValid;
            }

            // In production mode, validate against real WhiteBit API
            _logger.LogInformation("Production mode: Validating credentials against WhiteBit API");
            var response = await GetBalanceAsync(apiKey, apiSecret);
            return response != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test WhiteBit connection");
            return false;
        }
    }

    public async Task<WhiteBitBalanceResponse> GetBalanceAsync(string apiKey, string apiSecret)
    {
        return await SendRequestAsync<WhiteBitBalanceResponse>(
            "/api/v4/main-account/balance",
            HttpMethod.Post,
            apiKey,
            apiSecret,
            new { });
    }

    public async Task<WhiteBitPlansResponse> GetInvestmentPlansAsync(string apiKey, string apiSecret)
    {
        // Mock implementation - WhiteBit may have different endpoint
        // For MVP, we'll return a hardcoded USDC Flex plan
        var mockResponse = new WhiteBitPlansResponse
        {
            Plans = new List<WhiteBitPlan>
            {
                new WhiteBitPlan
                {
                    PlanId = "flex-usdc-1",
                    Asset = "USDC",
                    Apy = 8.50m,
                    MinAmount = 100m,
                    MaxAmount = 100000m,
                    Term = "flexible",
                    Description = "Flexible USDC Flex Plan - Earn 8.5% APY with no lock-up period"
                }
            }
        };

        return await Task.FromResult(mockResponse);
    }

    public async Task<WhiteBitInvestmentsResponse> GetInvestmentsAsync(string apiKey, string apiSecret)
    {
        // This would call WhiteBit's actual investment list endpoint
        // For MVP, returning empty list
        return await Task.FromResult(new WhiteBitInvestmentsResponse
        {
            Investments = new List<WhiteBitInvestment>()
        });
    }

    public async Task<WhiteBitCreateInvestmentResponse> CreateInvestmentAsync(
        string apiKey,
        string apiSecret,
        string planId,
        decimal amount)
    {
        var body = new
        {
            plan_id = planId,
            amount = amount.ToString("F8"),
            asset = "USDC"
        };

        // Mock implementation for MVP
        return await Task.FromResult(new WhiteBitCreateInvestmentResponse
        {
            InvestmentId = Guid.NewGuid().ToString(),
            PlanId = planId,
            Amount = amount,
            Asset = "USDC",
            Status = "active",
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<WhiteBitCloseInvestmentResponse> CloseInvestmentAsync(
        string apiKey,
        string apiSecret,
        string investmentId)
    {
        // Mock implementation for MVP
        return await Task.FromResult(new WhiteBitCloseInvestmentResponse
        {
            InvestmentId = investmentId,
            Status = "closed",
            FinalAmount = 0m, // Would come from actual API
            ClosedAt = DateTime.UtcNow
        });
    }

    public async Task<WhiteBitDepositAddressResponse> GetDepositAddressAsync(
        string apiKey,
        string apiSecret,
        string asset)
    {
        var body = new { asset = asset, network = "Polygon" };

        // Mock implementation - return test address
        return await Task.FromResult(new WhiteBitDepositAddressResponse
        {
            Asset = asset,
            Address = "0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb",
            Network = "Polygon",
            Memo = null
        });
    }

    private async Task<T> SendRequestAsync<T>(
        string endpoint,
        HttpMethod method,
        string apiKey,
        string apiSecret,
        object? body = null)
    {
        try
        {
            var nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var bodyJson = body != null ? JsonSerializer.Serialize(body) : "";

            // Generate HMAC-SHA256 signature
            var signature = GenerateSignature(apiSecret, endpoint, nonce.ToString(), bodyJson);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(method, $"{_baseUrl}{endpoint}");

            request.Headers.Add("X-TXC-APIKEY", apiKey);
            request.Headers.Add("X-TXC-PAYLOAD", Convert.ToBase64String(Encoding.UTF8.GetBytes(bodyJson)));
            request.Headers.Add("X-TXC-SIGNATURE", signature);

            if (body != null && (method == HttpMethod.Post || method == HttpMethod.Put))
            {
                request.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            }

            _logger.LogInformation("Sending WhiteBit API request to {Endpoint}", endpoint);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("WhiteBit API request failed. Status: {Status}, Response: {Response}",
                    response.StatusCode, content);
                throw new Exception($"WhiteBit API request failed: {response.StatusCode}");
            }

            var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? throw new Exception("Failed to deserialize WhiteBit API response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending WhiteBit API request to {Endpoint}", endpoint);
            throw;
        }
    }

    private string GenerateSignature(string apiSecret, string path, string nonce, string body)
    {
        var message = $"{path}{nonce}{body}";
        var keyBytes = Encoding.UTF8.GetBytes(apiSecret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
