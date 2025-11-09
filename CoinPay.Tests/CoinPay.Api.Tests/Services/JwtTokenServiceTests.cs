using Xunit;
using Moq;
using FluentAssertions;
using CoinPay.Api.Services.Auth;
using CoinPay.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CoinPay.Api.Tests.Services;

public class JwtTokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<JwtTokenService>> _mockLogger;
    private readonly JwtTokenService _jwtTokenService;
    private readonly string _testSecretKey = "this-is-a-very-long-secret-key-for-testing-purposes-minimum-32-characters";

    public JwtTokenServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<JwtTokenService>>();

        // Setup configuration
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("CoinPayTestIssuer");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("CoinPayTestAudience");
        _mockConfiguration.Setup(x => x["Jwt:SecretKey"]).Returns(_testSecretKey);
        _mockConfiguration.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("1440");

        _jwtTokenService = new JwtTokenService(_mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            WalletAddress = "0x1234567890abcdef",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts: header.payload.signature
    }

    [Fact]
    public void GenerateToken_ShouldIncludeUserIdInClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 42,
            Username = "testuser",
            CircleUserId = "circle-123",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        subClaim.Should().NotBeNull();
        subClaim!.Value.Should().Be("42");
    }

    [Fact]
    public void GenerateToken_ShouldIncludeUsernameInClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "johndoe",
            CircleUserId = "circle-123",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var uniqueNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
        uniqueNameClaim.Should().NotBeNull();
        uniqueNameClaim!.Value.Should().Be("johndoe");
    }

    [Fact]
    public void GenerateToken_ShouldIncludeCircleUserIdInClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-abc-123",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var circleUserIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "circleUserId");
        circleUserIdClaim.Should().NotBeNull();
        circleUserIdClaim!.Value.Should().Be("circle-abc-123");
    }

    [Fact]
    public void GenerateToken_ShouldIncludeWalletAddressInClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            WalletAddress = "0xABCDEF1234567890",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var walletAddressClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "walletAddress");
        walletAddressClaim.Should().NotBeNull();
        walletAddressClaim!.Value.Should().Be("0xABCDEF1234567890");
    }

    [Fact]
    public void GenerateToken_ShouldSetCorrectIssuerAndAudience()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        jwtToken.Issuer.Should().Be("CoinPayTestIssuer");
        jwtToken.Audiences.Should().Contain("CoinPayTestAudience");
    }

    [Fact]
    public void GenerateToken_ShouldSetExpirationTime()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            CreatedAt = DateTime.UtcNow
        };
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        jwtToken.ValidTo.Should().BeAfter(beforeGeneration.AddMinutes(1430)); // ~24 hours minus buffer
        jwtToken.ValidTo.Should().BeBefore(beforeGeneration.AddMinutes(1450)); // ~24 hours plus buffer
    }

    [Fact]
    public void ValidateToken_ShouldReturnPrincipal_ForValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            CreatedAt = DateTime.UtcNow
        };
        var token = _jwtTokenService.GenerateToken(user);

        // Act
        var principal = _jwtTokenService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Claims.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidateToken_ShouldReturnNull_ForInvalidToken()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var principal = _jwtTokenService.ValidateToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_ShouldReturnNull_ForExpiredToken()
    {
        // Arrange - Create a token with 0 minute expiration
        var tempConfig = new Mock<IConfiguration>();
        tempConfig.Setup(x => x["Jwt:Issuer"]).Returns("CoinPayTestIssuer");
        tempConfig.Setup(x => x["Jwt:Audience"]).Returns("CoinPayTestAudience");
        tempConfig.Setup(x => x["Jwt:SecretKey"]).Returns(_testSecretKey);
        tempConfig.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("-1"); // Expired immediately

        var tempService = new JwtTokenService(tempConfig.Object, _mockLogger.Object);
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            CreatedAt = DateTime.UtcNow
        };
        var expiredToken = tempService.GenerateToken(user);

        // Act
        var principal = _jwtTokenService.ValidateToken(expiredToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_ShouldReturnNull_ForTokenWithWrongSignature()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            CreatedAt = DateTime.UtcNow
        };
        var token = _jwtTokenService.GenerateToken(user);

        // Tamper with the token by changing the signature
        var parts = token.Split('.');
        var tamperedToken = $"{parts[0]}.{parts[1]}.invalid_signature";

        // Act
        var principal = _jwtTokenService.ValidateToken(tamperedToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_ShouldIncludeAllClaimsInPrincipal()
    {
        // Arrange
        var user = new User
        {
            Id = 123,
            Username = "testuser",
            CircleUserId = "circle-xyz",
            WalletAddress = "0x123ABC",
            CreatedAt = DateTime.UtcNow
        };
        var token = _jwtTokenService.GenerateToken(user);

        // Act
        var principal = _jwtTokenService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        var claims = principal!.Claims.ToList();

        // Note: JWT claims are mapped to longer claim types in ClaimsPrincipal
        claims.Should().Contain(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == "123");
        claims.Should().Contain(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" && c.Value == "testuser");
        claims.Should().Contain(c => c.Type == "circleUserId" && c.Value == "circle-xyz");
        claims.Should().Contain(c => c.Type == "walletAddress" && c.Value == "0x123ABC");
    }

    [Fact]
    public void GenerateToken_ShouldHandleNullWalletAddress()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = "circle-123",
            WalletAddress = null,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var walletAddressClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "walletAddress");
        walletAddressClaim.Should().NotBeNull();
        walletAddressClaim!.Value.Should().BeEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldHandleNullCircleUserId()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            CircleUserId = null,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var circleUserIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "circleUserId");
        circleUserIdClaim.Should().NotBeNull();
        circleUserIdClaim!.Value.Should().BeEmpty();
    }
}
