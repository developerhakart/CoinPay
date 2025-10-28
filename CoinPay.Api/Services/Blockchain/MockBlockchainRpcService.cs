namespace CoinPay.Api.Services.Blockchain;

/// <summary>
/// Mock implementation of blockchain RPC service for MVP/development testing.
/// Returns simulated blockchain data without making actual RPC calls.
/// </summary>
public class MockBlockchainRpcService : IBlockchainRpcService
{
    private readonly ILogger<MockBlockchainRpcService> _logger;
    private readonly Random _random = new();

    public MockBlockchainRpcService(ILogger<MockBlockchainRpcService> logger)
    {
        _logger = logger;
    }

    public Task<decimal> GetUSDCBalanceAsync(string walletAddress, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockBlockchain] Getting USDC balance for address: {Address}", walletAddress);

        // For MVP: Return mock balance between 0 and 1000 USDC
        var balance = (decimal)(_random.NextDouble() * 1000);
        balance = Math.Round(balance, 2);

        _logger.LogDebug("[MockBlockchain] Mock USDC balance for {Address}: {Balance}", walletAddress, balance);
        return Task.FromResult(balance);
    }

    public Task<decimal> GetNativeBalanceAsync(string walletAddress, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockBlockchain] Getting native balance (MATIC) for address: {Address}", walletAddress);

        // For MVP: Return mock balance between 0 and 10 MATIC
        var balance = (decimal)(_random.NextDouble() * 10);
        balance = Math.Round(balance, 4);

        _logger.LogDebug("[MockBlockchain] Mock MATIC balance for {Address}: {Balance}", walletAddress, balance);
        return Task.FromResult(balance);
    }

    public Task<TransactionReceipt?> GetTransactionReceiptAsync(string txHash, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockBlockchain] Getting transaction receipt for tx: {TxHash}", txHash);

        // For MVP: Return mock transaction receipt
        var receipt = new TransactionReceipt
        {
            TransactionHash = txHash,
            BlockHash = $"0x{Guid.NewGuid():N}",
            BlockNumber = _random.Next(40000000, 50000000),
            From = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            To = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            GasUsed = (decimal)(_random.NextDouble() * 100000),
            Status = "success",
            Timestamp = DateTime.UtcNow.AddMinutes(-_random.Next(1, 60))
        };

        return Task.FromResult<TransactionReceipt?>(receipt);
    }

    public Task<decimal> GetGasPriceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockBlockchain] Getting current gas price");

        // For MVP: Return mock gas price (in Gwei)
        var gasPrice = (decimal)(_random.NextDouble() * 100 + 20); // 20-120 Gwei
        gasPrice = Math.Round(gasPrice, 2);

        _logger.LogDebug("[MockBlockchain] Mock gas price: {GasPrice} Gwei", gasPrice);
        return Task.FromResult(gasPrice);
    }

    public Task<long> GetBlockNumberAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MockBlockchain] Getting current block number");

        // For MVP: Return mock block number (Polygon Amoy range)
        var blockNumber = _random.Next(40000000, 50000000);

        _logger.LogDebug("[MockBlockchain] Mock block number: {BlockNumber}", blockNumber);
        return Task.FromResult((long)blockNumber);
    }
}
