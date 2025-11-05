import React, { useState, useEffect } from 'react';
import {
  SLIPPAGE_PRESETS,
  MIN_SLIPPAGE,
  MAX_SLIPPAGE,
  HIGH_SLIPPAGE_THRESHOLD,
} from '@/types/swap';

interface SlippageSettingsProps {
  value: number;
  onChange: (slippage: number) => void;
}

export const SlippageSettings: React.FC<SlippageSettingsProps> = ({
  value,
  onChange,
}) => {
  const [isCustom, setIsCustom] = useState(false);
  const [customValue, setCustomValue] = useState<string>('');
  const [error, setError] = useState<string>('');

  useEffect(() => {
    // Check if current value matches a preset
    const isPreset = SLIPPAGE_PRESETS.some((preset) => preset.value === value);
    if (!isPreset && value > 0) {
      setIsCustom(true);
      setCustomValue(value.toString());
    }
  }, [value]);

  const handlePresetClick = (presetValue: number) => {
    setIsCustom(false);
    setCustomValue('');
    setError('');
    onChange(presetValue);
  };

  const handleCustomClick = () => {
    setIsCustom(true);
    setCustomValue(value.toString());
  };

  const handleCustomChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setCustomValue(val);

    if (!val) {
      setError('Please enter a value');
      return;
    }

    const numVal = parseFloat(val);

    if (isNaN(numVal)) {
      setError('Please enter a valid number');
      return;
    }

    if (numVal < MIN_SLIPPAGE) {
      setError(`Minimum slippage is ${MIN_SLIPPAGE}%`);
      return;
    }

    if (numVal > MAX_SLIPPAGE) {
      setError(`Maximum slippage is ${MAX_SLIPPAGE}%`);
      return;
    }

    setError('');
    onChange(numVal);
  };

  const isHighSlippage = value > HIGH_SLIPPAGE_THRESHOLD;

  return (
    <div className="bg-white rounded-lg border border-gray-200 p-4">
      <div className="flex items-center justify-between mb-3">
        <span className="text-sm font-medium text-gray-700">
          Slippage Tolerance
        </span>
        <button
          className="text-gray-400 hover:text-gray-600 group relative"
          aria-label="Slippage tolerance information"
        >
          <svg
            className="w-4 h-4"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <span className="absolute bottom-full right-0 mb-2 w-64 p-2 text-xs text-white bg-gray-900 rounded shadow-lg opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none">
            Your transaction will revert if the price changes unfavorably by
            more than this percentage
          </span>
        </button>
      </div>

      <div className="flex gap-2 mb-3">
        {SLIPPAGE_PRESETS.map((preset) => (
          <button
            key={preset.value}
            onClick={() => handlePresetClick(preset.value)}
            className={`flex-1 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
              !isCustom && value === preset.value
                ? 'bg-indigo-600 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            {preset.label}
          </button>
        ))}

        <button
          onClick={handleCustomClick}
          className={`flex-1 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
            isCustom
              ? 'bg-indigo-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          Custom
        </button>
      </div>

      {isCustom && (
        <div className="mb-3">
          <div className="relative">
            <input
              type="number"
              placeholder="0.5"
              value={customValue}
              onChange={handleCustomChange}
              min={MIN_SLIPPAGE}
              max={MAX_SLIPPAGE}
              step="0.1"
              className={`w-full px-3 py-2 pr-8 border rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent ${
                error ? 'border-red-500' : 'border-gray-300'
              }`}
              aria-label="Custom slippage percentage"
            />
            <span className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 text-sm">
              %
            </span>
          </div>
          {error && <p className="mt-1 text-xs text-red-600">{error}</p>}
        </div>
      )}

      {isHighSlippage && !error && (
        <div className="flex items-start gap-2 p-3 bg-yellow-50 border border-yellow-200 rounded-lg">
          <svg
            className="w-5 h-5 text-yellow-600 flex-shrink-0 mt-0.5"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
            />
          </svg>
          <div className="text-xs text-yellow-800">
            <strong>High slippage tolerance!</strong> Your transaction may be
            frontrun or result in an unfavorable trade.
          </div>
        </div>
      )}
    </div>
  );
};
