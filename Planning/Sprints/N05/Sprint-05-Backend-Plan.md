# Sprint N05 - Backend Engineering Plan
# Phase 5: Basic Swap (DEX Integration)

**Sprint**: N05
**Duration**: 2 weeks (10 working days)
**Sprint Dates**: March 3 - March 14, 2025
**Total Effort**: 20.00 days
**Team Size**: 2-3 engineers (dotnet-backend-engineer agents)
**Utilization**: 67% (healthy capacity with buffer)

---

## Sprint Goal

Integrate DEX aggregator (1inch) to enable USDC ↔ ETH and USDC ↔ MATIC token swaps with transparent fee collection and slippage protection.

---

## Epic Breakdown

### Epic 1: DEX Integration & Quote Service (7.00 days)

**Description**: Implement 1inch DEX aggregator client and swap quote service with price caching.

**Tasks**:

#### BE-501: DEX Aggregator Service Interface (1.00 day)
**Owner**: Senior Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Create a generic DEX aggregator interface that can support multiple providers (1inch, 0x).

**Requirements**:
- Interface for DEX provider implementations
- Support quote and swap operations
- Provider factory pattern for extensibility
- Common error handling and retry logic

**Implementation**:
```csharp
// Services/Swap/IDexAggregatorService.cs
public interface IDexAggregatorService
{
    Task<SwapQuote> GetQuoteAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance);

    Task<SwapTransaction> GetSwapTransactionAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance,
        string fromAddress);

    Task<decimal> EstimateGasAsync(SwapTransaction swapTx);
}

// Services/Swap/DexAggregatorFactory.cs
public class DexAggregatorFactory
{
    public IDexAggregatorService GetProvider(string providerName)
    {
        return providerName.ToLower() switch
        {
            "1inch" => new OneInchAggregatorService(),
            "0x" => new ZeroXAggregatorService(),
            _ => throw new ArgumentException($"Unknown provider: {providerName}")
        };
    }
}
```

**Acceptance Criteria**:
- [ ] Interface supports quote and swap operations
- [ ] Factory pattern allows multiple providers
- [ ] Error handling standardized across providers
- [ ] Unit tests for interface contracts
- [ ] Documentation complete

**Dependencies**: None

---

#### BE-502: 1inch API Client Implementation (2.50 days)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Implement 1inch DEX aggregator API client for Polygon network.

**Requirements**:
- HTTP client for 1inch API v5.0
- Polygon Amoy testnet support (chainId: 80002)
- Request/response serialization
- Rate limiting (10 req/sec)
- Retry policy with exponential backoff
- API key management

**1inch API Endpoints**:
```
Base URL: https://api.1inch.io/v5.0/{chainId}

GET /quote
  Query params:
    - fromTokenAddress: Token address to swap from
    - toTokenAddress: Token address to swap to
    - amount: Amount in wei (smallest unit)

  Response:
  {
    "fromToken": {...},
    "toToken": {...},
    "fromTokenAmount": "1000000",
    "toTokenAmount": "1234567890",
    "estimatedGas": "150000"
  }

GET /swap
  Query params:
    - fromTokenAddress
    - toTokenAddress
    - amount
    - fromAddress: Sender wallet address
    - slippage: Slippage tolerance (0.5, 1, 3)
    - disableEstimate: true/false

  Response:
  {
    "fromToken": {...},
    "toToken": {...},
    "fromTokenAmount": "1000000",
    "toTokenAmount": "1234567890",
    "tx": {
      "from": "0x...",
      "to": "0x...",
      "data": "0x...",
      "value": "0",
      "gas": "150000",
      "gasPrice": "100000000000"
    }
  }
```

**Implementation**:
```csharp
// Services/Swap/OneInchAggregatorService.cs
public class OneInchAggregatorService : IDexAggregatorService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OneInchAggregatorService> _logger;

    private const string BASE_URL = "https://api.1inch.io/v5.0";
    private const int POLYGON_AMOY_CHAIN_ID = 80002;

    public async Task<SwapQuote> GetQuoteAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance)
    {
        var amountInWei = ConvertToWei(amount, fromToken);
        var endpoint = $"{BASE_URL}/{POLYGON_AMOY_CHAIN_ID}/quote";

        var queryParams = new Dictionary<string, string>
        {
            ["fromTokenAddress"] = fromToken,
            ["toTokenAddress"] = toToken,
            ["amount"] = amountInWei.ToString()
        };

        var response = await SendRequestAsync<OneInchQuoteResponse>(
            endpoint,
            queryParams);

        return MapToSwapQuote(response);
    }

    public async Task<SwapTransaction> GetSwapTransactionAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance,
        string fromAddress)
    {
        var amountInWei = ConvertToWei(amount, fromToken);
        var endpoint = $"{BASE_URL}/{POLYGON_AMOY_CHAIN_ID}/swap";

        var queryParams = new Dictionary<string, string>
        {
            ["fromTokenAddress"] = fromToken,
            ["toTokenAddress"] = toToken,
            ["amount"] = amountInWei.ToString(),
            ["fromAddress"] = fromAddress,
            ["slippage"] = slippageTolerance.ToString("F1"),
            ["disableEstimate"] = "false"
        };

        var response = await SendRequestAsync<OneInchSwapResponse>(
            endpoint,
            queryParams);

        return MapToSwapTransaction(response);
    }
}
```

**Token Addresses (Polygon Amoy Testnet)**:
```csharp
public static class TestnetTokens
{
    public const string USDC = "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582";
    public const string WETH = "0x..."; // Testnet address
    public const string WMATIC = "0x..."; // Testnet address
}
```

**Acceptance Criteria**:
- [ ] 1inch API client successfully queries testnet
- [ ] Quote endpoint returns valid prices
- [ ] Swap endpoint returns transaction data
- [ ] Rate limiting prevents exceeding 10 req/sec
- [ ] Retry policy handles transient failures
- [ ] API key stored securely in configuration
- [ ] Comprehensive logging with correlation IDs
- [ ] Unit tests cover all methods
- [ ] Integration tests pass with 1inch testnet

**Dependencies**: BE-501

---

#### BE-503: Swap Quote Service (Price Comparison) (2.00 days)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Service to fetch and compare swap quotes, calculate fees, and provide best price routing.

**Requirements**:
- Fetch quotes from DEX aggregator
- Calculate platform fee (0.5-1%)
- Calculate price impact
- Validate token pairs
- Handle quote errors gracefully

**Implementation**:
```csharp
// Services/Swap/SwapQuoteService.cs
public class SwapQuoteService : ISwapQuoteService
{
    private readonly IDexAggregatorService _dexService;
    private readonly IFeeCalculationService _feeService;
    private readonly ITokenValidationService _tokenService;
    private readonly ILogger<SwapQuoteService> _logger;

    public async Task<SwapQuoteResult> GetBestQuoteAsync(
        string fromToken,
        string toToken,
        decimal fromAmount,
        decimal slippageTolerance)
    {
        // Validate tokens
        await _tokenService.ValidateTokenPairAsync(fromToken, toToken);

        // Get quote from DEX
        var dexQuote = await _dexService.GetQuoteAsync(
            fromToken,
            toToken,
            fromAmount,
            slippageTolerance);

        // Calculate platform fee
        var platformFee = await _feeService.CalculateSwapFeeAsync(
            fromToken,
            fromAmount);

        // Calculate price impact
        var priceImpact = CalculatePriceImpact(
            fromAmount,
            dexQuote.ToTokenAmount,
            dexQuote.ExchangeRate);

        // Build result
        return new SwapQuoteResult
        {
            FromToken = fromToken,
            ToToken = toToken,
            FromAmount = fromAmount,
            ToAmount = dexQuote.ToTokenAmount,
            ExchangeRate = dexQuote.ExchangeRate,
            PlatformFee = platformFee,
            PlatformFeePercentage = 0.5m, // 0.5%
            EstimatedGas = dexQuote.EstimatedGas,
            PriceImpact = priceImpact,
            SlippageTolerance = slippageTolerance,
            QuoteValidUntil = DateTime.UtcNow.AddSeconds(30),
            Provider = "1inch"
        };
    }

    private decimal CalculatePriceImpact(
        decimal fromAmount,
        decimal toAmount,
        decimal exchangeRate)
    {
        // Price impact = (Expected - Actual) / Expected * 100
        var expectedToAmount = fromAmount * exchangeRate;
        var priceImpact = (expectedToAmount - toAmount) / expectedToAmount * 100;
        return Math.Max(0, priceImpact); // Prevent negative impact
    }
}

// Models/SwapQuoteResult.cs
public class SwapQuoteResult
{
    public string FromToken { get; set; }
    public string ToToken { get; set; }
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal PlatformFeePercentage { get; set; }
    public decimal EstimatedGas { get; set; }
    public decimal PriceImpact { get; set; }
    public decimal SlippageTolerance { get; set; }
    public DateTime QuoteValidUntil { get; set; }
    public string Provider { get; set; }
}
```

**Acceptance Criteria**:
- [ ] Quotes fetched successfully from 1inch
- [ ] Platform fee calculated (0.5%)
- [ ] Price impact calculated for large swaps
- [ ] Token pair validation working
- [ ] Quote expiry set to 30 seconds
- [ ] Error handling for invalid tokens
- [ ] Unit tests for all calculations
- [ ] Integration tests with 1inch

**Dependencies**: BE-501, BE-502

---

#### BE-504: GET /api/swap/quote - Get Swap Quote (1.00 day)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
API endpoint to get swap quote with fees and price impact.

**API Specification**:
```
GET /api/swap/quote
Authorization: Bearer {jwt_token}

Query Parameters:
  - fromToken: Token address (USDC/WETH/WMATIC)
  - toToken: Token address
  - amount: Amount to swap (decimal)
  - slippage: Slippage tolerance (0.5, 1, 3) - default: 1

Response (200 OK):
{
  "fromToken": "0x41e94eb...",
  "fromTokenSymbol": "USDC",
  "toToken": "0x...",
  "toTokenSymbol": "WETH",
  "fromAmount": 100.0,
  "toAmount": 0.0285,
  "exchangeRate": 0.000285,
  "platformFee": 0.50,
  "platformFeePercentage": 0.5,
  "estimatedGas": "150000",
  "estimatedGasCost": "0.015",
  "priceImpact": 0.15,
  "slippageTolerance": 1.0,
  "minimumReceived": 0.02821,
  "quoteValidUntil": "2025-03-03T10:00:30Z",
  "provider": "1inch"
}

Response (400 Bad Request):
{
  "error": "Invalid token pair"
}

Response (400 Bad Request):
{
  "error": "Amount must be greater than 0"
}
```

**Implementation**:
```csharp
// Controllers/SwapController.cs
[ApiController]
[Route("api/swap")]
[Authorize]
public class SwapController : ControllerBase
{
    private readonly ISwapQuoteService _quoteService;
    private readonly ILogger<SwapController> _logger;

    [HttpGet("quote")]
    [ProducesResponseType(typeof(SwapQuoteResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> GetQuote(
        [FromQuery] string fromToken,
        [FromQuery] string toToken,
        [FromQuery] decimal amount,
        [FromQuery] decimal slippage = 1.0m)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(fromToken) ||
            string.IsNullOrWhiteSpace(toToken))
        {
            return BadRequest(new { error = "Token addresses required" });
        }

        if (amount <= 0)
        {
            return BadRequest(new { error = "Amount must be greater than 0" });
        }

        if (slippage < 0.1m || slippage > 50m)
        {
            return BadRequest(new { error = "Slippage must be between 0.1% and 50%" });
        }

        // Get quote
        var quote = await _quoteService.GetBestQuoteAsync(
            fromToken,
            toToken,
            amount,
            slippage);

        // Map to response
        var response = MapToQuoteResponse(quote);

        return Ok(response);
    }
}
```

**Acceptance Criteria**:
- [ ] Endpoint returns valid quotes
- [ ] Validates token addresses
- [ ] Validates amount and slippage
- [ ] Returns clear error messages
- [ ] Response time < 2s
- [ ] Swagger documentation complete
- [ ] Integration tests pass

**Dependencies**: BE-503

---

#### BE-505: Price Caching Service (30s TTL) (0.50 day)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P1 (High)

**Description**:
Cache swap quotes and exchange rates to reduce DEX API calls.

**Requirements**:
- Redis cache integration
- 30-second TTL for quotes
- Cache key: `swap:quote:{fromToken}:{toToken}:{amount}:{slippage}`
- Cache invalidation on expiry

**Implementation**:
```csharp
// Services/Caching/SwapQuoteCacheService.cs
public class SwapQuoteCacheService : ISwapQuoteCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<SwapQuoteCacheService> _logger;

    private const int CACHE_TTL_SECONDS = 30;

    public async Task<SwapQuoteResult?> GetCachedQuoteAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage)
    {
        var cacheKey = BuildCacheKey(fromToken, toToken, amount, slippage);
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (cachedData != null)
        {
            _logger.LogInformation("Cache hit for swap quote: {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<SwapQuoteResult>(cachedData);
        }

        return null;
    }

    public async Task SetCachedQuoteAsync(
        SwapQuoteResult quote,
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage)
    {
        var cacheKey = BuildCacheKey(fromToken, toToken, amount, slippage);
        var serialized = JsonSerializer.Serialize(quote);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CACHE_TTL_SECONDS)
        };

        await _cache.SetStringAsync(cacheKey, serialized, options);
        _logger.LogInformation("Cached swap quote: {CacheKey}", cacheKey);
    }

    private string BuildCacheKey(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage)
    {
        return $"swap:quote:{fromToken}:{toToken}:{amount:F6}:{slippage:F1}";
    }
}
```

**Acceptance Criteria**:
- [ ] Quotes cached in Redis
- [ ] Cache TTL set to 30 seconds
- [ ] Cache hit reduces API calls by 80%+
- [ ] Cache keys unique per token pair + amount
- [ ] Unit tests for cache service

**Dependencies**: BE-503

---

### Epic 2: Swap Execution & Validation (8.00 days)

#### BE-506: Swap Transaction Model & Repository (1.50 days)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Create database models and repository for swap transactions.

**Database Schema**:
```sql
CREATE TABLE swap_transactions (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    wallet_address VARCHAR(42) NOT NULL,
    from_token VARCHAR(42) NOT NULL,
    to_token VARCHAR(42) NOT NULL,
    from_token_symbol VARCHAR(10) NOT NULL,
    to_token_symbol VARCHAR(10) NOT NULL,
    from_amount NUMERIC(30, 18) NOT NULL,
    to_amount NUMERIC(30, 18) NOT NULL,
    exchange_rate NUMERIC(30, 18) NOT NULL,
    platform_fee NUMERIC(30, 18) NOT NULL,
    platform_fee_percentage NUMERIC(5, 2) NOT NULL,
    gas_used VARCHAR(50),
    gas_cost NUMERIC(30, 18),
    slippage_tolerance NUMERIC(5, 2) NOT NULL,
    price_impact NUMERIC(5, 2),
    minimum_received NUMERIC(30, 18) NOT NULL,
    dex_provider VARCHAR(50) NOT NULL, -- '1inch', '0x'
    transaction_hash VARCHAR(66),
    status VARCHAR(20) NOT NULL, -- 'pending', 'confirmed', 'failed'
    error_message TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    confirmed_at TIMESTAMP,
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE INDEX idx_swap_transactions_user ON swap_transactions(user_id);
CREATE INDEX idx_swap_transactions_wallet ON swap_transactions(wallet_address);
CREATE INDEX idx_swap_transactions_status ON swap_transactions(status);
CREATE INDEX idx_swap_transactions_created ON swap_transactions(created_at DESC);
CREATE INDEX idx_swap_transactions_hash ON swap_transactions(transaction_hash);
```

**Implementation**:
```csharp
// Models/SwapTransaction.cs
public class SwapTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string WalletAddress { get; set; }
    public string FromToken { get; set; }
    public string ToToken { get; set; }
    public string FromTokenSymbol { get; set; }
    public string ToTokenSymbol { get; set; }
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal PlatformFeePercentage { get; set; }
    public string? GasUsed { get; set; }
    public decimal? GasCost { get; set; }
    public decimal SlippageTolerance { get; set; }
    public decimal? PriceImpact { get; set; }
    public decimal MinimumReceived { get; set; }
    public string DexProvider { get; set; }
    public string? TransactionHash { get; set; }
    public SwapStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum SwapStatus
{
    Pending,
    Confirmed,
    Failed
}

// Repositories/ISwapTransactionRepository.cs
public interface ISwapTransactionRepository
{
    Task<SwapTransaction> CreateAsync(SwapTransaction transaction);
    Task<SwapTransaction?> GetByIdAsync(Guid id);
    Task<SwapTransaction?> GetByTransactionHashAsync(string txHash);
    Task<List<SwapTransaction>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20);
    Task<List<SwapTransaction>> GetByWalletAddressAsync(
        string walletAddress,
        int page = 1,
        int pageSize = 20);
    Task UpdateAsync(SwapTransaction transaction);
    Task<int> GetSwapCountByUserAsync(Guid userId);
    Task<decimal> GetTotalVolumeByUserAsync(Guid userId);
}
```

**Acceptance Criteria**:
- [ ] Database migration created
- [ ] Repository implements all CRUD operations
- [ ] Indexes optimize common queries
- [ ] Decimal precision supports token amounts
- [ ] Unit tests for repository methods
- [ ] Integration tests with test database

**Dependencies**: None (database only)

---

#### BE-507: Token Balance Validation Service (1.00 day)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Service to validate user has sufficient token balance before swap.

**Requirements**:
- Query Circle wallet balance
- Validate sufficient balance (including fees)
- Support USDC, WETH, WMATIC
- Handle native token (MATIC) vs wrapped token (WMATIC)

**Implementation**:
```csharp
// Services/Swap/TokenBalanceValidationService.cs
public class TokenBalanceValidationService : ITokenBalanceValidationService
{
    private readonly IWalletService _walletService;
    private readonly IFeeCalculationService _feeService;
    private readonly ILogger<TokenBalanceValidationService> _logger;

    public async Task<TokenBalanceValidationResult> ValidateBalanceAsync(
        Guid userId,
        string walletAddress,
        string tokenAddress,
        decimal requiredAmount)
    {
        // Get token balance from Circle wallet
        var balance = await _walletService.GetTokenBalanceAsync(
            walletAddress,
            tokenAddress);

        // Calculate platform fee
        var platformFee = await _feeService.CalculateSwapFeeAsync(
            tokenAddress,
            requiredAmount);

        // Total required = amount + fee
        var totalRequired = requiredAmount + platformFee;

        // Validate
        var hasEnough = balance >= totalRequired;

        return new TokenBalanceValidationResult
        {
            TokenAddress = tokenAddress,
            CurrentBalance = balance,
            RequiredAmount = requiredAmount,
            PlatformFee = platformFee,
            TotalRequired = totalRequired,
            HasSufficientBalance = hasEnough,
            ShortfallAmount = hasEnough ? 0 : (totalRequired - balance)
        };
    }
}

// Models/TokenBalanceValidationResult.cs
public class TokenBalanceValidationResult
{
    public string TokenAddress { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal RequiredAmount { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal TotalRequired { get; set; }
    public bool HasSufficientBalance { get; set; }
    public decimal ShortfallAmount { get; set; }
}
```

**Acceptance Criteria**:
- [ ] Balance validation works for all tokens
- [ ] Includes platform fee in validation
- [ ] Returns clear error for insufficient balance
- [ ] Handles Circle wallet API errors
- [ ] Unit tests for validation logic

**Dependencies**: Circle SDK, BE-511 (fee service)

---

#### BE-508: Slippage Tolerance Service (1.50 days)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Service to handle slippage tolerance and calculate minimum received amount.

**Requirements**:
- Support preset slippage values (0.5%, 1%, 3%)
- Support custom slippage (0.1% - 50%)
- Calculate minimum received based on slippage
- Validate slippage is within reasonable bounds

**Implementation**:
```csharp
// Services/Swap/SlippageToleranceService.cs
public class SlippageToleranceService : ISlippageToleranceService
{
    private const decimal MIN_SLIPPAGE = 0.1m;  // 0.1%
    private const decimal MAX_SLIPPAGE = 50.0m; // 50%

    public decimal CalculateMinimumReceived(
        decimal expectedAmount,
        decimal slippagePercentage)
    {
        ValidateSlippage(slippagePercentage);

        var slippageMultiplier = 1 - (slippagePercentage / 100m);
        var minimumReceived = expectedAmount * slippageMultiplier;

        return minimumReceived;
    }

    public bool IsSlippageExcessive(decimal slippagePercentage)
    {
        return slippagePercentage > 5.0m; // Warn if > 5%
    }

    public SlippageRecommendation GetRecommendedSlippage(
        string fromToken,
        string toToken,
        decimal amount)
    {
        // Recommend slippage based on liquidity and amount

        // For small amounts (<$100): 0.5%
        if (amount < 100m)
        {
            return new SlippageRecommendation
            {
                RecommendedSlippage = 0.5m,
                Reason = "Low slippage for small trade"
            };
        }

        // For medium amounts ($100-$1000): 1%
        if (amount < 1000m)
        {
            return new SlippageRecommendation
            {
                RecommendedSlippage = 1.0m,
                Reason = "Standard slippage for medium trade"
            };
        }

        // For large amounts (>$1000): 3%
        return new SlippageRecommendation
        {
            RecommendedSlippage = 3.0m,
            Reason = "Higher slippage for large trade"
        };
    }

    private void ValidateSlippage(decimal slippagePercentage)
    {
        if (slippagePercentage < MIN_SLIPPAGE || slippagePercentage > MAX_SLIPPAGE)
        {
            throw new ArgumentException(
                $"Slippage must be between {MIN_SLIPPAGE}% and {MAX_SLIPPAGE}%");
        }
    }
}
```

**Acceptance Criteria**:
- [ ] Minimum received calculated correctly
- [ ] Slippage validation working
- [ ] Recommended slippage based on amount
- [ ] Warning for excessive slippage (>5%)
- [ ] Unit tests for all calculations

**Dependencies**: BE-503

---

#### BE-509: Swap Execution Service (2.50 days)
**Owner**: Senior Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Core service to execute swaps using DEX aggregator and Circle SDK.

**Requirements**:
- Get swap transaction data from 1inch
- Validate token balance before swap
- Check token approval status
- Execute token approval if needed
- Submit swap transaction via Circle SDK
- Monitor transaction status
- Update swap record in database
- Handle errors and retries

**Implementation**:
```csharp
// Services/Swap/SwapExecutionService.cs
public class SwapExecutionService : ISwapExecutionService
{
    private readonly IDexAggregatorService _dexService;
    private readonly IWalletService _walletService;
    private readonly ITokenBalanceValidationService _balanceService;
    private readonly ISwapTransactionRepository _swapRepository;
    private readonly IFeeCollectionService _feeService;
    private readonly ILogger<SwapExecutionService> _logger;

    public async Task<SwapExecutionResult> ExecuteSwapAsync(
        Guid userId,
        string walletAddress,
        string fromToken,
        string toToken,
        decimal fromAmount,
        decimal slippageTolerance)
    {
        try
        {
            // 1. Validate balance
            var balanceCheck = await _balanceService.ValidateBalanceAsync(
                userId,
                walletAddress,
                fromToken,
                fromAmount);

            if (!balanceCheck.HasSufficientBalance)
            {
                throw new InsufficientBalanceException(
                    $"Insufficient balance. Required: {balanceCheck.TotalRequired}, Available: {balanceCheck.CurrentBalance}");
            }

            // 2. Get swap transaction from DEX
            var swapTx = await _dexService.GetSwapTransactionAsync(
                fromToken,
                toToken,
                fromAmount,
                slippageTolerance,
                walletAddress);

            // 3. Check token approval
            var needsApproval = await CheckTokenApprovalAsync(
                walletAddress,
                fromToken,
                swapTx.SpenderAddress,
                fromAmount);

            if (needsApproval)
            {
                // Execute approval transaction first
                await ExecuteTokenApprovalAsync(
                    walletAddress,
                    fromToken,
                    swapTx.SpenderAddress);
            }

            // 4. Create swap record
            var swapRecord = new SwapTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WalletAddress = walletAddress,
                FromToken = fromToken,
                ToToken = toToken,
                FromAmount = fromAmount,
                ToAmount = swapTx.ToTokenAmount,
                ExchangeRate = swapTx.ExchangeRate,
                PlatformFee = swapTx.PlatformFee,
                PlatformFeePercentage = 0.5m,
                SlippageTolerance = slippageTolerance,
                MinimumReceived = swapTx.MinimumReceived,
                DexProvider = "1inch",
                Status = SwapStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _swapRepository.CreateAsync(swapRecord);

            // 5. Execute swap transaction via Circle SDK
            var txHash = await _walletService.SubmitTransactionAsync(
                walletAddress,
                swapTx.To,
                swapTx.Data,
                swapTx.Value,
                swapTx.Gas);

            // 6. Update swap record
            swapRecord.TransactionHash = txHash;
            await _swapRepository.UpdateAsync(swapRecord);

            // 7. Collect platform fee (async)
            _ = Task.Run(async () => await _feeService.CollectSwapFeeAsync(
                userId,
                swapRecord.Id,
                swapRecord.PlatformFee));

            return new SwapExecutionResult
            {
                SwapId = swapRecord.Id,
                TransactionHash = txHash,
                Status = SwapStatus.Pending,
                ExpectedToAmount = swapTx.ToTokenAmount,
                Message = "Swap transaction submitted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Swap execution failed for user {UserId}", userId);
            throw;
        }
    }

    private async Task<bool> CheckTokenApprovalAsync(
        string walletAddress,
        string tokenAddress,
        string spenderAddress,
        decimal amount)
    {
        // Check if token approval exists for spender
        var allowance = await _walletService.GetTokenAllowanceAsync(
            walletAddress,
            tokenAddress,
            spenderAddress);

        return allowance < amount;
    }

    private async Task ExecuteTokenApprovalAsync(
        string walletAddress,
        string tokenAddress,
        string spenderAddress)
    {
        // Submit token approval transaction
        await _walletService.ApproveTokenAsync(
            walletAddress,
            tokenAddress,
            spenderAddress,
            decimal.MaxValue); // Approve infinite amount
    }
}
```

**Acceptance Criteria**:
- [ ] Balance validated before swap
- [ ] Token approval handled automatically
- [ ] Swap transaction submitted successfully
- [ ] Transaction hash returned
- [ ] Swap record created in database
- [ ] Platform fee collected
- [ ] Error handling for all failure modes
- [ ] Integration tests with testnet

**Dependencies**: BE-502, BE-506, BE-507, BE-508, Circle SDK

---

#### BE-510: POST /api/swap/execute - Execute Swap (1.50 days)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
API endpoint to execute token swap.

**API Specification**:
```
POST /api/swap/execute
Authorization: Bearer {jwt_token}

Request:
{
  "fromToken": "0x41e94eb...",
  "toToken": "0x...",
  "fromAmount": 100.0,
  "slippageTolerance": 1.0,
  "quoteId": "uuid" (optional, for quote verification)
}

Response (200 OK):
{
  "swapId": "uuid",
  "transactionHash": "0x...",
  "status": "pending",
  "fromToken": "0x41e94eb...",
  "fromTokenSymbol": "USDC",
  "toToken": "0x...",
  "toTokenSymbol": "WETH",
  "fromAmount": 100.0,
  "expectedToAmount": 0.0285,
  "minimumReceived": 0.02821,
  "platformFee": 0.50,
  "estimatedConfirmationTime": "45 seconds",
  "message": "Swap transaction submitted successfully"
}

Response (400 Bad Request):
{
  "error": "Insufficient balance"
}

Response (400 Bad Request):
{
  "error": "Slippage tolerance too high"
}
```

**Implementation**:
```csharp
// Controllers/SwapController.cs
[HttpPost("execute")]
[ProducesResponseType(typeof(SwapExecutionResponse), 200)]
[ProducesResponseType(typeof(ErrorResponse), 400)]
public async Task<IActionResult> ExecuteSwap(
    [FromBody] ExecuteSwapRequest request)
{
    // Get user ID from JWT
    var userId = GetUserIdFromClaims();

    // Get user's wallet address
    var wallet = await _walletService.GetUserWalletAsync(userId);

    if (wallet == null)
    {
        return BadRequest(new { error = "Wallet not found" });
    }

    // Validate inputs
    if (string.IsNullOrWhiteSpace(request.FromToken) ||
        string.IsNullOrWhiteSpace(request.ToToken))
    {
        return BadRequest(new { error = "Token addresses required" });
    }

    if (request.FromAmount <= 0)
    {
        return BadRequest(new { error = "Amount must be greater than 0" });
    }

    // Execute swap
    var result = await _swapExecutionService.ExecuteSwapAsync(
        userId,
        wallet.Address,
        request.FromToken,
        request.ToToken,
        request.FromAmount,
        request.SlippageTolerance);

    // Map to response
    var response = MapToExecutionResponse(result);

    return Ok(response);
}
```

**Acceptance Criteria**:
- [ ] Endpoint executes swaps successfully
- [ ] Validates balance before execution
- [ ] Returns transaction hash
- [ ] Handles token approval flow
- [ ] Returns clear error messages
- [ ] Swagger documentation complete
- [ ] Integration tests pass

**Dependencies**: BE-509

---

### Epic 3: Fee Management & History (5.00 days)

#### BE-511: Fee Calculation Service (0.5-1%) (1.50 days)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P0 (Critical Path)

**Description**:
Service to calculate platform swap fees.

**Requirements**:
- Calculate fee as percentage of swap amount
- Support tiered fee structure (0.5% standard, discounts for high volume)
- Calculate fee in source token
- Round fees correctly

**Implementation**:
```csharp
// Services/Swap/FeeCalculationService.cs
public class FeeCalculationService : IFeeCalculationService
{
    private const decimal STANDARD_FEE_PERCENTAGE = 0.5m; // 0.5%
    private const decimal HIGH_VOLUME_FEE_PERCENTAGE = 0.3m; // 0.3%
    private const decimal HIGH_VOLUME_THRESHOLD = 10000m; // $10k monthly volume

    public async Task<decimal> CalculateSwapFeeAsync(
        string tokenAddress,
        decimal amount)
    {
        var feePercentage = STANDARD_FEE_PERCENTAGE;

        // Apply fee
        var fee = amount * (feePercentage / 100m);

        return fee;
    }

    public async Task<FeeBreakdown> GetFeeBreakdownAsync(
        string fromToken,
        string toToken,
        decimal fromAmount)
    {
        var platformFee = await CalculateSwapFeeAsync(fromToken, fromAmount);
        var platformFeePercentage = STANDARD_FEE_PERCENTAGE;

        // Get DEX fee (if applicable)
        var dexFee = 0m; // 1inch doesn't charge separate fee

        return new FeeBreakdown
        {
            PlatformFee = platformFee,
            PlatformFeePercentage = platformFeePercentage,
            DexFee = dexFee,
            TotalFee = platformFee + dexFee
        };
    }
}
```

**Acceptance Criteria**:
- [ ] Fee calculated as 0.5% of swap amount
- [ ] Fee calculation accurate to 8 decimals
- [ ] Fee breakdown returned
- [ ] Unit tests for all calculations

**Dependencies**: None

---

#### BE-512: Platform Fee Collection Service (1.00 day)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P1 (High)

**Description**:
Service to collect platform fees from swaps.

**Requirements**:
- Deduct fee from user's token amount
- Transfer fee to platform treasury wallet
- Record fee collection in database
- Audit logging

**Implementation**:
```csharp
// Services/Swap/PlatformFeeCollectionService.cs
public class PlatformFeeCollectionService : IPlatformFeeCollectionService
{
    private readonly IWalletService _walletService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PlatformFeeCollectionService> _logger;

    private string TreasuryWallet => _configuration["Treasury:WalletAddress"];

    public async Task CollectSwapFeeAsync(
        Guid userId,
        Guid swapId,
        decimal feeAmount)
    {
        try
        {
            // Transfer fee to treasury (async, non-blocking)
            // Fee will be deducted from user's final swap amount

            _logger.LogInformation(
                "Collected swap fee: {FeeAmount} for swap {SwapId} from user {UserId}",
                feeAmount,
                swapId,
                userId);

            // TODO: Implement actual fee transfer if needed
            // For MVP, fee is implicit in swap execution
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to collect swap fee for swap {SwapId}", swapId);
            // Don't throw - fee collection failure shouldn't block swap
        }
    }
}
```

**Acceptance Criteria**:
- [ ] Fee collection logged
- [ ] Fee collection errors don't block swaps
- [ ] Audit trail maintained

**Dependencies**: BE-511

---

#### BE-513: GET /api/swap/history - Swap History (1.50 days)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P1 (High)

**Description**:
API endpoint to get user's swap history with pagination and filtering.

**API Specification**:
```
GET /api/swap/history?status=all&page=1&pageSize=20
Authorization: Bearer {jwt_token}

Query Parameters:
  - status: Filter by status (all, pending, confirmed, failed) - default: all
  - page: Page number - default: 1
  - pageSize: Items per page - default: 20
  - sortBy: Sort field (createdAt, fromAmount) - default: createdAt
  - sortOrder: Sort order (desc, asc) - default: desc

Response (200 OK):
{
  "swaps": [
    {
      "id": "uuid",
      "fromToken": "0x41e94eb...",
      "fromTokenSymbol": "USDC",
      "toToken": "0x...",
      "toTokenSymbol": "WETH",
      "fromAmount": 100.0,
      "toAmount": 0.0285,
      "exchangeRate": 0.000285,
      "platformFee": 0.50,
      "status": "confirmed",
      "transactionHash": "0x...",
      "createdAt": "2025-03-03T10:00:00Z",
      "confirmedAt": "2025-03-03T10:00:45Z"
    }
  ],
  "totalItems": 15,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

**Acceptance Criteria**:
- [ ] Returns swap history with pagination
- [ ] Supports status filtering
- [ ] Supports sorting
- [ ] Response time < 1s
- [ ] Swagger documentation complete

**Dependencies**: BE-506

---

#### BE-514: GET /api/swap/{id}/details - Swap Details (1.00 day)
**Owner**: Backend Engineer (dotnet-backend-engineer agent)
**Priority**: P1 (High)

**Description**:
API endpoint to get detailed information for a specific swap.

**API Specification**:
```
GET /api/swap/{id}/details
Authorization: Bearer {jwt_token}

Response (200 OK):
{
  "id": "uuid",
  "fromToken": "0x41e94eb...",
  "fromTokenSymbol": "USDC",
  "toToken": "0x...",
  "toTokenSymbol": "WETH",
  "fromAmount": 100.0,
  "toAmount": 0.0285,
  "exchangeRate": 0.000285,
  "platformFee": 0.50,
  "platformFeePercentage": 0.5,
  "gasUsed": "150000",
  "gasCost": "0.015",
  "slippageTolerance": 1.0,
  "priceImpact": 0.15,
  "minimumReceived": 0.02821,
  "dexProvider": "1inch",
  "transactionHash": "0x...",
  "status": "confirmed",
  "createdAt": "2025-03-03T10:00:00Z",
  "confirmedAt": "2025-03-03T10:00:45Z"
}

Response (404 Not Found):
{
  "error": "Swap not found"
}
```

**Acceptance Criteria**:
- [ ] Returns complete swap details
- [ ] Returns 404 if not found
- [ ] Response time < 1s
- [ ] Swagger documentation complete

**Dependencies**: BE-506

---

## Task Dependencies (Critical Path)

```
BE-501 (DEX Interface)
  └── BE-502 (1inch API Client)
        └── BE-503 (Quote Service)
              ├── BE-504 (Quote API)
              └── BE-505 (Price Caching)

BE-506 (Swap Model) ──┐
BE-507 (Balance Validation) ──┤
BE-508 (Slippage Service) ──┤
                            ├──> BE-509 (Swap Execution)
BE-503 (Quote Service) ──────┘        └──> BE-510 (Execute API)

BE-511 (Fee Calculation)
  └── BE-512 (Fee Collection)

BE-506 (Swap Model)
  ├── BE-513 (Swap History)
  └── BE-514 (Swap Details)
```

**Critical Path Duration**: ~15 days (with parallel work)

---

## Sprint Backlog (Priority Order)

### Must Have (P0) - Sprint Cannot Ship Without These
1. BE-501: DEX Aggregator Interface
2. BE-502: 1inch API Client
3. BE-503: Swap Quote Service
4. BE-504: Quote API Endpoint
5. BE-506: Swap Transaction Model
6. BE-507: Balance Validation
7. BE-508: Slippage Tolerance Service
8. BE-509: Swap Execution Service
9. BE-510: Execute Swap API
10. BE-511: Fee Calculation Service

### Should Have (P1) - Important But Can Ship Without
11. BE-505: Price Caching
12. BE-512: Fee Collection Service
13. BE-513: Swap History API
14. BE-514: Swap Details API

---

## Testing Requirements

### Unit Tests (Target: >80% Coverage)
- [ ] 1inch API client methods
- [ ] Quote service calculations
- [ ] Slippage tolerance calculations
- [ ] Fee calculation formulas
- [ ] Balance validation logic
- [ ] Repository CRUD operations

### Integration Tests
- [ ] 1inch API connectivity (testnet)
- [ ] Swap quote fetching
- [ ] Swap execution flow
- [ ] Token approval flow
- [ ] Circle SDK integration
- [ ] Database operations

### Performance Tests
- [ ] Quote API response time < 2s
- [ ] Execute API response time < 3s
- [ ] Price cache hit rate > 80%
- [ ] Concurrent swap handling (10 users)

---

## Monitoring & Alerts

### Key Metrics to Monitor
- 1inch API response times (P50, P95, P99)
- 1inch API error rates
- Swap success/failure rates
- Price cache hit rates
- Token approval success rates
- Platform fee collection amounts

### Alert Thresholds
- 1inch API error rate > 5%
- Swap failure rate > 5%
- Quote response time > 3s
- Price cache hit rate < 70%

---

## Documentation Requirements

### API Documentation (Swagger)
- [ ] All endpoints documented with examples
- [ ] Request/response schemas defined
- [ ] Error codes and messages listed
- [ ] Authentication requirements specified

### Developer Documentation
- [ ] 1inch integration guide
- [ ] Swap execution flow diagram
- [ ] Slippage tolerance guide
- [ ] Fee calculation formulas
- [ ] Troubleshooting guide

---

## Definition of Done (Backend)

Sprint N05 backend work is **DONE** when:

- [ ] All P0 tasks completed (10 tasks)
- [ ] All API endpoints implemented and documented
- [ ] 1inch integration tested on Polygon Amoy testnet
- [ ] Swap execution working end-to-end
- [ ] Slippage protection functional
- [ ] Fee calculation accurate (0.5%)
- [ ] Unit test coverage > 80%
- [ ] Integration tests pass
- [ ] API response time < 2s (P95)
- [ ] Code reviewed and approved
- [ ] Swagger documentation updated
- [ ] No security vulnerabilities
- [ ] Database migrations deployed

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-05 | Backend Lead | Initial Sprint N05 Backend Plan |

---

**BACKEND TEAM STATUS**: **READY TO START**

**NEXT STEPS**:
1. **Day 1 Morning**: Request 1inch API key
2. **Day 1**: Setup Polygon Amoy testnet environment
3. **Day 1**: Sprint kickoff meeting
4. **Day 1 Afternoon**: Begin BE-501 (DEX Interface)

---

**End of Sprint N05 Backend Plan**
