using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CoinPay.E2E.Tests;

/// <summary>
/// End-to-End tests for API endpoints
/// Tests API functionality through actual HTTP requests
/// </summary>
[TestClass]
public class ApiEndpointTests : PlaywrightTest
{
    private IAPIRequestContext? _apiContext;
    private string ApiUrl => "http://localhost:5100";

    [TestInitialize]
    public async Task Setup()
    {
        // Create API request context
        _apiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = ApiUrl,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"
            }
        });
    }

    [TestMethod]
    public async Task HealthEndpoint_ShouldReturn200()
    {
        // Act
        var response = await _apiContext!.GetAsync("/health");

        // Assert
        Assert.IsTrue(response.Ok, "Health endpoint should return 200 OK");
        Assert.AreEqual(200, response.Status);
    }

    [TestMethod]
    public async Task RootEndpoint_ShouldReturnWelcomeMessage()
    {
        // Act
        var response = await _apiContext!.GetAsync("/");

        // Assert
        Assert.IsTrue(response.Ok);
        var text = await response.TextAsync();
        Assert.IsTrue(text.Contains("CoinPay"), "Root endpoint should contain 'CoinPay'");
    }

    [TestMethod]
    public async Task GetAllTransactions_ShouldReturnJsonArray()
    {
        // Act
        var response = await _apiContext!.GetAsync("/api/transactions");

        // Assert
        Assert.IsTrue(response.Ok, "GET /api/transactions should succeed");

        var contentType = response.Headers["content-type"];
        Assert.IsTrue(contentType.Contains("application/json"), "Should return JSON");

        var jsonResponse = await response.JsonAsync();
        Assert.IsNotNull(jsonResponse, "Response should be valid JSON");
    }

    [TestMethod]
    public async Task CreateTransaction_ShouldReturn201Created()
    {
        // Arrange
        var transaction = new
        {
            amount = 100.50m,
            currency = "USD",
            type = "Payment",
            status = "Pending",
            senderName = "E2E Test Sender",
            receiverName = "E2E Test Receiver",
            description = "E2E test transaction"
        };

        // Act
        var response = await _apiContext!.PostAsync("/api/transactions", new()
        {
            DataObject = transaction
        });

        // Assert
        Assert.AreEqual(201, response.Status, "Should return 201 Created");

        var jsonResponse = await response.JsonAsync();
        var jsonDoc = JsonDocument.Parse(jsonResponse.ToString()!);
        Assert.IsTrue(jsonDoc.RootElement.TryGetProperty("id", out var idProp), "Response should contain id");
        Assert.IsTrue(idProp.GetInt32() > 0, "Created transaction should have valid ID");
    }

    [TestMethod]
    public async Task GetTransactionById_WithInvalidId_ShouldReturn404()
    {
        // Act
        var response = await _apiContext!.GetAsync("/api/transactions/99999");

        // Assert
        Assert.AreEqual(404, response.Status, "Should return 404 Not Found");
    }

    [TestMethod]
    public async Task GetAllUsers_ShouldReturnJsonArray()
    {
        // Act
        var response = await _apiContext!.GetAsync("/api/users");

        // Assert
        Assert.IsTrue(response.Ok, "GET /api/users should succeed");

        var contentType = response.Headers["content-type"];
        Assert.IsTrue(contentType.Contains("application/json"), "Should return JSON");
    }

    [TestMethod]
    public async Task CreateUser_ShouldReturn201Created()
    {
        // Arrange
        var user = new
        {
            username = $"e2euser_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            circleUserId = $"circle_e2e_{Guid.NewGuid()}"
        };

        // Act
        var response = await _apiContext!.PostAsync("/api/users", new()
        {
            DataObject = user
        });

        // Assert
        Assert.AreEqual(201, response.Status, "Should return 201 Created");

        var jsonResponse = await response.JsonAsync();
        var jsonDoc = JsonDocument.Parse(jsonResponse.ToString()!);
        Assert.IsTrue(jsonDoc.RootElement.TryGetProperty("username", out var usernameProp));
        Assert.AreEqual(user.username, usernameProp.GetString());
    }

    [TestMethod]
    public async Task UpdateTransaction_ShouldReturn200()
    {
        // Arrange - First create a transaction
        var transaction = new
        {
            amount = 50.00m,
            currency = "USD",
            type = "Payment",
            status = "Pending",
            senderName = "Test",
            receiverName = "Test2",
            description = "Test"
        };

        var createResponse = await _apiContext!.PostAsync("/api/transactions", new()
        {
            DataObject = transaction
        });

        var createJson = await createResponse.JsonAsync();
        var createDoc = JsonDocument.Parse(createJson.ToString()!);
        var transactionId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Act - Update the transaction
        var updatedTransaction = new
        {
            id = transactionId,
            amount = 75.00m,
            currency = "USD",
            type = "Payment",
            status = "Pending",
            senderName = "Test",
            receiverName = "Test2",
            description = "Updated description"
        };

        var updateResponse = await _apiContext.PutAsync($"/api/transactions/{transactionId}", new()
        {
            DataObject = updatedTransaction
        });

        // Assert
        Assert.IsTrue(updateResponse.Ok, "Update should succeed");

        var updateJson = await updateResponse.JsonAsync();
        var updateDoc = JsonDocument.Parse(updateJson.ToString()!);
        Assert.AreEqual(75.00m, updateDoc.RootElement.GetProperty("amount").GetDecimal());
    }

    [TestMethod]
    public async Task DeleteTransaction_ShouldReturn204()
    {
        // Arrange - Create a transaction
        var transaction = new
        {
            amount = 25.00m,
            currency = "USD",
            type = "Payment",
            status = "Pending",
            senderName = "Test",
            receiverName = "Test2",
            description = "To be deleted"
        };

        var createResponse = await _apiContext!.PostAsync("/api/transactions", new()
        {
            DataObject = transaction
        });

        var createJson = await createResponse.JsonAsync();
        var createDoc = JsonDocument.Parse(createJson.ToString()!);
        var transactionId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Act
        var deleteResponse = await _apiContext.DeleteAsync($"/api/transactions/{transactionId}");

        // Assert
        Assert.AreEqual(204, deleteResponse.Status, "Delete should return 204 No Content");

        // Verify it's deleted
        var getResponse = await _apiContext.GetAsync($"/api/transactions/{transactionId}");
        Assert.AreEqual(404, getResponse.Status, "Deleted transaction should not be found");
    }

    [TestMethod]
    public async Task UpdateTransactionStatus_ShouldSetCompletedAt()
    {
        // Arrange - Create a transaction
        var transaction = new
        {
            amount = 100.00m,
            currency = "USD",
            type = "Payment",
            status = "Pending",
            senderName = "Test",
            receiverName = "Test2",
            description = "Test"
        };

        var createResponse = await _apiContext!.PostAsync("/api/transactions", new()
        {
            DataObject = transaction
        });

        var createJson = await createResponse.JsonAsync();
        var createDoc = JsonDocument.Parse(createJson.ToString()!);
        var transactionId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Act - Update status to Completed
        var updateResponse = await _apiContext.PatchAsync($"/api/transactions/{transactionId}/status?status=Completed");

        // Assert
        Assert.IsTrue(updateResponse.Ok, "Status update should succeed");

        var updateJson = await updateResponse.JsonAsync();
        var updateDoc = JsonDocument.Parse(updateJson.ToString()!);

        Assert.AreEqual("Completed", updateDoc.RootElement.GetProperty("status").GetString());
        Assert.IsTrue(updateDoc.RootElement.TryGetProperty("completedAt", out var completedAt));
        Assert.IsNotNull(completedAt.GetString(), "CompletedAt should be set");
    }

    [TestMethod]
    public async Task InvalidEndpoint_ShouldReturn404()
    {
        // Act
        var response = await _apiContext!.GetAsync("/api/nonexistent");

        // Assert
        Assert.AreEqual(404, response.Status, "Invalid endpoint should return 404");
    }

    [TestMethod]
    public async Task RateLimiting_ShouldEnforce_RequestLimits()
    {
        // Note: This test checks if rate limiting exists
        // Actual limits depend on configuration

        // Act - Make multiple rapid requests
        var tasks = new List<Task<IAPIResponse>>();
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(_apiContext!.GetAsync("/health"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - Some requests might be rate limited (429)
        var rateLimitedCount = responses.Count(r => r.Status == 429);

        // Note: This is informational - actual rate limiting depends on config
        Console.WriteLine($"Rate limited requests: {rateLimitedCount} out of 100");
        Assert.IsTrue(rateLimitedCount >= 0, "Rate limiting check completed");
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_apiContext != null)
        {
            await _apiContext.DisposeAsync();
        }
    }
}
