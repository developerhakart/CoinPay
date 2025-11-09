using Xunit;
using Moq;
using FluentAssertions;
using CoinPay.Api.Services.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Tests.Services;

public class AesEncryptionServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<AesEncryptionService>> _mockLogger;
    private readonly AesEncryptionService _encryptionService;

    public AesEncryptionServiceTests()
    {
        _mockLogger = new Mock<ILogger<AesEncryptionService>>();

        // Create base64 encoded key (32 bytes for AES-256)
        var keyBytes = new byte[32];
        for (int i = 0; i < 32; i++)
        {
            keyBytes[i] = (byte)(i + 65); // Fill with A-Z pattern
        }
        var keyBase64 = Convert.ToBase64String(keyBytes);

        var configDict = new Dictionary<string, string>
        {
            {"Encryption:Key", keyBase64}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        _encryptionService = new AesEncryptionService(_configuration, _mockLogger.Object);
    }

    [Fact]
    public void Encrypt_ShouldReturnEncryptedBytes()
    {
        // Arrange
        var plaintext = "Hello, World!";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);

        // Assert
        encrypted.Should().NotBeNull();
        encrypted.Should().NotBeEmpty();
        encrypted.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Decrypt_ShouldReturnOriginalPlaintext()
    {
        // Arrange
        var plaintext = "Secret Message";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("Short text")]
    [InlineData("This is a much longer text that includes multiple words and sentences. It should still encrypt and decrypt correctly.")]
    public void EncryptDecrypt_ShouldWorkForVariousLengths(string plaintext)
    {
        // Skip empty string test as it throws ArgumentNullException
        if (string.IsNullOrEmpty(plaintext))
        {
            var action = () => _encryptionService.Encrypt(plaintext);
            action.Should().Throw<ArgumentNullException>();
            return;
        }

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldProduceDifferentCiphertextForSameInput()
    {
        // Arrange
        var plaintext = "Same input";

        // Act
        var encrypted1 = _encryptionService.Encrypt(plaintext);
        var encrypted2 = _encryptionService.Encrypt(plaintext);

        // Assert
        // Should be different due to random nonce (IV)
        encrypted1.Should().NotBeEquivalentTo(encrypted2);
    }

    [Fact]
    public void Decrypt_ShouldReturnSamePlaintext_ForDifferentCiphertext()
    {
        // Arrange
        var plaintext = "Same input";

        // Act
        var encrypted1 = _encryptionService.Encrypt(plaintext);
        var encrypted2 = _encryptionService.Encrypt(plaintext);
        var decrypted1 = _encryptionService.Decrypt(encrypted1);
        var decrypted2 = _encryptionService.Decrypt(encrypted2);

        // Assert
        decrypted1.Should().Be(plaintext);
        decrypted2.Should().Be(plaintext);
        decrypted1.Should().Be(decrypted2);
    }

    [Fact]
    public void Encrypt_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var plaintext = "Special: !@#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldHandleUnicodeCharacters()
    {
        // Arrange
        var plaintext = "Unicode: 你好世界 🌍 مرحبا العالم";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldHandleNumericStrings()
    {
        // Arrange
        var plaintext = "1234567890";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldHandleJsonStrings()
    {
        // Arrange
        var plaintext = "{\"key\":\"value\",\"number\":123,\"array\":[1,2,3]}";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Decrypt_ShouldThrowException_ForInvalidCiphertext()
    {
        // Arrange
        var invalidCiphertext = new byte[] { 1, 2, 3 }; // Too short

        // Act & Assert
        var action = () => _encryptionService.Decrypt(invalidCiphertext);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Decrypt_ShouldThrowException_ForTamperedCiphertext()
    {
        // Arrange
        var plaintext = "Original message";
        var encrypted = _encryptionService.Encrypt(plaintext);

        // Tamper with the encrypted bytes
        var tamperedEncrypted = new byte[encrypted.Length];
        Array.Copy(encrypted, tamperedEncrypted, encrypted.Length);
        tamperedEncrypted[tamperedEncrypted.Length - 1] ^= 0xFF; // Flip last byte

        // Act & Assert
        var action = () => _encryptionService.Decrypt(tamperedEncrypted);
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EncryptDecrypt_ShouldBeIdempotent()
    {
        // Arrange
        var plaintext = "Test idempotence";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted1 = _encryptionService.Decrypt(encrypted);
        var reEncrypted = _encryptionService.Encrypt(decrypted1);
        var decrypted2 = _encryptionService.Decrypt(reEncrypted);

        // Assert
        decrypted1.Should().Be(plaintext);
        decrypted2.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldThrowException_ForEmptyString()
    {
        // Arrange
        var plaintext = "";

        // Act & Assert
        var action = () => _encryptionService.Encrypt(plaintext);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Encrypt_ShouldThrowException_ForNullString()
    {
        // Arrange
        string plaintext = null!;

        // Act & Assert
        var action = () => _encryptionService.Encrypt(plaintext);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Encrypt_ShouldHandleWhitespaceString()
    {
        // Arrange
        var plaintext = "   ";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldHandleNewlineCharacters()
    {
        // Arrange
        var plaintext = "Line 1\nLine 2\r\nLine 3";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_ShouldProduceReasonableCiphertextLength()
    {
        // Arrange
        var plaintext = "Test";

        // Act
        var encrypted = _encryptionService.Encrypt(plaintext);

        // Assert
        // AES-GCM should produce ciphertext = nonce (12) + ciphertext (4 for "Test") + tag (16) = 32 bytes
        encrypted.Length.Should().Be(12 + 4 + 16);
    }

    [Fact]
    public void VerifyEncryption_ShouldReturnTrue_ForValidSetup()
    {
        // Act
        var result = _encryptionService.VerifyEncryption();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Decrypt_ShouldThrowException_ForNullCiphertext()
    {
        // Arrange
        byte[] ciphertext = null!;

        // Act & Assert
        var action = () => _encryptionService.Decrypt(ciphertext);
        action.Should().Throw<ArgumentException>();
    }
}
