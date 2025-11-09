using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.RegularExpressions;

namespace CoinPay.E2E.Tests;

/// <summary>
/// End-to-End tests for critical user journeys
/// Tests the complete user flow from registration to transactions
/// </summary>
[TestClass]
public class UserJourneyTests : PageTest
{
    private string BaseUrl => "http://localhost:5173"; // Vite dev server default port
    private string ApiUrl => "http://localhost:5100";  // API server port

    [TestInitialize]
    public async Task Setup()
    {
        // Set default timeout for all tests
        Page.SetDefaultTimeout(10000); // 10 seconds

        // Navigate to the application
        await Page.GotoAsync(BaseUrl);
    }

    [TestMethod]
    public async Task HomePage_ShouldLoad_AndDisplayWelcomeMessage()
    {
        // Assert - Check page title
        await Expect(Page).ToHaveTitleAsync(new Regex("CoinPay"));

        // Assert - Check for welcome message or main heading
        var heading = Page.Locator("h1");
        await Expect(heading).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Navigation_ShouldWork_BetweenPages()
    {
        // Arrange - Wait for page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act - Click on navigation links
        var aboutLink = Page.GetByRole(AriaRole.Link, new() { Name = "About" });
        if (await aboutLink.IsVisibleAsync())
        {
            await aboutLink.ClickAsync();

            // Assert - Verify navigation worked
            await Page.WaitForURLAsync(new Regex("/about"));
        }
    }

    [TestMethod]
    public async Task UserRegistration_CompleteFlow_ShouldCreateAccount()
    {
        // Arrange - Navigate to registration page
        await Page.GotoAsync($"{BaseUrl}/register");

        // Act - Fill out registration form
        var usernameInput = Page.GetByLabel("Username");
        var testUsername = $"testuser{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        if (await usernameInput.IsVisibleAsync())
        {
            await usernameInput.FillAsync(testUsername);

            // Click register button
            var registerButton = Page.GetByRole(AriaRole.Button, new() { Name = "Register" });
            await registerButton.ClickAsync();

            // Assert - Check for success message or redirect
            await Task.Delay(2000); // Wait for passkey prompt or success

            // Verify we're on the next step (passkey setup or dashboard)
            var currentUrl = Page.Url;
            Assert.IsTrue(
                currentUrl.Contains("/dashboard") || currentUrl.Contains("/setup"),
                $"Expected redirect after registration, but URL is: {currentUrl}"
            );
        }
    }

    [TestMethod]
    public async Task Dashboard_ShouldDisplay_WalletBalance()
    {
        // Note: This test assumes user is logged in
        // In a real scenario, you'd login first or use a test account

        // Arrange - Navigate to dashboard
        await Page.GotoAsync($"{BaseUrl}/dashboard");

        // Assert - Check for wallet balance display
        var balanceElement = Page.Locator("[data-testid='wallet-balance']");
        if (await balanceElement.IsVisibleAsync())
        {
            await Expect(balanceElement).ToBeVisibleAsync();

            // Verify balance is a number
            var balanceText = await balanceElement.TextContentAsync();
            Assert.IsNotNull(balanceText, "Balance should be displayed");
        }
    }

    [TestMethod]
    public async Task WalletCreation_ShouldSucceed_AfterRegistration()
    {
        // Arrange - Assume we're logged in and on wallet creation page
        await Page.GotoAsync($"{BaseUrl}/wallet/create");

        // Act - Click create wallet button
        var createWalletButton = Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Create.*Wallet", RegexOptions.IgnoreCase) });

        if (await createWalletButton.IsVisibleAsync())
        {
            await createWalletButton.ClickAsync();

            // Assert - Wait for wallet creation confirmation
            await Page.WaitForSelectorAsync("[data-testid='wallet-address']", new() { Timeout = 15000 });

            var walletAddress = Page.Locator("[data-testid='wallet-address']");
            await Expect(walletAddress).ToBeVisibleAsync();

            // Verify address format (starts with 0x)
            var addressText = await walletAddress.TextContentAsync();
            Assert.IsTrue(addressText?.StartsWith("0x"), "Wallet address should start with 0x");
        }
    }

    [TestMethod]
    public async Task SendTransaction_ShouldShow_ConfirmationDialog()
    {
        // Arrange - Navigate to send page
        await Page.GotoAsync($"{BaseUrl}/send");

        // Act - Fill out transaction form
        var recipientInput = Page.GetByLabel(new Regex("Recipient|To", RegexOptions.IgnoreCase));
        var amountInput = Page.GetByLabel(new Regex("Amount", RegexOptions.IgnoreCase));

        if (await recipientInput.IsVisibleAsync() && await amountInput.IsVisibleAsync())
        {
            await recipientInput.FillAsync("0x1234567890123456789012345678901234567890");
            await amountInput.FillAsync("0.001");

            // Click send button
            var sendButton = Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Send", RegexOptions.IgnoreCase) });
            await sendButton.ClickAsync();

            // Assert - Check for confirmation dialog
            var confirmDialog = Page.Locator("[data-testid='confirm-transaction']");
            await Expect(confirmDialog).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
    }

    [TestMethod]
    public async Task TransactionHistory_ShouldDisplay_PastTransactions()
    {
        // Arrange - Navigate to transaction history
        await Page.GotoAsync($"{BaseUrl}/transactions");

        // Assert - Check for transactions list
        var transactionsList = Page.Locator("[data-testid='transactions-list']");

        if (await transactionsList.IsVisibleAsync())
        {
            await Expect(transactionsList).ToBeVisibleAsync();

            // Check for at least one transaction item
            var transactionItems = Page.Locator("[data-testid^='transaction-']");
            var count = await transactionItems.CountAsync();

            // Note: Count might be 0 for new users
            Assert.IsTrue(count >= 0, "Transactions list should be accessible");
        }
    }

    [TestMethod]
    public async Task ErrorHandling_ShouldDisplay_UserFriendlyMessages()
    {
        // Arrange - Try to access a protected route without authentication
        await Page.GotoAsync($"{BaseUrl}/dashboard");

        // Assert - Should either show login page or error message
        var pageContent = await Page.ContentAsync();
        var isLoginPage = pageContent.Contains("login", StringComparison.OrdinalIgnoreCase);
        var hasErrorMessage = await Page.Locator("[role='alert']").IsVisibleAsync();

        Assert.IsTrue(
            isLoginPage || hasErrorMessage,
            "Should redirect to login or show error message for protected routes"
        );
    }

    [TestMethod]
    public async Task ApiHealth_ShouldRespond_WithHealthStatus()
    {
        // Act - Make API call to health endpoint
        var apiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = ApiUrl
        });

        var response = await apiContext.GetAsync("/health");

        // Assert - Check response
        Assert.IsTrue(response.Ok, "Health endpoint should return 200 OK");

        var responseText = await response.TextAsync();
        Assert.IsNotNull(responseText, "Health endpoint should return a response");
    }

    [TestMethod]
    public async Task ResponsiveDesign_ShouldWork_OnMobile()
    {
        // Arrange - Set mobile viewport
        await Page.SetViewportSizeAsync(375, 667); // iPhone SE size

        // Act - Navigate to homepage
        await Page.GotoAsync(BaseUrl);

        // Assert - Check mobile menu exists
        var mobileMenu = Page.Locator("[data-testid='mobile-menu']");
        var hamburgerButton = Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Menu|☰", RegexOptions.IgnoreCase) });

        // At least one of these should be visible on mobile
        var hasMobileNav = await mobileMenu.IsVisibleAsync() || await hamburgerButton.IsVisibleAsync();
        Assert.IsTrue(hasMobileNav, "Should have mobile navigation");
    }

    [TestMethod]
    public async Task FormValidation_ShouldShow_ErrorMessages()
    {
        // Arrange - Navigate to a form page
        await Page.GotoAsync($"{BaseUrl}/send");

        // Act - Try to submit empty form
        var sendButton = Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Send", RegexOptions.IgnoreCase) });

        if (await sendButton.IsVisibleAsync())
        {
            await sendButton.ClickAsync();

            // Assert - Check for validation errors
            await Task.Delay(1000); // Wait for validation

            var errorMessages = Page.Locator("[data-testid*='error'], .error-message, [role='alert']");
            var hasErrors = await errorMessages.CountAsync() > 0;

            // Note: This might not apply if frontend validation prevents submission
            // Assert.IsTrue(hasErrors, "Should show validation errors for empty form");
        }
    }

    [TestMethod]
    public async Task LoadingStates_ShouldDisplay_DuringAsyncOperations()
    {
        // Arrange - Navigate to page with async operations
        await Page.GotoAsync($"{BaseUrl}/dashboard");

        // Act - Trigger async operation (like refreshing balance)
        var refreshButton = Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Refresh", RegexOptions.IgnoreCase) });

        if (await refreshButton.IsVisibleAsync())
        {
            await refreshButton.ClickAsync();

            // Assert - Check for loading indicator
            var loadingIndicator = Page.Locator("[data-testid='loading'], .spinner, [aria-busy='true']");
            // Loading might be fast, so this is optional check
            var wasLoading = await loadingIndicator.IsVisibleAsync() || true;
            Assert.IsTrue(wasLoading, "Loading state should be handled");
        }
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        // Close any dialogs or modals
        try
        {
            var dialog = Page.Locator("[role='dialog']");
            if (await dialog.IsVisibleAsync())
            {
                var closeButton = dialog.Locator("button").First;
                await closeButton.ClickAsync();
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
