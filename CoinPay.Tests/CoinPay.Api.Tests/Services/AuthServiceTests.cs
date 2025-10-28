using Xunit;
using Moq;
using FluentAssertions;
using CoinPay.Api.Services.Auth;
using CoinPay.Api.Services.Circle;
using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<ICircleService> _mockCircleService;
    private readonly Mock<IJwtTokenService> _mockJwtService;
    private readonly AppDbContext _dbContext;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockCircleService = new Mock<ICircleService>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        _authService = new AuthService(
            _dbContext,
            _mockCircleService.Object,
            _mockJwtTokenService.Object
        );
    }

    [Fact]
    public async Task CheckUsernameAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        var username = "existinguser";
        _dbContext.Users.Add(new User
        {
            Username = username,
            CircleUserId = "circle123",
            WalletAddress = "0x123",
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _authService.CheckUsernameAsync(username);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckUsernameAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        // Arrange
        var username = "newuser";

        // Act
        var result = await _authService.CheckUsernameAsync(username);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InitiateRegistrationAsync_ShouldReturnChallenge_ForNewUser()
    {
        // Arrange
        var username = "newuser";
        var expectedChallenge = "test-challenge-123";

        _mockCircleService
            .Setup(x => x.InitiateUserRegistrationAsync(username))
            .ReturnsAsync(("challenge-id", expectedChallenge, "circle-user-id"));

        // Act
        var result = await _authService.InitiateRegistrationAsync(username);

        // Assert
        result.Should().NotBeNull();
        result.challenge.Should().Be(expectedChallenge);
        _mockCircleService.Verify(x => x.InitiateUserRegistrationAsync(username), Times.Once);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldCreateUser_WhenValidCredentials()
    {
        // Arrange
        var username = "newuser";
        var challengeId = "challenge123";
        var credentialId = "cred123";
        var publicKey = "pubkey123";
        var authenticatorData = "authdata123";
        var circleUserId = "circle-user-id";
        var walletAddress = "0x123456";

        _mockCircleService
            .Setup(x => x.CompleteUserRegistrationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((circleUserId, walletAddress));

        // Act
        var result = await _authService.CompleteRegistrationAsync(
            username,
            challengeId,
            credentialId,
            publicKey,
            authenticatorData
        );

        // Assert
        result.Should().NotBeNull();
        result.username.Should().Be(username);
        result.walletAddress.Should().Be(walletAddress);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        user.Should().NotBeNull();
        user!.CircleUserId.Should().Be(circleUserId);
        user.WalletAddress.Should().Be(walletAddress);
    }

    [Fact]
    public async Task InitiateLoginAsync_ShouldReturnChallenge_ForExistingUser()
    {
        // Arrange
        var username = "existinguser";
        var circleUserId = "circle123";
        var expectedChallenge = "login-challenge-123";

        _dbContext.Users.Add(new User
        {
            Username = username,
            CircleUserId = circleUserId,
            WalletAddress = "0x123",
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        _mockCircleService
            .Setup(x => x.InitiateUserLoginAsync(circleUserId))
            .ReturnsAsync(("challenge-id", expectedChallenge));

        // Act
        var result = await _authService.InitiateLoginAsync(username);

        // Assert
        result.Should().NotBeNull();
        result.challenge.Should().Be(expectedChallenge);
    }

    [Fact]
    public async Task CompleteLoginAsync_ShouldReturnToken_WhenValidCredentials()
    {
        // Arrange
        var username = "existinguser";
        var circleUserId = "circle123";
        var expectedToken = "jwt-token-123";

        _dbContext.Users.Add(new User
        {
            Username = username,
            CircleUserId = circleUserId,
            WalletAddress = "0x123",
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        _mockCircleService
            .Setup(x => x.CompleteUserLoginAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(username))
            .Returns(expectedToken);

        // Act
        var result = await _authService.CompleteLoginAsync(
            username,
            "challenge-id",
            "cred-id",
            "auth-data",
            "signature"
        );

        // Assert
        result.Should().NotBeNull();
        result.token.Should().Be(expectedToken);
        result.username.Should().Be(username);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
