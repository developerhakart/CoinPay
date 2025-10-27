namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Request to complete user authentication with Circle after passkey verification.
/// </summary>
public class CircleAuthenticationRequest
{
    /// <summary>
    /// The username attempting to authenticate.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The challenge ID from the initiation step.
    /// </summary>
    public string ChallengeId { get; set; } = string.Empty;

    /// <summary>
    /// The base64-encoded credential ID from WebAuthn.
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// The base64-encoded signature from WebAuthn.
    /// </summary>
    public string Signature { get; set; } = string.Empty;

    /// <summary>
    /// The authenticator data from WebAuthn.
    /// </summary>
    public string AuthenticatorData { get; set; } = string.Empty;

    /// <summary>
    /// The client data JSON from WebAuthn.
    /// </summary>
    public string ClientDataJson { get; set; } = string.Empty;
}
