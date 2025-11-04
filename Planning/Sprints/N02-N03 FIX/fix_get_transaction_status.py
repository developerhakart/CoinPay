file_path = r"D:\Projects\Test\Claude\CoinPay\CoinPay.Api\Services\Circle\CircleService.cs"

# Read the file
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Old code for GetTransactionStatusAsync
old_code = '''    /// <inheritdoc/>
    public async Task<CircleTransactionResponse> GetTransactionStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Retrieving transaction status for {TransactionId} [CorrelationId: {CorrelationId}]",
            transactionId,
            correlationId);

        var request = new RestRequest($"/transactions/{transactionId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        request.AddHeader("X-Circle-App-Id", _options.AppId);
        request.AddHeader("X-Correlation-Id", correlationId);

        var response = await ExecuteWithRetryAsync<CircleTransactionResponse>(
            request,
            correlationId,
            cancellationToken);

        return response;
    }'''

# New code with developer endpoint and proper parsing
new_code = '''    /// <inheritdoc/>
    public async Task<CircleTransactionResponse> GetTransactionStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation(
            "Retrieving transaction status for {TransactionId} [CorrelationId: {CorrelationId}]",
            transactionId,
            correlationId);

        // Use developer endpoint for developer-controlled wallet transactions
        var request = new RestRequest($"/developer/transactions/{transactionId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");
        request.AddHeader("X-Correlation-Id", correlationId);

        // Execute request with custom parsing (Circle wraps response in "data" object)
        var rawResponse = await _retryPolicy.ExecuteAsync(async () =>
            await _client.ExecuteAsync(request, cancellationToken));

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

        _logger.LogDebug("Transaction status retrieved: ID={Id}, Status={Status} [CorrelationId: {CorrelationId}]",
            response.TransactionId, response.Status, correlationId);

        return response;
    }'''

# Replace
new_content = content.replace(old_code, new_code)

if new_content != content:
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    print("Successfully updated GetTransactionStatusAsync")
else:
    print("Pattern not found")
