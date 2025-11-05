import { useState, useEffect } from 'react';
import { Dialog, Transition } from '@headlessui/react';
import { Fragment } from 'react';
import { WelcomeStep } from './WelcomeStep';
import { WalletSetupStep } from './WalletSetupStep';
import { FeatureTourStep } from './FeatureTourStep';
import { ProgressIndicator } from './ProgressIndicator';

interface OnboardingWizardProps {
  isOpen: boolean;
  onClose: () => void;
  onComplete: () => void;
}

const STORAGE_KEY = 'coinpay_onboarding_completed';

/**
 * OnboardingWizard Component
 *
 * A 3-step wizard that welcomes new users and guides them through CoinPay features.
 * Saves completion status to localStorage and supports skip/back/next navigation.
 *
 * Features:
 * - Step 1: Welcome message with feature highlights
 * - Step 2: Wallet setup and security best practices
 * - Step 3: Feature tour with key capabilities
 * - Progress indicator showing current progress
 * - localStorage persistence for completion status
 */
export function OnboardingWizard({ isOpen, onClose, onComplete }: OnboardingWizardProps) {
  const [currentStep, setCurrentStep] = useState(1);
  const totalSteps = 3;

  const handleNext = () => {
    if (currentStep < totalSteps) {
      setCurrentStep(currentStep + 1);
    } else {
      handleComplete();
    }
  };

  const handleBack = () => {
    if (currentStep > 1) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleComplete = () => {
    localStorage.setItem(STORAGE_KEY, 'true');
    onComplete();
  };

  const handleSkip = () => {
    localStorage.setItem(STORAGE_KEY, 'true');
    onClose();
  };

  return (
    <Transition appear show={isOpen} as={Fragment}>
      <Dialog as="div" className="relative z-50" onClose={() => {}}>
        {/* Backdrop */}
        <Transition.Child
          as={Fragment}
          enter="ease-out duration-300"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="ease-in duration-200"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-black/50 backdrop-blur-sm" />
        </Transition.Child>

        {/* Modal Container */}
        <div className="fixed inset-0 overflow-y-auto">
          <div className="flex min-h-full items-center justify-center p-4">
            <Transition.Child
              as={Fragment}
              enter="ease-out duration-300"
              enterFrom="opacity-0 scale-95"
              enterTo="opacity-100 scale-100"
              leave="ease-in duration-200"
              leaveFrom="opacity-100 scale-100"
              leaveTo="opacity-0 scale-95"
            >
              <Dialog.Panel className="w-full max-w-2xl transform overflow-hidden rounded-2xl bg-white shadow-2xl transition-all">
                {/* Header with Progress */}
                <div className="flex items-start justify-between bg-gray-50 px-6 py-4 border-b border-gray-200">
                  <div className="flex-1">
                    <ProgressIndicator currentStep={currentStep} totalSteps={totalSteps} />
                  </div>
                  <button
                    onClick={handleSkip}
                    className="ml-4 text-sm text-gray-500 hover:text-gray-700 font-medium transition-colors whitespace-nowrap flex-shrink-0"
                    aria-label="Skip onboarding"
                  >
                    Skip
                  </button>
                </div>

                {/* Step Content */}
                <div className="px-8 py-10">
                  {currentStep === 1 && <WelcomeStep />}
                  {currentStep === 2 && <WalletSetupStep />}
                  {currentStep === 3 && <FeatureTourStep />}
                </div>

                {/* Navigation Buttons */}
                <div className="bg-gray-50 px-8 py-4 border-t border-gray-200 flex items-center justify-between">
                  <button
                    onClick={handleBack}
                    disabled={currentStep === 1}
                    className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                    aria-label="Go back to previous step"
                  >
                    Back
                  </button>

                  <button
                    onClick={handleNext}
                    className="px-6 py-2 text-sm font-medium text-white bg-primary-500 rounded-md hover:bg-primary-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-colors"
                    aria-label={currentStep === totalSteps ? 'Complete onboarding' : 'Go to next step'}
                  >
                    {currentStep === totalSteps ? 'Get Started' : 'Next'}
                  </button>
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
}

/**
 * useOnboarding Hook
 *
 * Manages onboarding state and persistence.
 * Returns state of whether onboarding has been completed and methods to manage it.
 */
export function useOnboarding() {
  const [shouldShowOnboarding, setShouldShowOnboarding] = useState(false);

  useEffect(() => {
    const hasCompletedOnboarding = localStorage.getItem(STORAGE_KEY);
    setShouldShowOnboarding(!hasCompletedOnboarding);
  }, []);

  const markAsCompleted = () => {
    localStorage.setItem(STORAGE_KEY, 'true');
    setShouldShowOnboarding(false);
  };

  const resetOnboarding = () => {
    localStorage.removeItem(STORAGE_KEY);
    setShouldShowOnboarding(true);
  };

  return {
    shouldShowOnboarding,
    markAsCompleted,
    resetOnboarding,
  };
}
