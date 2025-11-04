file_path = r"D:\Projects\Test\Claude\CoinPay\CoinPay.Api\Services\BackgroundWorkers\CircleTransactionMonitoringService.cs"

# Read the file
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Old code - direct transaction query
old_code = '''    private async Task MonitorPendingCircleTransactionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var circleService = scope.ServiceProvider.GetRequiredService<ICircleService>();'''

# New code - add wallet repository
new_code = '''    private async Task MonitorPendingCircleTransactionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var circleService = scope.ServiceProvider.GetRequiredService<ICircleService>();
        var walletRepository = scope.ServiceProvider.GetRequiredService<CoinPay.Api.Repositories.IWalletRepository>();'''

content = content.replace(old_code, new_code)

# Replace the transaction status query logic
old_query_logic = '''            try
            {
                // Query Circle API for transaction status
                var circleStatus = await circleService.GetTransactionStatusAsync(
                    transaction.TransactionId!,
                    cancellationToken);

                _logger.LogDebug("Circle transaction {Id} status from API: {Status}",
                    transaction.Id, circleStatus.Status);

                // Update transaction status based on Circle response
                var previousStatus = transaction.Status;
                transaction.Status = circleStatus.Status?.ToUpper() switch
                {
                    "CONFIRMED" => "Completed",
                    "COMPLETE" => "Completed",
                    "FAILED" => "Failed",
                    "CANCELLED" => "Failed",
                    _ => "Pending"
                };

                // Set completion timestamp if status changed to completed or failed
                if (transaction.Status != "Pending" && previousStatus == "Pending")
                {
                    transaction.CompletedAt = DateTime.UtcNow;
                    updatedCount++;

                    _logger.LogInformation(
                        "Circle transaction {Id} status updated: {OldStatus} → {NewStatus}, CircleTransactionId: {CircleTransactionId}",
                        transaction.Id,
                        previousStatus,
                        transaction.Status,
                        transaction.TransactionId);
                }
            }'''

new_query_logic = '''            try
            {
                // For developer-controlled wallets, we need to query all wallet transactions
                // TODO: Store wallet ID with transaction to avoid hardcoding user ID
                var userId = 1; // Hardcoded for now
                var wallet = await walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || string.IsNullOrEmpty(wallet.CircleWalletId))
                {
                    _logger.LogWarning("No Circle wallet found for transaction {Id}", transaction.Id);
                    continue;
                }

                // Get all transactions for this wallet
                var walletTransactions = await circleService.GetWalletTransactionsAsync(
                    wallet.CircleWalletId,
                    cancellationToken);

                // Find our transaction by ID
                var circleStatus = walletTransactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);

                if (circleStatus == null)
                {
                    _logger.LogDebug("Transaction {CircleTransactionId} not found in wallet transactions (may not be synced yet)",
                        transaction.TransactionId);
                    continue;
                }

                _logger.LogDebug("Circle transaction {Id} status from API: {Status}",
                    transaction.Id, circleStatus.Status);

                // Update transaction status based on Circle response
                var previousStatus = transaction.Status;
                transaction.Status = circleStatus.Status?.ToUpper() switch
                {
                    "CONFIRMED" => "Completed",
                    "COMPLETE" => "Completed",
                    "FAILED" => "Failed",
                    "CANCELLED" => "Failed",
                    _ => "Pending"
                };

                // Set completion timestamp if status changed to completed or failed
                if (transaction.Status != "Pending" && previousStatus == "Pending")
                {
                    transaction.CompletedAt = DateTime.UtcNow;
                    updatedCount++;

                    _logger.LogInformation(
                        "Circle transaction {Id} status updated: {OldStatus} → {NewStatus}, CircleTransactionId: {CircleTransactionId}",
                        transaction.Id,
                        previousStatus,
                        transaction.Status,
                        transaction.TransactionId);
                }
            }'''

content = content.replace(old_query_logic, new_query_logic)

# Write the updated content
with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)

print("Successfully updated CircleTransactionMonitoringService to use wallet transactions query")
