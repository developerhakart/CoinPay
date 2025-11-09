using System.Net;
using System.Net.Http.Json;
using CoinPay.Api.Models;

namespace CoinPay.Integration.Tests;

/// <summary>
/// Integration tests for User API endpoints
/// Tests user CRUD operations
/// </summary>
public class UserEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public UserEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>();

        Assert.NotNull(users);
        Assert.True(users.Count >= 0);
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newUser = new User
        {
            Username = $"testuser_{Guid.NewGuid()}",
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(user);
        Assert.True(user.Id > 0);
        Assert.Equal(newUser.Username, user.Username);
        Assert.NotNull(user.CreatedAt);
    }

    [Fact]
    public async Task GetUserById_WithValidId_ShouldReturnUser()
    {
        // Arrange - Create a user first
        var newUser = new User
        {
            Username = $"testuser_{Guid.NewGuid()}",
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var created = await createResponse.Content.ReadFromJsonAsync<User>();

        // Act
        var response = await _client.GetAsync($"/api/users/{created!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(user);
        Assert.Equal(created.Id, user.Id);
        Assert.Equal(created.Username, user.Username);
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetUserByUsername_WithValidUsername_ShouldReturnUser()
    {
        // Arrange
        var username = $"testuser_{Guid.NewGuid()}";
        var newUser = new User
        {
            Username = username,
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        await _client.PostAsJsonAsync("/api/users", newUser);

        // Act
        var response = await _client.GetAsync($"/api/users/username/{username}");

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(user);
        Assert.Equal(username, user.Username);
    }

    [Fact]
    public async Task GetUserByUsername_WithInvalidUsername_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/username/nonexistentuser");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldReturnOk()
    {
        // Arrange - Create a user
        var newUser = new User
        {
            Username = $"testuser_{Guid.NewGuid()}",
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var created = await createResponse.Content.ReadFromJsonAsync<User>();

        // Act - Update the user
        created!.WalletAddress = "0xABCDEF1234567890";

        var updateResponse = await _client.PutAsJsonAsync($"/api/users/{created.Id}", created);

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<User>();

        Assert.Equal("0xABCDEF1234567890", updated!.WalletAddress);
    }

    [Fact]
    public async Task UpdateUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var user = new User
        {
            Id = 99999,
            Username = "testuser",
            CircleUserId = "circle_123"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/99999", user);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WithValidId_ShouldReturnNoContent()
    {
        // Arrange - Create a user
        var newUser = new User
        {
            Username = $"testuser_{Guid.NewGuid()}",
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var created = await createResponse.Content.ReadFromJsonAsync<User>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/users/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/users/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/users/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ShouldSetCreatedAtToUtc()
    {
        // Arrange
        var newUser = new User
        {
            Username = $"testuser_{Guid.NewGuid()}",
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", newUser);
        var created = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.NotNull(created!.CreatedAt);
        Assert.Equal(DateTimeKind.Utc, created.CreatedAt.Kind);
        Assert.True(created.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateUsername_ShouldFail()
    {
        // Arrange
        var username = $"testuser_{Guid.NewGuid()}";
        var user1 = new User
        {
            Username = username,
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        var user2 = new User
        {
            Username = username, // Same username
            CircleUserId = $"circle_{Guid.NewGuid()}"
        };

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/users", user1);
        var response2 = await _client.PostAsJsonAsync("/api/users", user2);

        // Assert
        response1.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }
}
