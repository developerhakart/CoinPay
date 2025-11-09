using Xunit;
using Moq;
using FluentAssertions;
using CoinPay.Api.Services.BankAccount;
using CoinPay.Api.Services.Encryption;
using CoinPay.Api.Repositories;

namespace CoinPay.Api.Tests.Services;

public class BankAccountValidationServiceTests
{
    private readonly Mock<IBankAccountRepository> _mockBankAccountRepository;
    private readonly Mock<IEncryptionService> _mockEncryptionService;
    private readonly BankAccountValidationService _validationService;

    public BankAccountValidationServiceTests()
    {
        _mockBankAccountRepository = new Mock<IBankAccountRepository>();
        _mockEncryptionService = new Mock<IEncryptionService>();
        _validationService = new BankAccountValidationService(
            _mockBankAccountRepository.Object,
            _mockEncryptionService.Object);
    }

    [Theory]
    [InlineData("011401533", true)] // Wells Fargo
    [InlineData("021000021", true)] // JPMorgan Chase
    [InlineData("026009593", true)] // Bank of America
    [InlineData("121000248", true)] // Wells Fargo
    [InlineData("12345678", false)] // Invalid checksum
    [InlineData("123456", false)] // Too short
    [InlineData("1234567890", false)] // Too long
    [InlineData("", false)] // Empty
    [InlineData("ABCD1234", false)] // Contains letters
    public void ValidateRoutingNumber_ShouldReturnExpectedResult(string routingNumber, bool expectedValid)
    {
        // Act
        var result = _validationService.ValidateRoutingNumber(routingNumber);

        // Assert
        result.IsValid.Should().Be(expectedValid);
        if (!expectedValid)
        {
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }

    [Theory]
    [InlineData("1234567890", true)]
    [InlineData("123456789", true)]
    [InlineData("12345678901234567", true)]
    [InlineData("1234", false)] // Too short
    [InlineData("123456789012345678", false)] // Too long
    [InlineData("", false)] // Empty
    [InlineData("ABCD123456", true)] // Letters are stripped, leaving "123456" which is valid
    public void ValidateAccountNumber_ShouldReturnExpectedResult(string accountNumber, bool expectedValid)
    {
        // Act
        var result = _validationService.ValidateAccountNumber(accountNumber);

        // Assert
        result.IsValid.Should().Be(expectedValid);
        if (!expectedValid)
        {
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }

    [Theory]
    [InlineData("John Doe", true)]
    [InlineData("Mary Jane Smith", true)]
    [InlineData("José García", false)] // Accented characters not supported by regex
    [InlineData("O'Connor", true)]
    [InlineData("Jean-Pierre", true)]
    [InlineData("A", false)] // Too short
    [InlineData("", false)] // Empty
    [InlineData("123456", false)] // Numbers only
    public void ValidateAccountHolderName_ShouldReturnExpectedResult(string name, bool expectedValid)
    {
        // Act
        var result = _validationService.ValidateAccountHolderName(name);

        // Assert
        result.IsValid.Should().Be(expectedValid);
        if (!expectedValid)
        {
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void ValidateRoutingNumber_ShouldThrowException_ForNullInput()
    {
        // Act & Assert
        var action = () => _validationService.ValidateRoutingNumber(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValidateAccountNumber_ShouldThrowException_ForNullInput()
    {
        // Act & Assert
        var action = () => _validationService.ValidateAccountNumber(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValidateAccountHolderName_ShouldThrowException_ForNullInput()
    {
        // Act & Assert
        var action = () => _validationService.ValidateAccountHolderName(null!);
        action.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void ValidateRoutingNumber_ShouldRemoveNonDigitCharacters()
    {
        // Arrange - Valid routing number with hyphens
        var routingNumber = "011-401-533";

        // Act
        var result = _validationService.ValidateRoutingNumber(routingNumber);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateAccountNumber_ShouldRemoveNonDigitCharacters()
    {
        // Arrange - Account number with spaces
        var accountNumber = "1234 5678 90";

        // Act
        var result = _validationService.ValidateAccountNumber(accountNumber);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateAccountHolderName_ShouldTrimWhitespace()
    {
        // Arrange
        var name = "  John Doe  ";

        // Act
        var result = _validationService.ValidateAccountHolderName(name);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateRoutingNumber_ShouldProvideHelpfulErrorMessages()
    {
        // Arrange
        var tooShort = "12345";

        // Act
        var result = _validationService.ValidateRoutingNumber(tooShort);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("9 digits");
    }

    [Fact]
    public void ValidateAccountNumber_ShouldProvideHelpfulErrorMessages()
    {
        // Arrange
        var tooShort = "123";

        // Act
        var result = _validationService.ValidateAccountNumber(tooShort);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("5");
    }
}
