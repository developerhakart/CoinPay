using Nethereum.Web3;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;

namespace CoinPay.Api.Services.Blockchain;

/// <summary>
/// Real blockchain RPC service for Polygon Amoy testnet
/// Queries actual blockchain data using Nethereum
/// </summary>
public class PolygonAmoyRpcService : IBlockchainRpcService
{
    private readonly ILogger<PolygonAmoyRpcService> _logger;
    private readonly Web3 _web3;
    private readonly string _usdcContractAddress;

    // Polygon Amoy Testnet RPC URL
    private const string POLYGON_AMOY_RPC = "https://rpc-amoy.polygon.technology";

    // USDC Contract Address on Polygon Amoy
    // Circle's USDC: 0x41E94Eb019C0762f9Bfcf9Fb1E58725BfB0e7582
    private const string USDC_CONTRACT = "0x41E94Eb019C0762f9Bfcf9Fb1E58725BfB0e7582";

    public PolygonAmoyRpcService(
        ILogger<PolygonAmoyRpcService> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        // Get RPC URL from configuration or use default
        var rpcUrl = configuration["Blockchain:PolygonAmoy:RpcUrl"] ?? POLYGON_AMOY_RPC;
        _usdcContractAddress = configuration["Blockchain:PolygonAmoy:USDCContract"] ?? USDC_CONTRACT;

        _logger.LogInformation("Initializing Polygon Amoy RPC Service with RPC: {RpcUrl}, USDC Contract: {USDCContract}",
            rpcUrl, _usdcContractAddress);

        _web3 = new Web3(rpcUrl);
    }

    /// <summary>
    /// Get real USDC balance from Polygon Amoy blockchain
    /// </summary>
    public async Task<decimal> GetUSDCBalanceAsync(string walletAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("[PolygonAmoy] Fetching USDC balance for address: {Address}", walletAddress);

            // Create ERC20 contract service
            var tokenService = _web3.Eth.ERC20.GetContractService(_usdcContractAddress);

            // Get balance (returns BigInteger in smallest unit - 6 decimals for USDC)
            var balanceInSmallestUnit = await tokenService.BalanceOfQueryAsync(walletAddress);

            // Convert from smallest unit to USDC (divide by 10^6 since USDC has 6 decimals)
            var balance = (decimal)balanceInSmallestUnit / 1_000_000m;

            _logger.LogInformation("[PolygonAmoy] USDC balance for {Address}: {Balance} USDC",
                walletAddress, balance);

            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PolygonAmoy] Failed to fetch USDC balance for {Address}", walletAddress);
            throw;
        }
    }

    /// <summary>
    /// Get native MATIC balance from Polygon Amoy blockchain
    /// </summary>
    public async Task<decimal> GetNativeBalanceAsync(string walletAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("[PolygonAmoy] Fetching MATIC balance for address: {Address}", walletAddress);

            // Get balance in Wei
            var balanceInWei = await _web3.Eth.GetBalance.SendRequestAsync(walletAddress);

            // Convert from Wei to MATIC (divide by 10^18)
            var balance = Web3.Convert.FromWei(balanceInWei.Value);

            _logger.LogInformation("[PolygonAmoy] MATIC balance for {Address}: {Balance} MATIC",
                walletAddress, balance);

            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PolygonAmoy] Failed to fetch MATIC balance for {Address}", walletAddress);
            throw;
        }
    }

    public async Task<TransactionReceipt?> GetTransactionReceiptAsync(string txHash, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("[PolygonAmoy] Fetching transaction receipt for tx: {TxHash}", txHash);

            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);

            if (receipt == null)
            {
                _logger.LogWarning("[PolygonAmoy] Transaction receipt not found for tx: {TxHash}", txHash);
                return null;
            }

            // Get block to extract timestamp
            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(
                new BlockParameter(receipt.BlockNumber));

            var result = new TransactionReceipt
            {
                TransactionHash = receipt.TransactionHash,
                BlockHash = receipt.BlockHash,
                BlockNumber = (long)receipt.BlockNumber.Value,
                From = receipt.From,
                To = receipt.To,
                GasUsed = (decimal)receipt.GasUsed.Value,
                Status = receipt.Status?.Value == 1 ? "success" : "failed",
                Timestamp = block != null ? DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).UtcDateTime : null
            };

            _logger.LogInformation("[PolygonAmoy] Transaction receipt for {TxHash}: Status={Status}, Block={BlockNumber}",
                txHash, result.Status, result.BlockNumber);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PolygonAmoy] Failed to fetch transaction receipt for {TxHash}", txHash);
            throw;
        }
    }

    public async Task<decimal> GetGasPriceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("[PolygonAmoy] Fetching current gas price");

            var gasPriceInWei = await _web3.Eth.GasPrice.SendRequestAsync();

            // Convert from Wei to Gwei
            var gasPriceInGwei = Web3.Convert.FromWei(gasPriceInWei.Value, Nethereum.Util.UnitConversion.EthUnit.Gwei);

            _logger.LogInformation("[PolygonAmoy] Current gas price: {GasPrice} Gwei", gasPriceInGwei);

            return gasPriceInGwei;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PolygonAmoy] Failed to fetch gas price");
            throw;
        }
    }

    public async Task<long> GetBlockNumberAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("[PolygonAmoy] Fetching current block number");

            var blockNumber = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            var blockNum = (long)blockNumber.Value;

            _logger.LogInformation("[PolygonAmoy] Current block number: {BlockNumber}", blockNum);

            return blockNum;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PolygonAmoy] Failed to fetch block number");
            throw;
        }
    }
}
