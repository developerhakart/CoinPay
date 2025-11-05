import React from 'react';
import { PRICE_IMPACT_LEVELS } from '@/types/swap';

interface PriceImpactIndicatorProps {
  priceImpact: number | null | undefined;
}

export const PriceImpactIndicator: React.FC<PriceImpactIndicatorProps> = ({
  priceImpact,
}) => {
  if (priceImpact === null || priceImpact === undefined || priceImpact === 0) {
    return null;
  }

  const getImpactLevel = () => {
    for (const level of PRICE_IMPACT_LEVELS) {
      if (priceImpact < level.threshold) {
        return level;
      }
    }
    return PRICE_IMPACT_LEVELS[PRICE_IMPACT_LEVELS.length - 1]!;
  };

  const impactLevel = getImpactLevel();

  const colorClasses = {
    green: 'text-green-600 bg-green-50 border-green-200',
    yellow: 'text-yellow-600 bg-yellow-50 border-yellow-200',
    red: 'text-red-600 bg-red-50 border-red-200',
  };

  const iconColorClasses = {
    green: 'text-green-600',
    yellow: 'text-yellow-600',
    red: 'text-red-600',
  };

  const getSuggestion = () => {
    if (impactLevel.level === 'high') {
      return 'Consider splitting this trade into smaller amounts to reduce price impact';
    }
    return null;
  };

  return (
    <div
      className={`rounded-lg border p-3 ${colorClasses[impactLevel.color]}`}
    >
      <div className="flex items-center justify-between mb-2">
        <span className="text-sm font-medium">Price Impact</span>
        <span className="text-lg font-bold">{priceImpact.toFixed(2)}%</span>
      </div>

      {getSuggestion() && (
        <div className="flex items-start gap-2 mt-2">
          <svg
            className={`w-4 h-4 flex-shrink-0 mt-0.5 ${
              iconColorClasses[impactLevel.color]
            }`}
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
          <p className="text-xs">{getSuggestion()}</p>
        </div>
      )}
    </div>
  );
};
