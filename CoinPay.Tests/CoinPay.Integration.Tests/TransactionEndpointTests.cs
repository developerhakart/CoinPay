using System.Net;
using System.Net.Http.Json;
using CoinPay.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using CoinPay.Api.Data;

namespace CoinPay.Integration.Tests;

/// <summary>
/// Integration tests for Transaction API endpoints
/// Tests CRUD operations and business logic
/// </summary>
public class TransactionEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public TransactionEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTransactions_ShouldReturnTransactions()
    {
        // Act
        var response = await _client.GetAsync("/api/transactions");

        // Assert
        response.EnsureSuccessStatusCode();
        var transactions = await response.Content.ReadFromJsonAsync<List<Transaction>>();

        Assert.NotNull(transactions);
        Assert.True(transactions.Count >= 0);
    }

    [Fact]
    public async Task GetTransactionById_WithValidId_ShouldReturnTransaction()
    {
        // Arrange - Create a transaction first
        var newTransaction = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test Sender",
            ReceiverName = "Test Receiver",
            Description = "Test transaction"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transactions", newTransaction);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        // Act
        var response = await _client.GetAsync($"/api/transactions/{created!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transaction = await response.Content.ReadFromJsonAsync<Transaction>();

        Assert.NotNull(transaction);
        Assert.Equal(created.Id, transaction.Id);
        Assert.Equal(100.00m, transaction.Amount);
    }

    [Fact]
    public async Task GetTransactionById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/transactions/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateTransaction_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newTransaction = new Transaction
        {
            Amount = 250.50m,
            Currency = "USD",
            Type = "Transfer",
            Status = "Pending",
            SenderName = "Alice",
            ReceiverName = "Bob",
            Description = "Test transfer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transactions", newTransaction);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var transaction = await response.Content.ReadFromJsonAsync<Transaction>();

        Assert.NotNull(transaction);
        Assert.True(transaction.Id > 0);
        Assert.Equal(250.50m, transaction.Amount);
        Assert.Equal("Alice", transaction.SenderName);
        Assert.NotNull(transaction.TransactionId);
        Assert.NotNull(transaction.CreatedAt);
    }

    [Fact]
    public async Task CreateTransaction_ShouldAutoGenerateTransactionId()
    {
        // Arrange
        var newTransaction = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transactions", newTransaction);

        // Assert
        response.EnsureSuccessStatusCode();
        var transaction = await response.Content.ReadFromJsonAsync<Transaction>();

        Assert.NotNull(transaction?.TransactionId);
        Assert.StartsWith("TXN", transaction.TransactionId);
    }

    [Fact]
    public async Task UpdateTransaction_WithValidData_ShouldReturnOk()
    {
        // Arrange - Create a transaction
        var newTransaction = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Original Sender",
            ReceiverName = "Original Receiver",
            Description = "Original description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transactions", newTransaction);
        var created = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        // Act - Update the transaction
        created!.Amount = 200.00m;
        created.Description = "Updated description";

        var updateResponse = await _client.PutAsJsonAsync($"/api/transactions/{created.Id}", created);

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<Transaction>();

        Assert.Equal(200.00m, updated!.Amount);
        Assert.Equal("Updated description", updated.Description);
    }

    [Fact]
    public async Task UpdateTransaction_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var transaction = new Transaction
        {
            Id = 99999,
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/transactions/99999", transaction);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTransaction_WithValidId_ShouldReturnNoContent()
    {
        // Arrange - Create a transaction
        var newTransaction = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transactions", newTransaction);
        var created = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/transactions/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/transactions/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTransaction_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/transactions/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTransactionStatus_WithValidData_ShouldUpdateStatus()
    {
        // Arrange - Create a transaction
        var newTransaction = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transactions", newTransaction);
        var created = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        // Act - Update status to Completed
        var response = await _client.PatchAsync($"/api/transactions/{created!.Id}/status?status=Completed", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<Transaction>();

        Assert.Equal("Completed", updated!.Status);
        Assert.NotNull(updated.CompletedAt);
    }

    [Fact]
    public async Task UpdateTransactionStatus_ShouldSetCompletedAt_WhenStatusIsCompleted()
    {
        // Arrange
        var newTransaction = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transactions", newTransaction);
        var created = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        // Act
        var response = await _client.PatchAsync($"/api/transactions/{created!.Id}/status?status=Completed", null);
        var updated = await response.Content.ReadFromJsonAsync<Transaction>();

        // Assert
        Assert.NotNull(updated!.CompletedAt);
        Assert.True(updated.CompletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task GetTransactionsByStatus_ShouldReturnFilteredTransactions()
    {
        // Arrange - Create transactions with different statuses
        var pendingTx = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test"
        };

        var completedTx = new Transaction
        {
            Amount = 200.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Completed",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test",
            CompletedAt = DateTime.UtcNow
        };

        await _client.PostAsJsonAsync("/api/transactions", pendingTx);
        await _client.PostAsJsonAsync("/api/transactions", completedTx);

        // Act
        var response = await _client.GetAsync("/api/transactions/status/Pending");

        // Assert
        response.EnsureSuccessStatusCode();
        var transactions = await response.Content.ReadFromJsonAsync<List<Transaction>>();

        Assert.NotNull(transactions);
        Assert.All(transactions, t => Assert.Equal("Pending", t.Status));
    }

    [Fact]
    public async Task CreateTransaction_ShouldSetCreatedAtToUtc()
    {
        // Arrange
        var newTransaction = new Transaction
        {
            Amount = 100.00m,
            Currency = "USD",
            Type = "Payment",
            Status = "Pending",
            SenderName = "Test",
            ReceiverName = "Test2",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transactions", newTransaction);
        var created = await response.Content.ReadFromJsonAsync<Transaction>();

        // Assert
        Assert.NotNull(created!.CreatedAt);
        Assert.Equal(DateTimeKind.Utc, created.CreatedAt.Kind);
        Assert.True(created.CreatedAt <= DateTime.UtcNow);
    }
}
