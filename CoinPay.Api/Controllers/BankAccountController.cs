using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoinPay.Api.DTOs;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.BankAccount;
using CoinPay.Api.Services.Encryption;
using System.Security.Claims;

namespace CoinPay.Api.Controllers;

/// <summary>
/// Bank Account management endpoints
/// Handles CRUD operations for user bank accounts with encrypted sensitive data
/// </summary>
[ApiController]
[Route("api/bank-account")]
[Authorize]
public class BankAccountController : ControllerBase
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IBankAccountValidationService _validationService;
    private readonly ILogger<BankAccountController> _logger;

    public BankAccountController(
        IBankAccountRepository bankAccountRepository,
        IEncryptionService encryptionService,
        IBankAccountValidationService validationService,
        ILogger<BankAccountController> logger)
    {
        _bankAccountRepository = bankAccountRepository;
        _encryptionService = encryptionService;
        _validationService = validationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all bank accounts for the authenticated user
    /// </summary>
    /// <returns>List of bank accounts (with masked account numbers)</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BankAccountListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BankAccountListResponse>> GetBankAccounts()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("GetBankAccounts: User ID not found in token");
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        try
        {
            var bankAccounts = await _bankAccountRepository.GetAllByUserIdAsync(userId.Value);

            var response = new BankAccountListResponse
            {
                BankAccounts = bankAccounts.Select(MapToResponse).ToList(),
                Total = bankAccounts.Count()
            };

            _logger.LogInformation("GetBankAccounts: Retrieved {Count} bank accounts for user {UserId}", response.Total, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBankAccounts: Error retrieving bank accounts for user {UserId}", userId);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to retrieve bank accounts" } });
        }
    }

    /// <summary>
    /// Get bank account by ID
    /// </summary>
    /// <param name="id">Bank account ID</param>
    /// <returns>Bank account details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankAccountResponse>> GetBankAccountById(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        try
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(id);

            if (bankAccount == null)
            {
                _logger.LogWarning("GetBankAccountById: Bank account {BankAccountId} not found", id);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Bank account not found" } });
            }

            // Verify ownership
            if (bankAccount.UserId != userId.Value)
            {
                _logger.LogWarning("GetBankAccountById: User {UserId} attempted to access bank account {BankAccountId} owned by {OwnerId}", userId, id, bankAccount.UserId);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Bank account not found" } });
            }

            return Ok(MapToResponse(bankAccount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBankAccountById: Error retrieving bank account {BankAccountId}", id);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to retrieve bank account" } });
        }
    }

    /// <summary>
    /// Add new bank account
    /// </summary>
    /// <param name="request">Bank account details</param>
    /// <returns>Created bank account</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BankAccountResponse>> AddBankAccount([FromBody] AddBankAccountRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("AddBankAccount: User ID not found in token");
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        // Server-side validation
        var routingValidation = _validationService.ValidateRoutingNumber(request.RoutingNumber);
        if (!routingValidation.IsValid)
        {
            _logger.LogWarning("AddBankAccount: Invalid routing number for user {UserId}", userId);
            return BadRequest(new { error = new { code = "INVALID_ROUTING_NUMBER", message = routingValidation.ErrorMessage } });
        }

        var accountValidation = _validationService.ValidateAccountNumber(request.AccountNumber);
        if (!accountValidation.IsValid)
        {
            _logger.LogWarning("AddBankAccount: Invalid account number for user {UserId}", userId);
            return BadRequest(new { error = new { code = "INVALID_ACCOUNT_NUMBER", message = accountValidation.ErrorMessage } });
        }

        var nameValidation = _validationService.ValidateAccountHolderName(request.AccountHolderName);
        if (!nameValidation.IsValid)
        {
            _logger.LogWarning("AddBankAccount: Invalid account holder name for user {UserId}", userId);
            return BadRequest(new { error = new { code = "INVALID_ACCOUNT_HOLDER_NAME", message = nameValidation.ErrorMessage } });
        }

        // Check for duplicate account
        var isDuplicate = await _validationService.IsDuplicateAccountAsync(
            userId.Value,
            request.RoutingNumber,
            request.AccountNumber);

        if (isDuplicate)
        {
            _logger.LogWarning("AddBankAccount: Duplicate bank account detected for user {UserId}", userId);
            return BadRequest(new { error = new { code = "DUPLICATE_ACCOUNT", message = "This bank account is already registered" } });
        }

        try
        {
            // Encrypt sensitive data
            var encryptedRoutingNumber = BankAccountEncryptionHelper.EncryptRoutingNumber(
                request.RoutingNumber,
                _encryptionService);

            var encryptedAccountNumber = BankAccountEncryptionHelper.EncryptAccountNumber(
                request.AccountNumber,
                _encryptionService);

            var lastFourDigits = _validationService.GetLastFourDigits(request.AccountNumber);

            // Create bank account entity
            var bankAccount = new BankAccount
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                AccountHolderName = request.AccountHolderName.Trim(),
                RoutingNumberEncrypted = encryptedRoutingNumber,
                AccountNumberEncrypted = encryptedAccountNumber,
                LastFourDigits = lastFourDigits,
                AccountType = request.AccountType.ToLower(),
                BankName = request.BankName?.Trim(),
                IsPrimary = request.IsPrimary,
                IsVerified = false, // Requires verification flow
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save to database (repository handles primary account logic)
            var created = await _bankAccountRepository.AddAsync(bankAccount);

            _logger.LogInformation("AddBankAccount: Created bank account {BankAccountId} for user {UserId}, Primary: {IsPrimary}", created.Id, userId, created.IsPrimary);

            var response = MapToResponse(created);
            return CreatedAtAction(
                nameof(GetBankAccountById),
                new { id = created.Id },
                response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddBankAccount: Error creating bank account for user {UserId}", userId);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to create bank account" } });
        }
    }

    /// <summary>
    /// Update existing bank account (metadata only)
    /// </summary>
    /// <param name="id">Bank account ID</param>
    /// <param name="request">Updated bank account details</param>
    /// <returns>Updated bank account</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankAccountResponse>> UpdateBankAccount(
        Guid id,
        [FromBody] UpdateBankAccountRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("UpdateBankAccount: User ID not found in token");
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        // Validate account holder name
        var nameValidation = _validationService.ValidateAccountHolderName(request.AccountHolderName);
        if (!nameValidation.IsValid)
        {
            _logger.LogWarning("UpdateBankAccount: Invalid account holder name for user {UserId}", userId);
            return BadRequest(new { error = new { code = "INVALID_ACCOUNT_HOLDER_NAME", message = nameValidation.ErrorMessage } });
        }

        try
        {
            // Get existing bank account
            var existingAccount = await _bankAccountRepository.GetByIdAsync(id);

            if (existingAccount == null)
            {
                _logger.LogWarning("UpdateBankAccount: Bank account {BankAccountId} not found", id);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Bank account not found" } });
            }

            // Verify ownership
            if (existingAccount.UserId != userId.Value)
            {
                _logger.LogWarning("UpdateBankAccount: User {UserId} attempted to update bank account {BankAccountId} owned by {OwnerId}",
                    userId, id, existingAccount.UserId);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Bank account not found" } });
            }

            // Update metadata only (cannot change routing/account numbers for security)
            existingAccount.AccountHolderName = request.AccountHolderName.Trim();
            existingAccount.BankName = request.BankName?.Trim();
            existingAccount.IsPrimary = request.IsPrimary;
            existingAccount.UpdatedAt = DateTime.UtcNow;

            // Save changes (repository handles primary account logic)
            var updated = await _bankAccountRepository.UpdateAsync(existingAccount);

            _logger.LogInformation("UpdateBankAccount: Updated bank account {BankAccountId} for user {UserId}", id, userId);

            return Ok(MapToResponse(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateBankAccount: Error updating bank account {BankAccountId}", id);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to update bank account" } });
        }
    }

    /// <summary>
    /// Delete bank account (soft delete)
    /// </summary>
    /// <param name="id">Bank account ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBankAccount(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("DeleteBankAccount: User ID not found in token");
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        try
        {
            // Verify ownership before deletion
            var existingAccount = await _bankAccountRepository.GetByIdAsync(id);

            if (existingAccount == null)
            {
                _logger.LogWarning("DeleteBankAccount: Bank account {BankAccountId} not found", id);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Bank account not found" } });
            }

            if (existingAccount.UserId != userId.Value)
            {
                _logger.LogWarning("DeleteBankAccount: User {UserId} attempted to delete bank account {BankAccountId} owned by {OwnerId}",
                    userId, id, existingAccount.UserId);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Bank account not found" } });
            }

            // TODO: Check if bank account has pending payouts
            // This will be implemented when payout endpoints are created
            // For now, allow deletion

            // Soft delete
            var deleted = await _bankAccountRepository.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogError("DeleteBankAccount: Failed to delete bank account {BankAccountId}", id);
                return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to delete bank account" } });
            }

            _logger.LogInformation("DeleteBankAccount: Deleted bank account {BankAccountId} for user {UserId}", id, userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteBankAccount: Error deleting bank account {BankAccountId}", id);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to delete bank account" } });
        }
    }

    /// <summary>
    /// Validate bank account and lookup bank name from routing number
    /// </summary>
    /// <param name="request">Validation request with routing number</param>
    /// <returns>Validation result with suggested bank name</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(BankValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<BankValidationResponse> ValidateBankAccount([FromBody] BankAccountValidationRequest request)
    {
        var validationResult = _validationService.ValidateComplete(request);

        var response = new BankValidationResponse
        {
            IsValid = validationResult.IsValid,
            Errors = validationResult.Errors,
            Warnings = validationResult.Warnings,
            SuggestedBankName = validationResult.SuggestedBankName
        };

        _logger.LogInformation("ValidateBankAccount: Validation result - IsValid: {IsValid}, Errors: {ErrorCount}, Warnings: {WarningCount}",
            validationResult.IsValid, validationResult.Errors.Count, validationResult.Warnings.Count);

        return Ok(response);
    }

    /// <summary>
    /// Lookup bank name from routing number
    /// </summary>
    /// <param name="routingNumber">9-digit routing number</param>
    /// <returns>Bank name if found</returns>
    [HttpGet("lookup/{routingNumber}")]
    [ProducesResponseType(typeof(BankLookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BankLookupResponse> LookupBankName(string routingNumber)
    {
        var bankName = _validationService.LookupBankName(routingNumber);

        if (bankName == null)
        {
            _logger.LogInformation("LookupBankName: No bank found for routing number {RoutingNumber}", routingNumber);
            return NotFound(new { error = new { code = "BANK_NOT_FOUND", message = "Bank not found for this routing number" } });
        }

        var response = new BankLookupResponse
        {
            RoutingNumber = routingNumber,
            BankName = bankName
        };

        _logger.LogInformation("LookupBankName: Found bank {BankName} for routing number {RoutingNumber}", bankName, routingNumber);

        return Ok(response);
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

    /// <summary>
    /// Map BankAccount entity to response DTO
    /// </summary>
    private static BankAccountResponse MapToResponse(BankAccount bankAccount)
    {
        return new BankAccountResponse
        {
            Id = bankAccount.Id,
            AccountHolderName = bankAccount.AccountHolderName,
            AccountType = bankAccount.AccountType,
            BankName = bankAccount.BankName,
            LastFourDigits = bankAccount.LastFourDigits,
            IsPrimary = bankAccount.IsPrimary,
            IsVerified = bankAccount.IsVerified,
            CreatedAt = bankAccount.CreatedAt,
            UpdatedAt = bankAccount.UpdatedAt
        };
    }
}
