/**
 * ProgressIndicator Component
 *
 * Displays visual progress through the onboarding steps.
 * Shows the current step number and a visual progress bar.
 */

interface ProgressIndicatorProps {
  currentStep: number;
  totalSteps: number;
}

export function ProgressIndicator({
  currentStep,
  totalSteps,
}: ProgressIndicatorProps) {
  const progress = (currentStep / totalSteps) * 100;

  return (
    <div className="bg-gray-50 px-6 py-4 border-b border-gray-200">
      {/* Step counter and skip button header */}
      <div className="flex items-center justify-between mb-2">
        <span className="text-sm font-medium text-gray-700">
          Step {currentStep} of {totalSteps}
        </span>
      </div>

      {/* Progress bar */}
      <div className="relative h-2 bg-gray-200 rounded-full overflow-hidden">
        <div
          className="absolute inset-y-0 left-0 bg-gradient-to-r from-primary-500 to-primary-600 rounded-full transition-all duration-500"
          style={{ width: `${progress}%` }}
          role="progressbar"
          aria-valuenow={currentStep}
          aria-valuemin={1}
          aria-valuemax={totalSteps}
          aria-label={`Progress: step ${currentStep} of ${totalSteps}`}
        />
      </div>

      {/* Step indicators */}
      <div className="flex items-center gap-1.5 mt-3">
        {Array.from({ length: totalSteps }).map((_, index) => (
          <div
            key={index}
            className={`h-2 rounded-full transition-all duration-300 ${
              index + 1 === currentStep
                ? 'w-8 bg-primary-500'
                : index + 1 < currentStep
                ? 'w-2 bg-primary-300'
                : 'w-2 bg-gray-300'
            }`}
            aria-hidden="true"
          />
        ))}
      </div>
    </div>
  );
}
