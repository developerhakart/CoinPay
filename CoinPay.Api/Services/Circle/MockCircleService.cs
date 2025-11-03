using CoinPay.Api.Services.Circle.Models;

namespace CoinPay.Api.Services.Circle;

/// <summary>
/// Mock implementation of Circle service for MVP/development testing.
/// Returns simulated responses without making actual API calls.
/// </summary>
public class MockCircleService : ICircleService
{
    private readonly ILogger<MockCircleService> _logger;
    private readonly Dictionary<string, string> _mockUsers = new();

    public MockCircleService(
        ILogger<MockCircleService> logger,
        IEntitySecretEncryptionService? encryptionService = null)
    {
        _logger = logger;
        // Mock service doesn't need encryption service - it's optional
    }

    public Task<CircleRegistrationChallengeResponse> InitiateUserRegistrationAsync(string username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Initiating registration for user: {Username}", username);

        var challenge = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return Task.FromResult(new CircleRegistrationChallengeResponse
        {
            Challenge = challenge,
            RpId = "localhost",
            UserId = $"mock_user_{Guid.NewGuid()}",
            Timeout = 60000
        });
    }

    public Task<CircleUserResponse> CompleteUserRegistrationAsync(CircleRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Completing registration for user: {Username}", request.Username);

        var circleUserId = $"mock_user_{Guid.NewGuid()}";
        _mockUsers[request.Username] = circleUserId;

        return Task.FromResult(new CircleUserResponse
        {
            UserId = circleUserId,
            Username = request.Username,
            CredentialId = $"mock_cred_{Guid.NewGuid()}",
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<CircleAuthenticationChallengeResponse> InitiateUserLoginAsync(string username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Initiating login for user: {Username}", username);

        // For MVP: Accept any username for login (validation happens in AuthService against database)
        var challenge = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return Task.FromResult(new CircleAuthenticationChallengeResponse
        {
            Challenge = challenge,
            RpId = "localhost",
            Timeout = 60000,
            AllowedCredentials = new List<string> { "mock_cred_" + username }
        });
    }

    public Task<CircleAuthenticationResponse> CompleteUserLoginAsync(CircleAuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Completing login for user: {Username}", request.Username);

        // For MVP: Generate mock circle user ID (actual validation happens in AuthService)
        var circleUserId = _mockUsers.TryGetValue(request.Username, out var existingId)
            ? existingId
            : $"mock_user_{Guid.NewGuid()}";

        return Task.FromResult(new CircleAuthenticationResponse
        {
            UserId = circleUserId,
            Username = request.Username,
            SessionToken = $"mock_session_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
    }

    public Task<CircleUserResponse> GetUserAsync(string circleUserId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Getting user: {CircleUserId}", circleUserId);

        return Task.FromResult(new CircleUserResponse
        {
            UserId = circleUserId,
            Username = "mock_user",
            CredentialId = "mock_cred_123",
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        });
    }

    public Task<CircleWalletResponse> CreateWalletAsync(string circleUserId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Creating wallet for Circle User ID: {CircleUserId}", circleUserId);

        var walletId = $"mock_wallet_{Guid.NewGuid()}";
        var walletAddress = $"0x{Guid.NewGuid():N}".Substring(0, 42); // Mock Ethereum address format

        return Task.FromResult(new CircleWalletResponse
        {
            WalletId = walletId,
            Address = walletAddress,
            Blockchain = "MATIC-AMOY",
            WalletType = "SCA",
            UserId = circleUserId,
            CreatedAt = DateTime.UtcNow,
            Balance = 0,
            BalanceCurrency = "USDC"
        });
    }

    public Task<CircleWalletResponse> GetWalletAsync(string walletId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Getting wallet: {WalletId}", walletId);

        return Task.FromResult(new CircleWalletResponse
        {
            WalletId = walletId,
            Address = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            Blockchain = "MATIC-AMOY",
            WalletType = "SCA",
            UserId = "mock_user_123",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Balance = 100.00m,
            BalanceCurrency = "USDC"
        });
    }

    public Task<CircleTransactionChallengeResponse> InitiateTransactionAsync(
        CircleTransactionChallengeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Initiating transaction to {ToAddress}", request.DestinationAddress);

        var challenge = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return Task.FromResult(new CircleTransactionChallengeResponse
        {
            ChallengeId = $"mock_challenge_{Guid.NewGuid()}",
            Challenge = challenge,
            RpId = "localhost",
            UserVerification = "required"
        });
    }

    public Task<CircleTransactionResponse> ExecuteTransactionAsync(
        CircleTransactionExecuteRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Executing transaction for challenge {ChallengeId}", request.ChallengeId);

        return Task.FromResult(new CircleTransactionResponse
        {
            TransactionId = $"mock_tx_{Guid.NewGuid()}",
            TxHash = $"0x{Guid.NewGuid():N}",
            Status = "PENDING",
            Blockchain = "MATIC-AMOY",
            From = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            To = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            Amount = "0",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public Task<CircleTransactionResponse> GetTransactionStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockCircle] Getting transaction status for {TransactionId}", transactionId);

        return Task.FromResult(new CircleTransactionResponse
        {
            TransactionId = transactionId,
            TxHash = $"0x{Guid.NewGuid():N}",
            Status = "CONFIRMED",
            Blockchain = "MATIC-AMOY",
            From = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            To = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            Amount = "0",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            UpdatedAt = DateTime.UtcNow
        });
    }

    public Task<CircleTransactionResponse> ExecuteDeveloperTransferAsync(
        CircleDeveloperTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MockCircle] Executing developer transfer from wallet {WalletId} to {Destination}, Amount: {Amount}",
            request.WalletId,
            request.DestinationAddress,
            string.Join(",", request.Amounts));

        return Task.FromResult(new CircleTransactionResponse
        {
            TransactionId = Guid.NewGuid().ToString(),
            TxHash = $"0x{Guid.NewGuid():N}",
            Status = "PENDING",
            Blockchain = request.Blockchain,
            From = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            To = request.DestinationAddress,
            Amount = request.Amounts.FirstOrDefault() ?? "0",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }
}
