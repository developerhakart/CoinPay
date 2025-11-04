file_path = r"D:\Projects\Test\Claude\CoinPay\CoinPay.Api\Program.cs"

# Read the file
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Add webhook service registration
old_services = '''// Use real CircleService for testnet testing
builder.Services.AddScoped<ICircleService, CircleService>();
// Use MockCircleService for MVP testing (no real Circle API calls):
// builder.Services.AddScoped<ICircleService, MockCircleService>();
builder.Services.AddScoped<IAuthService, AuthService>();'''

new_services = '''// Use real CircleService for testnet testing
builder.Services.AddScoped<ICircleService, CircleService>();
// Use MockCircleService for MVP testing (no real Circle API calls):
// builder.Services.AddScoped<ICircleService, MockCircleService>();
builder.Services.AddScoped<ICircleWebhookHandler, CircleWebhookHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();'''

content = content.replace(old_services, new_services)

# 2. Add webhook endpoint before health check endpoints
# Find the health check endpoints section
old_healthcheck_section = '''// Health check endpoints
app.MapHealthChecks("/health",'''

new_webhook_endpoint = '''// Circle webhook endpoint for transaction status updates
app.MapPost("/api/webhooks/circle", async (
    CircleWebhookNotification notification,
    ICircleWebhookHandler webhookHandler,
    ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Received Circle webhook notification: Type={Type}, NotificationId={NotificationId}",
            notification.NotificationType, notification.NotificationId);

        await webhookHandler.ProcessWebhookAsync(notification);

        return Results.Ok(new { success = true, message = "Webhook processed successfully" });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing Circle webhook");
        return Results.StatusCode(500);
    }
})
.WithName("CircleWebhook")
.WithTags("Webhooks")
.WithSummary("Circle webhook endpoint for transaction updates")
.WithDescription("Receives transaction status updates from Circle API")
.AllowAnonymous(); // Circle webhooks don't use JWT

// Health check endpoints
app.MapHealthChecks("/health",'''

content = content.replace(old_healthcheck_section, new_webhook_endpoint)

# Write the updated content
with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)

print("Successfully added Circle webhook endpoint and service registration")
