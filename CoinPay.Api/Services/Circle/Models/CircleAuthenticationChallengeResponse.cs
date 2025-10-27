namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Response containing the WebAuthn authentication challenge data from Circle.
/// </summary>
public class CircleAuthenticationChallengeResponse
{
    /// <summary>
    /// The base64-encoded challenge string for WebAuthn authentication.
    /// </summary>
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// The relying party identifier for WebAuthn.
    /// </summary>
    public string RpId { get; set; } = string.Empty;

    /// <summary>
    /// The timeout for the challenge in milliseconds.
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// The list of allowed credentials for this authentication.
    /// </summary>
    public List<string> AllowedCredentials { get; set; } = new();
}
