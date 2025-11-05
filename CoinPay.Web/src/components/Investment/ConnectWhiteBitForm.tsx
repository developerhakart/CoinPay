import React, { useState, useEffect } from 'react';
import { useInvestmentStore } from '@/store/investmentStore';
import { investmentService } from '@/services';

interface FormData {
  apiKey: string;
  apiSecret: string;
}

interface ValidationErrors {
  apiKey?: string;
  apiSecret?: string;
}

export const ConnectWhiteBitForm: React.FC = () => {
  const {
    isConnected,
    isConnecting,
    connectionStatus,
    setConnecting,
    setConnectionStatus,
    setError,
  } = useInvestmentStore();

  const [formData, setFormData] = useState<FormData>({
    apiKey: '',
    apiSecret: '',
  });

  const [errors, setErrors] = useState<ValidationErrors>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});
  const [showSuccess, setShowSuccess] = useState(false);

  // Check connection status on mount
  useEffect(() => {
    const checkStatus = async () => {
      try {
        const status = await investmentService.getWhiteBitStatus();
        setConnectionStatus(status);
      } catch (error) {
        console.error('Failed to check connection status:', error);
      }
    };
    checkStatus();
  }, [setConnectionStatus]);

  // Validation
  const validateForm = (data: FormData): ValidationErrors => {
    const errors: ValidationErrors = {};

    if (!data.apiKey.trim()) {
      errors.apiKey = 'API Key is required';
    } else if (data.apiKey.length < 32) {
      errors.apiKey = 'API Key must be at least 32 characters';
    }

    if (!data.apiSecret.trim()) {
      errors.apiSecret = 'API Secret is required';
    } else if (data.apiSecret.length < 32) {
      errors.apiSecret = 'API Secret must be at least 32 characters';
    }

    return errors;
  };

  // Real-time validation
  useEffect(() => {
    if (Object.keys(touched).length > 0) {
      const newErrors = validateForm(formData);
      setErrors(newErrors);
    }
  }, [formData, touched]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleBlur = (e: React.FocusEvent<HTMLInputElement>) => {
    const { name } = e.target;
    setTouched((prev) => ({ ...prev, [name]: true }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Mark all fields as touched
    setTouched({ apiKey: true, apiSecret: true });

    // Validate
    const validationErrors = validateForm(formData);
    setErrors(validationErrors);

    if (Object.keys(validationErrors).length > 0) {
      return;
    }

    setConnecting(true);
    setError(null);

    try {
      const connection = await investmentService.connectWhiteBit({
        apiKey: formData.apiKey,
        apiSecret: formData.apiSecret,
      });

      // Update connection status
      setConnectionStatus({
        connected: true,
        connectionId: connection.connectionId,
        exchangeName: connection.exchangeName,
        connectedAt: connection.connectedAt,
      });

      // Show success message
      setShowSuccess(true);

      // Clear form
      setFormData({ apiKey: '', apiSecret: '' });
      setTouched({});

      // Hide success message after 3 seconds
      setTimeout(() => setShowSuccess(false), 3000);
    } catch (error: any) {
      const errorMessage = error.response?.data?.error || 'Failed to connect WhiteBit account';
      setError(errorMessage);
    } finally {
      setConnecting(false);
    }
  };

  const isFormValid =
    Object.keys(errors).length === 0 &&
    formData.apiKey &&
    formData.apiSecret;

  // If already connected, show status
  if (isConnected && connectionStatus) {
    return (
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold text-gray-900">
            WhiteBit Connection
          </h3>
          <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800">
            <span className="w-2 h-2 rounded-full bg-green-500 mr-2"></span>
            Connected
          </span>
        </div>

        <div className="space-y-3 text-sm">
          <div className="flex justify-between">
            <span className="text-gray-600">Exchange:</span>
            <span className="font-medium text-gray-900">WhiteBit</span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-600">Connected At:</span>
            <span className="font-medium text-gray-900">
              {new Date(connectionStatus.connectedAt!).toLocaleDateString()}
            </span>
          </div>
          {connectionStatus.lastValidated && (
            <div className="flex justify-between">
              <span className="text-gray-600">Last Validated:</span>
              <span className="font-medium text-gray-900">
                {new Date(connectionStatus.lastValidated).toLocaleDateString()}
              </span>
            </div>
          )}
        </div>

        <div className="mt-6 p-4 bg-blue-50 rounded-md">
          <p className="text-sm text-blue-700">
            Your WhiteBit account is successfully connected. You can now create investments and view your positions.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <div className="mb-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          Connect WhiteBit Account
        </h3>
        <p className="text-sm text-gray-600">
          Connect your WhiteBit exchange account to start earning rewards on your crypto holdings.
        </p>
      </div>

      {showSuccess && (
        <div className="mb-6 p-4 bg-green-50 border border-green-200 rounded-md">
          <div className="flex items-center">
            <svg
              className="w-5 h-5 text-green-500 mr-2"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fillRule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                clipRule="evenodd"
              />
            </svg>
            <p className="text-sm font-medium text-green-800">
              WhiteBit account connected successfully!
            </p>
          </div>
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* API Key */}
        <div>
          <label
            htmlFor="apiKey"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            API Key *
          </label>
          <input
            type="text"
            id="apiKey"
            name="apiKey"
            value={formData.apiKey}
            onChange={handleChange}
            onBlur={handleBlur}
            className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 ${
              touched.apiKey && errors.apiKey
                ? 'border-red-500 focus:ring-red-500'
                : 'border-gray-300 focus:ring-blue-500'
            }`}
            placeholder="Enter your WhiteBit API key"
            disabled={isConnecting}
            required
          />
          {touched.apiKey && errors.apiKey && (
            <p className="mt-1 text-sm text-red-600" role="alert">
              {errors.apiKey}
            </p>
          )}
          <p className="mt-1 text-xs text-gray-500">
            Find your API key in WhiteBit Settings → API Keys
          </p>
        </div>

        {/* API Secret */}
        <div>
          <label
            htmlFor="apiSecret"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            API Secret *
          </label>
          <input
            type="password"
            id="apiSecret"
            name="apiSecret"
            value={formData.apiSecret}
            onChange={handleChange}
            onBlur={handleBlur}
            className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 ${
              touched.apiSecret && errors.apiSecret
                ? 'border-red-500 focus:ring-red-500'
                : 'border-gray-300 focus:ring-blue-500'
            }`}
            placeholder="Enter your WhiteBit API secret"
            disabled={isConnecting}
            required
          />
          {touched.apiSecret && errors.apiSecret && (
            <p className="mt-1 text-sm text-red-600" role="alert">
              {errors.apiSecret}
            </p>
          )}
          <p className="mt-1 text-xs text-gray-500">
            Your API secret is encrypted and stored securely
          </p>
        </div>

        {/* Security Notice */}
        <div className="p-4 bg-yellow-50 border border-yellow-200 rounded-md">
          <div className="flex">
            <svg
              className="w-5 h-5 text-yellow-600 mr-2 flex-shrink-0"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fillRule="evenodd"
                d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
                clipRule="evenodd"
              />
            </svg>
            <div className="text-sm text-yellow-800">
              <p className="font-medium mb-1">Security Recommendations:</p>
              <ul className="list-disc list-inside space-y-1">
                <li>Use read-only API keys when possible</li>
                <li>Enable IP whitelisting on WhiteBit</li>
                <li>Never share your API credentials</li>
              </ul>
            </div>
          </div>
        </div>

        {/* Submit Button */}
        <div className="flex justify-end space-x-3">
          <button
            type="submit"
            disabled={!isFormValid || isConnecting}
            className={`px-6 py-2 rounded-md font-medium text-white transition-colors ${
              !isFormValid || isConnecting
                ? 'bg-gray-400 cursor-not-allowed'
                : 'bg-blue-600 hover:bg-blue-700'
            }`}
          >
            {isConnecting ? (
              <span className="flex items-center">
                <svg
                  className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                  xmlns="http://www.w3.org/2000/svg"
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
                  ></circle>
                  <path
                    className="opacity-75"
                    fill="currentColor"
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                  ></path>
                </svg>
                Connecting...
              </span>
            ) : (
              'Connect WhiteBit'
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default ConnectWhiteBitForm;
