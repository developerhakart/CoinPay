using CoinPay.Api.Services.Circle.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using Polly;
using Polly.Retry;
using System.Net;

namespace CoinPay.Api.Services.Circle;

/// <summary>
/// Implementation of Circle Web3 Services SDK client for passkey-based wallet management.
/// Provides resilient HTTP communication with Circle's API using Polly retry policies.
/// </summary>
public class CircleService : ICircleService
{
    private readonly RestClient _client;
    private readonly CircleOptions _options;
    private readonly ILogger<CircleService> _logger;
    private readonly AsyncRetryPolicy<RestResponse> _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the CircleService.
    /// </summary>
    /// <param name="options">Circle configuration options</param>
    /// <param name="logger">Logger instance for structured logging</param>
    public CircleService(IOptions<CircleOptions> options, ILogger<CircleService> logger)
    {
        _options = options.Value;
        _logger = logger;

        // Initialize RestSharp client with base URL
        var restOptions = new RestClientOptions(_options.ApiUrl)
        {
            ThrowOnAnyError = false,
            MaxTimeout = 30000 // 30 seconds
        };
        _client = new RestClient(restOptions);

        // Configure Polly retry policy for transient failures
        _retryPolicy = Policy<RestResponse>
            .Handle<HttpRequestException>()
            .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout ||
                          r.StatusCode == HttpStatusCode.TooManyRequests ||
                          (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Circle API request failed with {StatusCode}. Retrying in {Delay}s (Attempt {RetryCount}/3)",
                        outcome.Result?.StatusCode,
                        timespan.TotalSeconds,
                        retryCount);
                });
    }

    /// <inheritdoc/>
    public async Task<CircleRegistrationChallengeResponse> InitiateUserRegistrationAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Initiating user registration for username: {Username} [CorrelationId: {CorrelationId}]",
            username,
            correlationId);

        var request = new RestRequest("/users/challenge/registration", Method.Post);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        request.AddHeader("X-Circle-App-Id", _options.AppId);
        request.AddHeader("X-Correlation-Id", correlationId);
        request.AddJsonBody(new { username });

        var response = await ExecuteWithRetryAsync<CircleRegistrationChallengeResponse>(
            request,
            correlationId,
            cancellationToken);

        return response;
    }

    /// <inheritdoc/>
    public async Task<CircleUserResponse> CompleteUserRegistrationAsync(
        CircleRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Completing user registration for username: {Username} [CorrelationId: {CorrelationId}]",
            request.Username,
            correlationId);

        var restRequest = new RestRequest("/users/register", Method.Post);
        restRequest.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        restRequest.AddHeader("X-Circle-App-Id", _options.AppId);
        restRequest.AddHeader("X-Correlation-Id", correlationId);
        restRequest.AddJsonBody(request);

        var response = await ExecuteWithRetryAsync<CircleUserResponse>(
            restRequest,
            correlationId,
            cancellationToken);

        _logger.LogInformation(
            "User registration completed successfully. CircleUserId: {CircleUserId} [CorrelationId: {CorrelationId}]",
            response.UserId,
            correlationId);

        return response;
    }

    /// <inheritdoc/>
    public async Task<CircleAuthenticationChallengeResponse> InitiateUserLoginAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Initiating user login for username: {Username} [CorrelationId: {CorrelationId}]",
            username,
            correlationId);

        var request = new RestRequest("/users/challenge/authentication", Method.Post);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        request.AddHeader("X-Circle-App-Id", _options.AppId);
        request.AddHeader("X-Correlation-Id", correlationId);
        request.AddJsonBody(new { username });

        var response = await ExecuteWithRetryAsync<CircleAuthenticationChallengeResponse>(
            request,
            correlationId,
            cancellationToken);

        return response;
    }

    /// <inheritdoc/>
    public async Task<CircleAuthenticationResponse> CompleteUserLoginAsync(
        CircleAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Completing user login for username: {Username} [CorrelationId: {CorrelationId}]",
            request.Username,
            correlationId);

        var restRequest = new RestRequest("/users/authenticate", Method.Post);
        restRequest.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        restRequest.AddHeader("X-Circle-App-Id", _options.AppId);
        restRequest.AddHeader("X-Correlation-Id", correlationId);
        restRequest.AddJsonBody(request);

        var response = await ExecuteWithRetryAsync<CircleAuthenticationResponse>(
            restRequest,
            correlationId,
            cancellationToken);

        _logger.LogInformation(
            "User login completed successfully. CircleUserId: {CircleUserId} [CorrelationId: {CorrelationId}]",
            response.UserId,
            correlationId);

        return response;
    }

    /// <inheritdoc/>
    public async Task<CircleUserResponse> GetUserAsync(
        string circleUserId,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Retrieving user details for CircleUserId: {CircleUserId} [CorrelationId: {CorrelationId}]",
            circleUserId,
            correlationId);

        var request = new RestRequest($"/users/{circleUserId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        request.AddHeader("X-Circle-App-Id", _options.AppId);
        request.AddHeader("X-Correlation-Id", correlationId);

        var response = await ExecuteWithRetryAsync<CircleUserResponse>(
            request,
            correlationId,
            cancellationToken);

        return response;
    }

    /// <inheritdoc/>
    public async Task<CircleWalletResponse> CreateWalletAsync(
        string circleUserId,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Creating wallet for CircleUserId: {CircleUserId} [CorrelationId: {CorrelationId}]",
            circleUserId,
            correlationId);

        var request = new RestRequest("/wallets", Method.Post);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        request.AddHeader("X-Circle-App-Id", _options.AppId);
        request.AddHeader("X-Correlation-Id", correlationId);
        request.AddJsonBody(new
        {
            userId = circleUserId,
            blockchain = "MATIC-AMOY", // Polygon Amoy testnet
            walletType = "SCA" // Smart Contract Account (ERC-4337)
        });

        var response = await ExecuteWithRetryAsync<CircleWalletResponse>(
            request,
            correlationId,
            cancellationToken);

        _logger.LogInformation(
            "Wallet created successfully. WalletId: {WalletId}, Address: {Address} [CorrelationId: {CorrelationId}]",
            response.WalletId,
            response.Address,
            correlationId);

        return response;
    }

    /// <inheritdoc/>
    public async Task<CircleWalletResponse> GetWalletAsync(
        string walletId,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Retrieving wallet details for WalletId: {WalletId} [CorrelationId: {CorrelationId}]",
            walletId,
            correlationId);

        var request = new RestRequest($"/wallets/{walletId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        request.AddHeader("X-Circle-App-Id", _options.AppId);
        request.AddHeader("X-Correlation-Id", correlationId);

        var response = await ExecuteWithRetryAsync<CircleWalletResponse>(
            request,
            correlationId,
            cancellationToken);

        return response;
    }

    /// <summary>
    /// Executes a REST request with retry policy and error handling.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response into</typeparam>
    /// <param name="request">The REST request to execute</param>
    /// <param name="correlationId">Correlation ID for tracking the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deserialized response</returns>
    /// <exception cref="HttpRequestException">Thrown when the Circle API returns an error</exception>
    private async Task<T> ExecuteWithRetryAsync<T>(
        RestRequest request,
        string correlationId,
        CancellationToken cancellationToken) where T : new()
    {
        var response = await _retryPolicy.ExecuteAsync(async () =>
            await _client.ExecuteAsync(request, cancellationToken));

        if (!response.IsSuccessful)
        {
            _logger.LogError(
                "Circle API request failed. StatusCode: {StatusCode}, Error: {Error}, Content: {Content} [CorrelationId: {CorrelationId}]",
                response.StatusCode,
                response.ErrorMessage,
                response.Content,
                correlationId);

            throw new HttpRequestException(
                $"Circle API request failed with status {response.StatusCode}: {response.ErrorMessage ?? response.Content}");
        }

        var data = System.Text.Json.JsonSerializer.Deserialize<T>(
            response.Content ?? "{}",
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return data ?? throw new InvalidOperationException("Failed to deserialize Circle API response");
    }
}

/// <summary>
/// Configuration options for Circle Web3 Services SDK.
/// </summary>
public class CircleOptions
{
    /// <summary>
    /// The base URL for Circle's Web3 Services API.
    /// Default: https://api.circle.com/v1/w3s
    /// </summary>
    public string ApiUrl { get; set; } = "https://api.circle.com/v1/w3s";

    /// <summary>
    /// The API key for authenticating with Circle's API.
    /// Obtain this from Circle's developer dashboard.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The entity secret for enhanced security operations.
    /// </summary>
    public string EntitySecret { get; set; } = string.Empty;

    /// <summary>
    /// The application ID registered with Circle.
    /// </summary>
    public string AppId { get; set; } = string.Empty;
}
