namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Response containing the WebAuthn registration challenge data from Circle.
/// </summary>
public class CircleRegistrationChallengeResponse
{
    /// <summary>
    /// The base64-encoded challenge string for WebAuthn registration.
    /// </summary>
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// The relying party identifier for WebAuthn.
    /// </summary>
    public string RpId { get; set; } = string.Empty;

    /// <summary>
    /// The user identifier for the registration.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The timeout for the challenge in milliseconds.
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// Additional options for the WebAuthn registration.
    /// </summary>
    public Dictionary<string, object>? Options { get; set; }
}
