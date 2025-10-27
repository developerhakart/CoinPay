namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Request to complete user registration with Circle after passkey creation.
/// </summary>
public class CircleRegistrationRequest
{
    /// <summary>
    /// The username for the registration.
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
    /// The base64-encoded public key from WebAuthn.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// The attestation object from WebAuthn.
    /// </summary>
    public string AttestationObject { get; set; } = string.Empty;

    /// <summary>
    /// The client data JSON from WebAuthn.
    /// </summary>
    public string ClientDataJson { get; set; } = string.Empty;
}
