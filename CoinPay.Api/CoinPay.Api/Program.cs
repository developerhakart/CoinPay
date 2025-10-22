using Microsoft.EntityFrameworkCore;
using CoinPay.Api.Data;
using CoinPay.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "CoinPay API",
        Version = "v1",
        Description = "A simple transactions API for CoinPay application",
        Contact = new()
        {
            Name = "CoinPay Team",
            Email = "developerhakart@yahoo.com"
        }
    });
});

// Add DbContext with InMemory database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CoinPayDb"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoinPay API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "CoinPay API Documentation";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// API Endpoints

// GET: Get all transactions
app.MapGet("/api/transactions", async (AppDbContext db) =>
{
    var transactions = await db.Transactions.ToListAsync();
    return Results.Ok(transactions);
})
.WithName("GetAllTransactions")
.WithTags("Transactions")
.WithSummary("Get all transactions")
.WithDescription("Retrieves a list of all transactions in the system");

// GET: Get transaction by ID
app.MapGet("/api/transactions/{id}", async (int id, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    return transaction is not null ? Results.Ok(transaction) : Results.NotFound();
})
.WithName("GetTransactionById")
.WithTags("Transactions")
.WithSummary("Get transaction by ID")
.WithDescription("Retrieves a specific transaction by its unique identifier");

// GET: Get transactions by status
app.MapGet("/api/transactions/status/{status}", async (string status, AppDbContext db) =>
{
    var transactions = await db.Transactions
        .Where(t => t.Status.ToLower() == status.ToLower())
        .ToListAsync();
    return Results.Ok(transactions);
})
.WithName("GetTransactionsByStatus")
.WithTags("Transactions")
.WithSummary("Get transactions by status")
.WithDescription("Retrieves all transactions filtered by status (Pending, Completed, Failed)");

// POST: Create a new transaction
app.MapPost("/api/transactions", async (Transaction transaction, AppDbContext db) =>
{
    // Generate transaction ID if not provided
    if (string.IsNullOrEmpty(transaction.TransactionId))
    {
        transaction.TransactionId = $"TXN{DateTime.UtcNow.Ticks}";
    }

    transaction.CreatedAt = DateTime.UtcNow;

    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();

    return Results.Created($"/api/transactions/{transaction.Id}", transaction);
})
.WithName("CreateTransaction")
.WithTags("Transactions")
.WithSummary("Create a new transaction")
.WithDescription("Creates a new transaction with the provided details");

// PUT: Update transaction
app.MapPut("/api/transactions/{id}", async (int id, Transaction updatedTransaction, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    if (transaction is null)
    {
        return Results.NotFound();
    }

    transaction.Amount = updatedTransaction.Amount;
    transaction.Currency = updatedTransaction.Currency;
    transaction.Type = updatedTransaction.Type;
    transaction.Status = updatedTransaction.Status;
    transaction.SenderName = updatedTransaction.SenderName;
    transaction.ReceiverName = updatedTransaction.ReceiverName;
    transaction.Description = updatedTransaction.Description;

    if (updatedTransaction.Status == "Completed" && transaction.CompletedAt == null)
    {
        transaction.CompletedAt = DateTime.UtcNow;
    }

    await db.SaveChangesAsync();

    return Results.Ok(transaction);
})
.WithName("UpdateTransaction")
.WithTags("Transactions")
.WithSummary("Update a transaction")
.WithDescription("Updates an existing transaction with new details");

// DELETE: Delete transaction
app.MapDelete("/api/transactions/{id}", async (int id, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    if (transaction is null)
    {
        return Results.NotFound();
    }

    db.Transactions.Remove(transaction);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteTransaction")
.WithTags("Transactions")
.WithSummary("Delete a transaction")
.WithDescription("Deletes a transaction from the system");

// PATCH: Update transaction status
app.MapPatch("/api/transactions/{id}/status", async (int id, string status, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    if (transaction is null)
    {
        return Results.NotFound();
    }

    transaction.Status = status;
    if (status == "Completed" && transaction.CompletedAt == null)
    {
        transaction.CompletedAt = DateTime.UtcNow;
    }

    await db.SaveChangesAsync();

    return Results.Ok(transaction);
})
.WithName("UpdateTransactionStatus")
.WithTags("Transactions")
.WithSummary("Update transaction status")
.WithDescription("Updates the status of a transaction (Pending, Completed, Failed)");

app.Run();
