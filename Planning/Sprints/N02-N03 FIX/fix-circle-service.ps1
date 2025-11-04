# PowerShell script to fix CircleService.cs
$filePath = "D:\Projects\Test\Claude\CoinPay\CoinPay.Api\Services\Circle\CircleService.cs"
$content = Get-Content $filePath -Raw

# Define the old code block to replace
$oldCode = @'
        var response = await ExecuteWithRetryAsync<CircleTransactionResponse>(
            restRequest,
            correlationId,
            cancellationToken);

        _logger.LogInformation(
            "Developer transfer executed. TransactionId: {TransactionId}, Status: {Status} [CorrelationId: {CorrelationId}]",
            response.TransactionId,
            response.Status,
            correlationId);

        return response;
'@

# Define the new code block
$newCode = @'
        // Execute request with custom parsing (Circle wraps response in "data" object)
        var rawResponse = await _retryPolicy.ExecuteAsync(async () =>
            await _client.ExecuteAsync(restRequest, cancellationToken));

        if (!rawResponse.IsSuccessful)
        {
            _logger.LogError(
                "Circle API request failed. StatusCode: {StatusCode}, Error: {Error}, Content: {Content} [CorrelationId: {CorrelationId}]",
                rawResponse.StatusCode,
                rawResponse.ErrorMessage,
                rawResponse.Content,
                correlationId);

            throw new HttpRequestException(
                $"Circle API request failed with status {rawResponse.StatusCode}: {rawResponse.ErrorMessage ?? rawResponse.Content}");
        }

        _logger.LogDebug("Circle API Raw Response: {Content} [CorrelationId: {CorrelationId}]",
            rawResponse.Content, correlationId);

        // Parse nested response: { "data": { "id": "...", "state": "...", ... } }
        var jsonDoc = System.Text.Json.JsonDocument.Parse(rawResponse.Content ?? "{}");
        var dataElement = jsonDoc.RootElement.GetProperty("data");

        // Map Circle's field names to our model (id -> TransactionId, state -> Status)
        var response = new CircleTransactionResponse
        {
            TransactionId = dataElement.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? string.Empty : string.Empty,
            Status = dataElement.TryGetProperty("state", out var stateProp) ? stateProp.GetString() ?? string.Empty : string.Empty,
            TxHash = dataElement.TryGetProperty("txHash", out var txHashProp) ? txHashProp.GetString() : null,
            Blockchain = dataElement.TryGetProperty("blockchain", out var blockchainProp) ? blockchainProp.GetString() ?? string.Empty : string.Empty,
            From = dataElement.TryGetProperty("sourceAddress", out var fromProp) ? fromProp.GetString() ?? string.Empty : string.Empty,
            To = dataElement.TryGetProperty("destinationAddress", out var toProp) ? toProp.GetString() ?? string.Empty : string.Empty,
            TokenAddress = dataElement.TryGetProperty("tokenId", out var tokenProp) ? tokenProp.GetString() : null
        };

        // Parse amounts array
        if (dataElement.TryGetProperty("amounts", out var amountsProp) && amountsProp.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            var amountsArray = amountsProp.EnumerateArray().ToList();
            if (amountsArray.Count > 0)
            {
                response.Amount = amountsArray[0].GetString() ?? string.Empty;
            }
        }

        // Parse timestamps
        if (dataElement.TryGetProperty("createDate", out var createProp) && createProp.TryGetDateTime(out var createDate))
        {
            response.CreatedAt = createDate;
        }
        if (dataElement.TryGetProperty("updateDate", out var updateProp) && updateProp.TryGetDateTime(out var updateDate))
        {
            response.UpdatedAt = updateDate;
        }

        _logger.LogInformation(
            "Developer transfer executed. TransactionId: {TransactionId}, Status: {Status} [CorrelationId: {CorrelationId}]",
            response.TransactionId,
            response.Status,
            correlationId);

        return response;
'@

# Replace the code
$newContent = $content.Replace($oldCode, $newCode)

if ($newContent -ne $content) {
    Set-Content -Path $filePath -Value $newContent -NoNewline
    Write-Host "Successfully updated CircleService.cs"
} else {
    Write-Host "No changes made - pattern not found"
}
