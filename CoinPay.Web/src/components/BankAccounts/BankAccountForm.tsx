import React, { useState, useEffect } from 'react';
import {
  validateBankAccountForm,
  validateRoutingNumber,
  validateAccountNumber,
  validateAccountHolderName,
  formatRoutingNumber,
  maskAccountNumber,
  hasValidationErrors,
  type BankAccountFormData,
  type BankAccountValidationErrors,
} from '../../utils/bankAccountValidation';

interface BankAccountFormProps {
  mode: 'add' | 'edit';
  initialData?: Partial<BankAccountFormData>;
  onSubmit: (data: BankAccountFormData) => Promise<void>;
  onCancel: () => void;
}

export const BankAccountForm: React.FC<BankAccountFormProps> = ({
  mode,
  initialData,
  onSubmit,
  onCancel,
}) => {
  const [formData, setFormData] = useState<BankAccountFormData>({
    accountHolderName: initialData?.accountHolderName || '',
    routingNumber: initialData?.routingNumber || '',
    accountNumber: initialData?.accountNumber || '',
    accountType: initialData?.accountType || 'checking',
    bankName: initialData?.bankName || '',
    isPrimary: initialData?.isPrimary || false,
  });

  const [errors, setErrors] = useState<BankAccountValidationErrors>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Real-time validation with debounce
  useEffect(() => {
    const timer = setTimeout(() => {
      if (Object.keys(touched).length > 0) {
        const newErrors = validateBankAccountForm(formData);
        setErrors(newErrors);
      }
    }, 300);

    return () => clearTimeout(timer);
  }, [formData, touched]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value, type } = e.target;
    const checked = (e.target as HTMLInputElement).checked;

    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleBlur = (e: React.FocusEvent<HTMLInputElement>) => {
    const { name } = e.target;
    setTouched((prev) => ({ ...prev, [name]: true }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Mark all fields as touched
    setTouched({
      accountHolderName: true,
      routingNumber: true,
      accountNumber: true,
      accountType: true,
    });

    // Validate
    const validationErrors = validateBankAccountForm(formData);
    setErrors(validationErrors);

    if (hasValidationErrors(validationErrors)) {
      return;
    }

    setIsSubmitting(true);

    try {
      await onSubmit(formData);
      // Success - parent component will handle closing modal
    } catch (error) {
      console.error('Failed to submit bank account:', error);
      // Show error notification (implement with toast/notification system)
    } finally {
      setIsSubmitting(false);
    }
  };

  const isFormValid = !hasValidationErrors(errors) &&
    formData.accountHolderName &&
    formData.routingNumber &&
    formData.accountNumber;

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      {/* Account Holder Name */}
      <div>
        <label
          htmlFor="accountHolderName"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Account Holder Name *
        </label>
        <input
          type="text"
          id="accountHolderName"
          name="accountHolderName"
          value={formData.accountHolderName}
          onChange={handleChange}
          onBlur={handleBlur}
          className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 ${
            touched.accountHolderName && errors.accountHolderName
              ? 'border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:ring-blue-500'
          }`}
          placeholder="John Doe"
          disabled={isSubmitting}
          required
        />
        {touched.accountHolderName && errors.accountHolderName && (
          <p className="mt-1 text-sm text-red-600" role="alert">
            {errors.accountHolderName}
          </p>
        )}
        <p className="mt-1 text-xs text-gray-500">
          Name as it appears on your bank account
        </p>
      </div>

      {/* Routing Number */}
      <div>
        <label
          htmlFor="routingNumber"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Routing Number *
        </label>
        <input
          type="text"
          id="routingNumber"
          name="routingNumber"
          value={formData.routingNumber}
          onChange={handleChange}
          onBlur={handleBlur}
          maxLength={11} // Allow for dashes in formatting
          className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 font-mono ${
            touched.routingNumber && errors.routingNumber
              ? 'border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:ring-blue-500'
          }`}
          placeholder="011401533"
          disabled={isSubmitting || mode === 'edit'} // Cannot edit routing in edit mode
          required
        />
        {touched.routingNumber && errors.routingNumber && (
          <p className="mt-1 text-sm text-red-600" role="alert">
            {errors.routingNumber}
          </p>
        )}
        {mode === 'edit' ? (
          <p className="mt-1 text-xs text-gray-500">
            Routing number cannot be changed for security reasons
          </p>
        ) : (
          <p className="mt-1 text-xs text-gray-500">
            9-digit routing number (usually found at bottom of check)
          </p>
        )}
      </div>

      {/* Account Number */}
      <div>
        <label
          htmlFor="accountNumber"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Account Number *
        </label>
        <input
          type="text"
          id="accountNumber"
          name="accountNumber"
          value={formData.accountNumber}
          onChange={handleChange}
          onBlur={handleBlur}
          maxLength={17}
          className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 font-mono ${
            touched.accountNumber && errors.accountNumber
              ? 'border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:ring-blue-500'
          }`}
          placeholder="1234567890"
          disabled={isSubmitting || mode === 'edit'} // Cannot edit account number in edit mode
          required
        />
        {touched.accountNumber && errors.accountNumber && (
          <p className="mt-1 text-sm text-red-600" role="alert">
            {errors.accountNumber}
          </p>
        )}
        {mode === 'edit' ? (
          <p className="mt-1 text-xs text-gray-500">
            Account number cannot be changed for security reasons
          </p>
        ) : (
          <p className="mt-1 text-xs text-gray-500">
            5-17 digit account number
          </p>
        )}
      </div>

      {/* Account Type */}
      <div>
        <label
          htmlFor="accountType"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Account Type *
        </label>
        <select
          id="accountType"
          name="accountType"
          value={formData.accountType}
          onChange={handleChange}
          className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          disabled={isSubmitting}
          required
        >
          <option value="checking">Checking</option>
          <option value="savings">Savings</option>
        </select>
      </div>

      {/* Bank Name (Optional) */}
      <div>
        <label
          htmlFor="bankName"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Bank Name (Optional)
        </label>
        <input
          type="text"
          id="bankName"
          name="bankName"
          value={formData.bankName}
          onChange={handleChange}
          className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Wells Fargo"
          disabled={isSubmitting}
        />
        <p className="mt-1 text-xs text-gray-500">
          Help identify your bank account
        </p>
      </div>

      {/* Primary Account Checkbox */}
      <div className="flex items-center">
        <input
          type="checkbox"
          id="isPrimary"
          name="isPrimary"
          checked={formData.isPrimary}
          onChange={handleChange}
          className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
          disabled={isSubmitting}
        />
        <label htmlFor="isPrimary" className="ml-2 block text-sm text-gray-700">
          Set as primary bank account
        </label>
      </div>

      {/* Form Actions */}
      <div className="flex justify-end space-x-3 pt-4 border-t border-gray-200">
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmitting}
          className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={!isFormValid || isSubmitting}
          className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isSubmitting ? (
            <span className="flex items-center">
              <svg
                className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                />
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                />
              </svg>
              Saving...
            </span>
          ) : (
            mode === 'add' ? 'Add Bank Account' : 'Save Changes'
          )}
        </button>
      </div>
    </form>
  );
};
