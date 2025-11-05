using CoinPay.Api.DTOs;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Blockchain;
using CoinPay.Api.Services.UserOperation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoinPay.Api.Controllers;

/// <summary>
/// Controller for blockchain transaction operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUserOperationService _userOperationService;
    private readonly IBlockchainRpcService _blockchainRpcService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TransactionController> _logger;

    // Default USDC token address on Polygon Amoy testnet
    private const string DEFAULT_USDC_ADDRESS = "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582";
    private const int POLYGON_AMOY_CHAIN_ID = 80002;
    private const string POLYGON_AMOY_EXPLORER_URL = "https://amoy.polygonscan.com";

    public TransactionController(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IUserOperationService userOperationService,
        IBlockchainRpcService blockchainRpcService,
        IConfiguration configuration,
        ILogger<TransactionController> logger)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _userOperationService = userOperationService;
        _blockchainRpcService = blockchainRpcService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Submit a gasless USDC transfer
    /// </summary>
    /// <param name="request">Transfer details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction submission response</returns>
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(TransferResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TransferResponse>> SubmitTransfer(
        [FromBody] TransferRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received transfer request: {Amount} USDC to {ToAddress}",
            request.Amount, request.ToAddress);

        try
        {
            var userId = GetUserId();
            if (userId == null)
            {
                _logger.LogWarning("User ID not found in token");
                return Unauthorized(new { error = "User not authenticated" });
            }

            var wallet = await _walletRepository.GetByUserIdAsync(userId.Value, cancellationToken);

            if (wallet == null)
            {
                _logger.LogWarning("No wallet found for user {UserId}", userId);
                return NotFound(new { error = "Wallet not found. Please create a wallet first." });
            }

            // Use provided token address or default to USDC
            var tokenAddress = request.TokenAddress ?? DEFAULT_USDC_ADDRESS;

            // Check wallet balance
            var balance = await _blockchainRpcService.GetUSDCBalanceAsync(wallet.Address, cancellationToken);

            if (balance < request.Amount)
            {
                _logger.LogWarning("Insufficient balance. Required: {Amount}, Available: {Balance}",
                    request.Amount, balance);
                return BadRequest(new
                {
                    error = "Insufficient balance",
                    required = request.Amount,
                    available = balance
                });
            }

            // Construct UserOperation for the transfer
            var userOp = await _userOperationService.ConstructTransferOperationAsync(
                wallet.Address,
                request.ToAddress,
                tokenAddress,
                request.Amount,
                cancellationToken);

            // Submit UserOperation to bundler
            var userOpHash = await _userOperationService.SubmitUserOperationAsync(userOp, cancellationToken);

            // Create transaction record in database
            var transaction = new BlockchainTransaction
            {
                WalletId = wallet.Id,
                UserOpHash = userOpHash,
                FromAddress = wallet.Address,
                ToAddress = request.ToAddress,
                TokenAddress = tokenAddress,
                Amount = (request.Amount * 1_000_000m).ToString(), // Convert to 6 decimals
                AmountDecimal = request.Amount,
                Status = TransactionStatus.Pending,
                ChainId = POLYGON_AMOY_CHAIN_ID,
                TransactionType = "Transfer",
                IsGasless = true,
                CreatedAt = DateTime.UtcNow,
                SubmittedAt = DateTime.UtcNow
            };

            await _transactionRepository.CreateAsync(transaction, cancellationToken);

            _logger.LogInformation("Transfer submitted successfully. TransactionId: {Id}, UserOpHash: {UserOpHash}",
                transaction.Id, userOpHash);

            var response = new TransferResponse
            {
                TransactionId = transaction.Id,
                UserOpHash = userOpHash,
                Status = "Pending",
                FromAddress = wallet.Address,
                ToAddress = request.ToAddress,
                Amount = request.Amount,
                TokenAddress = tokenAddress,
                IsGasless = true,
                SubmittedAt = transaction.SubmittedAt ?? DateTime.UtcNow
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting transfer");
            return StatusCode(500, new { error = "Failed to submit transfer", details = ex.Message });
        }
    }

    /// <summary>
    /// Get transaction status by ID
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction status</returns>
    [HttpGet("{id}/status")]
    [ProducesResponseType(typeof(TransactionStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionStatusResponse>> GetTransactionStatus(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching status for transaction {Id}", id);

        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction {Id} not found", id);
            return NotFound(new { error = "Transaction not found" });
        }

        // If transaction is still pending, check for receipt
        if (transaction.Status == TransactionStatus.Pending && !string.IsNullOrEmpty(transaction.UserOpHash))
        {
            try
            {
                var receipt = await _userOperationService.GetReceiptAsync(transaction.UserOpHash, cancellationToken);

                if (receipt != null)
                {
                    // Update transaction with receipt information
                    await _transactionRepository.UpdateWithReceiptAsync(
                        transaction.Id,
                        receipt.TransactionHash,
                        receipt.BlockNumber,
                        receipt.ActualGasUsed,
                        cancellationToken);

                    // Reload transaction to get updated data
                    transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch receipt for transaction {Id}", id);
            }
        }

        // Generate block explorer URLs
        string? explorerUrl = null;
        string? userOpExplorerUrl = null;

        if (transaction != null)
        {
            if (!string.IsNullOrEmpty(transaction.TransactionHash))
            {
                explorerUrl = GetBlockExplorerUrl(transaction.ChainId, transaction.TransactionHash, "tx");
            }

            if (!string.IsNullOrEmpty(transaction.UserOpHash))
            {
                userOpExplorerUrl = GetJiffyScanUrl(transaction.ChainId, transaction.UserOpHash);
            }
        }

        var response = new TransactionStatusResponse
        {
            TransactionId = transaction!.Id,
            UserOpHash = transaction.UserOpHash,
            TransactionHash = transaction.TransactionHash,
            Status = transaction.Status.ToString(),
            FromAddress = transaction.FromAddress,
            ToAddress = transaction.ToAddress,
            Amount = transaction.AmountDecimal,
            TokenAddress = transaction.TokenAddress,
            IsGasless = transaction.IsGasless,
            GasUsed = transaction.GasUsed,
            BlockNumber = transaction.BlockNumber,
            Confirmations = transaction.Confirmations,
            SubmittedAt = transaction.SubmittedAt ?? transaction.CreatedAt,
            ConfirmedAt = transaction.ConfirmedAt,
            ErrorMessage = transaction.ErrorMessage,
            ExplorerUrl = explorerUrl,
            UserOpExplorerUrl = userOpExplorerUrl
        };

        return Ok(response);
    }

    /// <summary>
    /// Get detailed transaction information by ID
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed transaction information</returns>
    [HttpGet("{id}/details")]
    [ProducesResponseType(typeof(TransactionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionDetailResponse>> GetTransactionDetails(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching details for transaction {Id}", id);

        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction {Id} not found", id);
            return NotFound(new { error = "Transaction not found" });
        }

        // If transaction is pending, check for receipt to get latest status
        if (transaction.Status == TransactionStatus.Pending && !string.IsNullOrEmpty(transaction.UserOpHash))
        {
            try
            {
                var receipt = await _userOperationService.GetReceiptAsync(transaction.UserOpHash, cancellationToken);

                if (receipt != null)
                {
                    // Update transaction with receipt information
                    await _transactionRepository.UpdateWithReceiptAsync(
                        transaction.Id,
                        receipt.TransactionHash,
                        receipt.BlockNumber,
                        receipt.ActualGasUsed,
                        cancellationToken);

                    // Reload transaction to get updated data
                    transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch receipt for transaction {Id}", id);
            }
        }

        // Generate block explorer URLs
        string? explorerUrl = null;
        string? userOpExplorerUrl = null;

        if (transaction != null)
        {
            if (!string.IsNullOrEmpty(transaction.TransactionHash))
            {
                explorerUrl = GetBlockExplorerUrl(transaction.ChainId, transaction.TransactionHash, "tx");
            }

            if (!string.IsNullOrEmpty(transaction.UserOpHash))
            {
                userOpExplorerUrl = GetJiffyScanUrl(transaction.ChainId, transaction.UserOpHash);
            }
        }

        // Get chain name
        var chainName = GetChainName(transaction!.ChainId);

        // Format amount for display
        var formattedAmount = $"{transaction.AmountDecimal:N6} USDC";

        var response = new TransactionDetailResponse
        {
            Id = transaction.Id,
            UserOpHash = transaction.UserOpHash ?? string.Empty,
            TransactionHash = transaction.TransactionHash,
            FromAddress = transaction.FromAddress,
            ToAddress = transaction.ToAddress,
            Amount = transaction.Amount,
            AmountDecimal = transaction.AmountDecimal,
            FormattedAmount = formattedAmount,
            TokenAddress = transaction.TokenAddress,
            TokenSymbol = "USDC",
            Status = transaction.Status.ToString(),
            CreatedAt = transaction.CreatedAt,
            SubmittedAt = transaction.SubmittedAt ?? transaction.CreatedAt,
            ConfirmedAt = transaction.ConfirmedAt,
            ErrorMessage = transaction.ErrorMessage,
            ChainId = transaction.ChainId,
            ChainName = chainName,
            BlockNumber = transaction.BlockNumber,
            Confirmations = transaction.Confirmations,
            GasUsed = transaction.GasUsed,
            GasPaidByUser = 0, // Gasless transactions
            IsGasless = transaction.IsGasless,
            ExplorerUrl = explorerUrl,
            UserOpExplorerUrl = userOpExplorerUrl,
            Nonce = null, // UserOperation nonce - not currently stored
            Signature = null, // UserOperation signature - not currently stored
            TransactionType = transaction.TransactionType ?? "Transfer"
        };

        _logger.LogInformation("Transaction {Id} details retrieved successfully", id);

        return Ok(response);
    }

    /// <summary>
    /// Generate block explorer URL for a transaction
    /// </summary>
    private string GetBlockExplorerUrl(int chainId, string hash, string type = "tx")
    {
        var baseUrl = chainId switch
        {
            80002 => POLYGON_AMOY_EXPLORER_URL, // Polygon Amoy testnet
            137 => "https://polygonscan.com", // Polygon mainnet
            1 => "https://etherscan.io", // Ethereum mainnet
            11155111 => "https://sepolia.etherscan.io", // Sepolia testnet
            _ => POLYGON_AMOY_EXPLORER_URL // Default to Amoy
        };

        return $"{baseUrl}/{type}/{hash}";
    }

    /// <summary>
    /// Generate JiffyScan URL for a UserOperation
    /// </summary>
    private string GetJiffyScanUrl(int chainId, string userOpHash)
    {
        var chainName = chainId switch
        {
            80002 => "amoy",
            137 => "polygon",
            1 => "mainnet",
            11155111 => "sepolia",
            _ => "amoy"
        };

        return $"https://jiffyscan.xyz/userOpHash/{userOpHash}?network={chainName}";
    }

    /// <summary>
    /// Get human-readable chain name
    /// </summary>
    private string GetChainName(int chainId)
    {
        return chainId switch
        {
            80002 => "Polygon Amoy Testnet",
            137 => "Polygon Mainnet",
            1 => "Ethereum Mainnet",
            11155111 => "Sepolia Testnet",
            _ => $"Chain {chainId}"
        };
    }

    /// <summary>
    /// Get transaction history for authenticated user's wallet with advanced filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="status">Filter by status (Pending, Confirmed, Failed)</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="minAmount">Filter by minimum amount</param>
    /// <param name="maxAmount">Filter by maximum amount</param>
    /// <param name="sortBy">Sort by field (CreatedAt, Amount, Status, ConfirmedAt)</param>
    /// <param name="sortDescending">Sort descending (default: true)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction history with pagination</returns>
    [HttpGet("history")]
    [ProducesResponseType(typeof(TransactionHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionHistoryResponse>> GetTransactionHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fetching transaction history - Page: {Page}, PageSize: {PageSize}, Status: {Status}, SortBy: {SortBy}",
            page, pageSize, status, sortBy);

        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User ID not found in token");
            return Unauthorized(new { error = "User not authenticated" });
        }

        var wallet = await _walletRepository.GetByUserIdAsync(userId.Value, cancellationToken);

        if (wallet == null)
        {
            _logger.LogWarning("No wallet found for user {UserId}", userId);
            return NotFound(new { error = "Wallet not found" });
        }

        // Use the enhanced GetHistoryAsync method with filtering
        var (transactions, totalCount) = await _transactionRepository.GetHistoryAsync(
            wallet.Id,
            page,
            pageSize,
            status,
            startDate,
            endDate,
            minAmount,
            maxAmount,
            sortBy,
            sortDescending,
            cancellationToken);

        var response = new TransactionHistoryResponse
        {
            Transactions = transactions.Select(t =>
            {
                // Generate explorer URLs
                string? explorerUrl = null;
                string? userOpExplorerUrl = null;

                if (!string.IsNullOrEmpty(t.TransactionHash))
                {
                    explorerUrl = GetBlockExplorerUrl(t.ChainId, t.TransactionHash, "tx");
                }

                if (!string.IsNullOrEmpty(t.UserOpHash))
                {
                    userOpExplorerUrl = GetJiffyScanUrl(t.ChainId, t.UserOpHash);
                }

                return new TransactionStatusResponse
                {
                    TransactionId = t.Id,
                    UserOpHash = t.UserOpHash,
                    TransactionHash = t.TransactionHash,
                    Status = t.Status.ToString(),
                    FromAddress = t.FromAddress,
                    ToAddress = t.ToAddress,
                    Amount = t.AmountDecimal,
                    TokenAddress = t.TokenAddress,
                    IsGasless = t.IsGasless,
                    GasUsed = t.GasUsed,
                    BlockNumber = t.BlockNumber,
                    Confirmations = t.Confirmations,
                    SubmittedAt = t.SubmittedAt ?? t.CreatedAt,
                    ConfirmedAt = t.ConfirmedAt,
                    ErrorMessage = t.ErrorMessage,
                    ExplorerUrl = explorerUrl,
                    UserOpExplorerUrl = userOpExplorerUrl
                };
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(response);
    }

    /// <summary>
    /// Get wallet balance
    /// </summary>
    /// <param name="address">Wallet address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Wallet balance</returns>
    [HttpGet("balance/{address}")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BalanceResponse>> GetBalance(
        string address,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching balance for address {Address}", address);

        if (!address.StartsWith("0x") || address.Length != 42)
        {
            return BadRequest(new { error = "Invalid Ethereum address format" });
        }

        try
        {
            var balance = await _blockchainRpcService.GetUSDCBalanceAsync(address, cancellationToken);

            var response = new BalanceResponse
            {
                Address = address,
                TokenAddress = DEFAULT_USDC_ADDRESS,
                Balance = balance,
                Currency = "USDC",
                LastUpdated = DateTime.UtcNow
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching balance for address {Address}", address);
            return StatusCode(500, new { error = "Failed to fetch balance", details = ex.Message });
        }
    }

    /// <summary>
    /// Get authenticated user ID from JWT token
    /// </summary>
    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        return null;
    }
}
