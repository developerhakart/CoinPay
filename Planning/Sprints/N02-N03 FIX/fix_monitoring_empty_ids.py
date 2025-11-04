file_path = r"D:\Projects\Test\Claude\CoinPay\CoinPay.Api\Services\BackgroundWorkers\CircleTransactionMonitoringService.cs"

# Read the file
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Old code - right after the age check
old_code = '''            try
            {
                // Query Circle API for transaction status
                var circleStatus = await circleService.GetTransactionStatusAsync(
                    transaction.TransactionId!,
                    cancellationToken);'''

# New code - add empty transaction ID check
new_code = '''            // Check if transaction has empty/null Circle transaction ID (API call failed)
            if (string.IsNullOrEmpty(transaction.TransactionId))
            {
                _logger.LogWarning("Circle transaction {Id} has empty TransactionId, marking as failed",
                    transaction.Id);

                transaction.Status = "Failed";
                transaction.CompletedAt = DateTime.UtcNow;
                failedCount++;
                continue;
            }

            try
            {
                // Query Circle API for transaction status
                var circleStatus = await circleService.GetTransactionStatusAsync(
                    transaction.TransactionId!,
                    cancellationToken);'''

# Replace
new_content = content.replace(old_code, new_code)

if new_content != content:
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    print("Successfully updated CircleTransactionMonitoringService.cs with empty ID check")
else:
    print("Pattern not found")
