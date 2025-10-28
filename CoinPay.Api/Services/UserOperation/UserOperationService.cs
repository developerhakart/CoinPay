using System.Numerics;
using System.Text;
using CoinPay.Api.Services.Paymaster;

namespace CoinPay.Api.Services.UserOperation;

/// <summary>
/// Implementation of UserOperation service for ERC-4337 Account Abstraction
/// </summary>
public class UserOperationService : IUserOperationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPaymasterService _paymasterService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserOperationService> _logger;

    private const string ENTRY_POINT_ADDRESS = "0x5FF137D4b0FDCD49DcA30c7CF57E578a026d2789"; // ERC-4337 v0.6 EntryPoint
    private const string BUNDLER_HTTP_CLIENT = "CircleBundler";

    public UserOperationService(
        IHttpClientFactory httpClientFactory,
        IPaymasterService paymasterService,
        IConfiguration configuration,
        ILogger<UserOperationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _paymasterService = paymasterService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserOperationDto> ConstructTransferOperationAsync(
        string fromAddress,
        string toAddress,
        string tokenAddress,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Constructing UserOperation for transfer: {From} -> {To}, Amount: {Amount}",
            fromAddress, toAddress, amount);

        // Convert amount to wei (USDC has 6 decimals)
        var amountInWei = ConvertToWei(amount, 6);

        // Encode ERC-20 transfer function call
        var callData = EncodeERC20Transfer(tokenAddress, toAddress, amountInWei);

        // Get nonce from entry point
        var nonce = await GetNonceAsync(fromAddress, cancellationToken);

        // Create UserOperation
        var userOp = new UserOperationDto
        {
            Sender = fromAddress,
            Nonce = nonce,
            InitCode = "0x", // Wallet already deployed
            CallData = callData,
            CallGasLimit = "0x0", // Will be estimated
            VerificationGasLimit = "0x0", // Will be estimated
            PreVerificationGas = "0x0", // Will be estimated
            MaxFeePerGas = "0x0", // Will be estimated
            MaxPriorityFeePerGas = "0x0", // Will be estimated
            PaymasterAndData = "0x", // Will be added by paymaster service
            Signature = "0x" // Will be signed by Circle SDK
        };

        // Estimate gas
        var gasEstimate = await EstimateGasAsync(userOp, cancellationToken);
        userOp.CallGasLimit = gasEstimate.CallGasLimit;
        userOp.VerificationGasLimit = gasEstimate.VerificationGasLimit;
        userOp.PreVerificationGas = gasEstimate.PreVerificationGas;
        userOp.MaxFeePerGas = gasEstimate.MaxFeePerGas;
        userOp.MaxPriorityFeePerGas = gasEstimate.MaxPriorityFeePerGas;

        // Get paymaster data for gas sponsorship
        var paymasterData = await _paymasterService.GetPaymasterDataAsync(userOp, cancellationToken);
        userOp.PaymasterAndData = paymasterData;

        _logger.LogInformation("UserOperation constructed successfully for sender {Sender}", fromAddress);

        return userOp;
    }

    public async Task<string> SubmitUserOperationAsync(UserOperationDto userOp, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Submitting UserOperation for sender {Sender}", userOp.Sender);

        var client = _httpClientFactory.CreateClient(BUNDLER_HTTP_CLIENT);

        var request = new
        {
            jsonrpc = "2.0",
            id = 1,
            method = "eth_sendUserOperation",
            @params = new object[]
            {
                userOp,
                ENTRY_POINT_ADDRESS
            }
        };

        var response = await client.PostAsJsonAsync("", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonRpcResponse<string>>(cancellationToken);

        if (result?.Result == null)
        {
            throw new InvalidOperationException("Failed to submit UserOperation: No userOpHash returned");
        }

        _logger.LogInformation("UserOperation submitted successfully. UserOpHash: {UserOpHash}", result.Result);

        return result.Result;
    }

    public async Task<UserOperationReceipt?> GetReceiptAsync(string userOpHash, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching UserOperation receipt for {UserOpHash}", userOpHash);

        var client = _httpClientFactory.CreateClient(BUNDLER_HTTP_CLIENT);

        var request = new
        {
            jsonrpc = "2.0",
            id = 1,
            method = "eth_getUserOperationReceipt",
            @params = new[] { userOpHash }
        };

        var response = await client.PostAsJsonAsync("", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonRpcResponse<UserOperationReceiptResponse>>(cancellationToken);

        if (result?.Result == null)
        {
            _logger.LogInformation("No receipt found for UserOpHash {UserOpHash}", userOpHash);
            return null;
        }

        var receipt = new UserOperationReceipt
        {
            UserOpHash = userOpHash,
            TransactionHash = result.Result.TransactionHash,
            EntryPoint = result.Result.EntryPoint,
            Sender = result.Result.Sender,
            Nonce = result.Result.Nonce,
            Success = result.Result.Success,
            ActualGasCost = ConvertFromWei(result.Result.ActualGasCost),
            ActualGasUsed = ConvertFromWei(result.Result.ActualGasUsed),
            BlockNumber = Convert.ToInt64(result.Result.BlockNumber, 16),
            BlockTimestamp = null // Could fetch from block if needed
        };

        _logger.LogInformation("UserOperation receipt fetched. TxHash: {TxHash}, Success: {Success}",
            receipt.TransactionHash, receipt.Success);

        return receipt;
    }

    public async Task<GasEstimate> EstimateGasAsync(UserOperationDto userOp, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Estimating gas for UserOperation");

        var client = _httpClientFactory.CreateClient(BUNDLER_HTTP_CLIENT);

        var request = new
        {
            jsonrpc = "2.0",
            id = 1,
            method = "eth_estimateUserOperationGas",
            @params = new object[]
            {
                userOp,
                ENTRY_POINT_ADDRESS
            }
        };

        try
        {
            var response = await client.PostAsJsonAsync("", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonRpcResponse<GasEstimateResponse>>(cancellationToken);

            if (result?.Result != null)
            {
                return new GasEstimate
                {
                    CallGasLimit = result.Result.CallGasLimit,
                    VerificationGasLimit = result.Result.VerificationGasLimit,
                    PreVerificationGas = result.Result.PreVerificationGas,
                    MaxFeePerGas = result.Result.MaxFeePerGas ?? "0x0",
                    MaxPriorityFeePerGas = result.Result.MaxPriorityFeePerGas ?? "0x0"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Gas estimation failed, using default values");
        }

        // Fallback to default gas values
        return new GasEstimate
        {
            CallGasLimit = "0x55730", // ~350k
            VerificationGasLimit = "0x186A0", // ~100k
            PreVerificationGas = "0xC350", // ~50k
            MaxFeePerGas = "0x59682F00", // 1.5 gwei
            MaxPriorityFeePerGas = "0x59682F00" // 1.5 gwei
        };
    }

    #region Helper Methods

    private string EncodeERC20Transfer(string tokenAddress, string toAddress, string amountInWei)
    {
        // ERC-20 transfer function signature: transfer(address,uint256)
        var functionSignature = "0xa9059cbb"; // Keccak256("transfer(address,uint256)").Substring(0, 10)

        // Pad addresses to 32 bytes (remove 0x prefix, pad left with zeros)
        var paddedTo = toAddress.Replace("0x", "").PadLeft(64, '0');

        // Pad amount to 32 bytes
        var amountHex = BigInteger.Parse(amountInWei).ToString("X").PadLeft(64, '0');

        // Construct execute call to Smart Account
        // execute(address target, uint256 value, bytes calldata data)
        var executeSignature = "0xb61d27f6"; // Keccak256("execute(address,uint256,bytes)")

        var paddedToken = tokenAddress.Replace("0x", "").PadLeft(64, '0');
        var value = "0".PadLeft(64, '0');

        // Encode the ERC-20 transfer data
        var transferData = functionSignature + paddedTo + amountHex;
        var dataOffset = "60".PadLeft(64, '0'); // Offset to data (3 * 32 = 96 bytes = 0x60)
        var dataLength = ((transferData.Length - 2) / 2).ToString("X").PadLeft(64, '0'); // Length in bytes

        return executeSignature + paddedToken + value + dataOffset + dataLength + transferData.Replace("0x", "");
    }

    private string ConvertToWei(decimal amount, int decimals)
    {
        var multiplier = BigInteger.Pow(10, decimals);
        var weiAmount = (BigInteger)(amount * (decimal)multiplier);
        return weiAmount.ToString();
    }

    private decimal ConvertFromWei(string weiAmount, int decimals = 18)
    {
        if (string.IsNullOrEmpty(weiAmount) || weiAmount == "0x0")
            return 0;

        var wei = weiAmount.StartsWith("0x")
            ? BigInteger.Parse(weiAmount.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber)
            : BigInteger.Parse(weiAmount);

        var divisor = BigInteger.Pow(10, decimals);
        return (decimal)wei / (decimal)divisor;
    }

    private Task<string> GetNonceAsync(string address, CancellationToken cancellationToken)
    {
        // In production, query the EntryPoint contract for the nonce
        // For now, return 0 (will be managed by Circle SDK)
        return Task.FromResult("0x0");
    }

    #endregion

    #region JSON-RPC Response Models

    private class JsonRpcResponse<T>
    {
        public string? JsonRpc { get; set; }
        public int Id { get; set; }
        public T? Result { get; set; }
        public JsonRpcError? Error { get; set; }
    }

    private class JsonRpcError
    {
        public int Code { get; set; }
        public string? Message { get; set; }
    }

    private class UserOperationReceiptResponse
    {
        public string TransactionHash { get; set; } = string.Empty;
        public string EntryPoint { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string Nonce { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ActualGasCost { get; set; } = string.Empty;
        public string ActualGasUsed { get; set; } = string.Empty;
        public string BlockNumber { get; set; } = string.Empty;
    }

    private class GasEstimateResponse
    {
        public string CallGasLimit { get; set; } = string.Empty;
        public string VerificationGasLimit { get; set; } = string.Empty;
        public string PreVerificationGas { get; set; } = string.Empty;
        public string? MaxFeePerGas { get; set; }
        public string? MaxPriorityFeePerGas { get; set; }
    }

    #endregion
}
