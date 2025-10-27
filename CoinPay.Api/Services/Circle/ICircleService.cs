using CoinPay.Api.Services.Circle.Models;

namespace CoinPay.Api.Services.Circle;

/// <summary>
/// Interface for Circle Web3 Services SDK client operations.
/// Provides methods for user management, wallet operations, and passkey authentication.
/// </summary>
public interface ICircleService
{
    /// <summary>
    /// Initializes a registration challenge for WebAuthn passkey creation.
    /// </summary>
    /// <param name="username">The username for the new user</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>A challenge response containing the WebAuthn challenge data</returns>
    Task<CircleRegistrationChallengeResponse> InitiateUserRegistrationAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes user registration by verifying the passkey credential and creating a Circle user.
    /// </summary>
    /// <param name="request">The registration completion request containing the signed challenge</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>The created user details including Circle user ID and wallet information</returns>
    Task<CircleUserResponse> CompleteUserRegistrationAsync(CircleRegistrationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a login challenge for WebAuthn passkey authentication.
    /// </summary>
    /// <param name="username">The username attempting to login</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>A challenge response containing the WebAuthn authentication challenge</returns>
    Task<CircleAuthenticationChallengeResponse> InitiateUserLoginAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes user login by verifying the passkey authentication response.
    /// </summary>
    /// <param name="request">The authentication completion request containing the signed challenge</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>The authenticated user details and session token</returns>
    Task<CircleAuthenticationResponse> CompleteUserLoginAsync(CircleAuthenticationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user details from Circle by user ID.
    /// </summary>
    /// <param name="circleUserId">The Circle user ID</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>User details including wallet information</returns>
    Task<CircleUserResponse> GetUserAsync(string circleUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a smart account wallet for a user.
    /// </summary>
    /// <param name="circleUserId">The Circle user ID</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>The created wallet details including wallet address</returns>
    Task<CircleWalletResponse> CreateWalletAsync(string circleUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves wallet details for a specific wallet ID.
    /// </summary>
    /// <param name="walletId">The wallet ID</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Wallet details including address and balance</returns>
    Task<CircleWalletResponse> GetWalletAsync(string walletId, CancellationToken cancellationToken = default);
}
