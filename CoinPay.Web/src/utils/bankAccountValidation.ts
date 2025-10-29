/**
 * Bank account validation utilities
 * Implements US ACH validation rules
 */

export interface ValidationResult {
  isValid: boolean;
  error?: string;
}

/**
 * Validate US routing number (9 digits with checksum)
 * Uses ABA routing number algorithm
 */
export function validateRoutingNumber(routing: string): ValidationResult {
  // Remove any non-digit characters
  const digitsOnly = routing.replace(/\D/g, '');

  // Check length
  if (digitsOnly.length !== 9) {
    return {
      isValid: false,
      error: 'Routing number must be exactly 9 digits',
    };
  }

  // Validate checksum using ABA algorithm
  // Multiply digits by weights [3,7,1,3,7,1,3,7,1] and sum
  const weights = [3, 7, 1, 3, 7, 1, 3, 7, 1];
  let sum = 0;

  for (let i = 0; i < 9; i++) {
    sum += parseInt(digitsOnly[i]) * weights[i];
  }

  // Sum must be divisible by 10
  if (sum % 10 !== 0) {
    return {
      isValid: false,
      error: 'Invalid routing number checksum',
    };
  }

  return { isValid: true };
}

/**
 * Validate US bank account number
 * Length: 5-17 digits
 */
export function validateAccountNumber(account: string): ValidationResult {
  // Remove any non-digit characters
  const digitsOnly = account.replace(/\D/g, '');

  // Check length
  if (digitsOnly.length < 5) {
    return {
      isValid: false,
      error: 'Account number must be at least 5 digits',
    };
  }

  if (digitsOnly.length > 17) {
    return {
      isValid: false,
      error: 'Account number must be at most 17 digits',
    };
  }

  return { isValid: true };
}

/**
 * Validate account holder name
 * Length: 2-255 characters
 * Allowed: letters, spaces, hyphens, apostrophes
 */
export function validateAccountHolderName(name: string): ValidationResult {
  const trimmed = name.trim();

  // Check length
  if (trimmed.length < 2) {
    return {
      isValid: false,
      error: 'Account holder name must be at least 2 characters',
    };
  }

  if (trimmed.length > 255) {
    return {
      isValid: false,
      error: 'Account holder name must be at most 255 characters',
    };
  }

  // Check format: letters, spaces, hyphens, apostrophes only
  const validPattern = /^[a-zA-Z\s\-']+$/;
  if (!validPattern.test(trimmed)) {
    return {
      isValid: false,
      error: 'Account holder name can only contain letters, spaces, hyphens, and apostrophes',
    };
  }

  return { isValid: true };
}

/**
 * Format routing number for display: XXX-XXX-XXX
 */
export function formatRoutingNumber(routing: string): string {
  const digitsOnly = routing.replace(/\D/g, '');

  if (digitsOnly.length !== 9) {
    return digitsOnly;
  }

  return `${digitsOnly.slice(0, 3)}-${digitsOnly.slice(3, 6)}-${digitsOnly.slice(6, 9)}`;
}

/**
 * Mask account number showing only last 4 digits: •••• XXXX
 */
export function maskAccountNumber(account: string): string {
  const digitsOnly = account.replace(/\D/g, '');

  if (digitsOnly.length <= 4) {
    return digitsOnly;
  }

  const lastFour = digitsOnly.slice(-4);
  return `•••• ${lastFour}`;
}

/**
 * Get last 4 digits of account number
 */
export function getLastFourDigits(account: string): string {
  const digitsOnly = account.replace(/\D/g, '');
  return digitsOnly.slice(-4);
}

/**
 * Validate all bank account fields
 */
export interface BankAccountFormData {
  accountHolderName: string;
  routingNumber: string;
  accountNumber: string;
  accountType: 'checking' | 'savings';
  bankName?: string;
  isPrimary?: boolean;
}

export interface BankAccountValidationErrors {
  accountHolderName?: string;
  routingNumber?: string;
  accountNumber?: string;
  accountType?: string;
}

export function validateBankAccountForm(
  data: BankAccountFormData
): BankAccountValidationErrors {
  const errors: BankAccountValidationErrors = {};

  // Validate account holder name
  const nameResult = validateAccountHolderName(data.accountHolderName);
  if (!nameResult.isValid) {
    errors.accountHolderName = nameResult.error;
  }

  // Validate routing number
  const routingResult = validateRoutingNumber(data.routingNumber);
  if (!routingResult.isValid) {
    errors.routingNumber = routingResult.error;
  }

  // Validate account number
  const accountResult = validateAccountNumber(data.accountNumber);
  if (!accountResult.isValid) {
    errors.accountNumber = accountResult.error;
  }

  // Validate account type
  if (!data.accountType || !['checking', 'savings'].includes(data.accountType)) {
    errors.accountType = 'Please select an account type';
  }

  return errors;
}

/**
 * Check if form has any validation errors
 */
export function hasValidationErrors(errors: BankAccountValidationErrors): boolean {
  return Object.keys(errors).length > 0;
}
