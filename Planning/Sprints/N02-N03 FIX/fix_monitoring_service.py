file_path = r"D:\Projects\Test\Claude\CoinPay\CoinPay.Api\Services\BackgroundWorkers\CircleTransactionMonitoringService.cs"

# Read the file
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Old code
old_code = '''        // Get all pending Circle transactions (POL transfers)
        var pendingTransactions = await db.Transactions
            .Where(t => t.Status == "Pending" &&
                       t.Currency == "POL" &&
                       t.TransactionId != null &&
                       t.TransactionId.StartsWith("transaction-"))
            .ToListAsync(cancellationToken);'''

# New code
new_code = '''        // Get all pending Circle transactions (POL transfers)
        // Note: Also includes transactions with empty/null TransactionId to mark them as failed
        var pendingTransactions = await db.Transactions
            .Where(t => t.Status == "Pending" &&
                       t.Currency == "POL")
            .ToListAsync(cancellationToken);'''

# Replace
new_content = content.replace(old_code, new_code)

if new_content != content:
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    print("Successfully updated CircleTransactionMonitoringService.cs")
else:
    print("Pattern not found")
