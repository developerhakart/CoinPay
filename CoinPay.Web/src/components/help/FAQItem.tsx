import { Disclosure, Transition } from '@headlessui/react';
import { FAQ } from '@/data/faqData';

interface FAQItemProps {
  faq: FAQ;
  /**
   * Additional CSS classes to apply to the item
   */
  className?: string;
  /**
   * Callback when item is expanded
   */
  onExpand?: (faq: FAQ) => void;
}

/**
 * FAQItem - Individual FAQ accordion item
 * Displays a question that can be expanded to reveal the answer
 */
export function FAQItem({ faq, className = '', onExpand }: FAQItemProps) {
  const handleToggle = (open: boolean) => {
    if (open && onExpand) {
      onExpand(faq);
    }
  };

  return (
    <Disclosure>
      {({ open }) => {
        // Call onExpand when disclosure opens
        if (open && !open) {
          handleToggle(open);
        }

        return (
          <div
            className={`bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md transition-shadow ${className}`}
            data-testid={`faq-item-${faq.id}`}
          >
            <Disclosure.Button
              onClick={() => handleToggle(true)}
              className="w-full px-6 py-4 text-left flex items-center justify-between gap-4 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-inset rounded-lg hover:bg-gray-50 transition-colors"
              data-testid={`faq-question-${faq.id}`}
            >
              <div className="flex-1 min-w-0">
                {/* Category Badge */}
                <div className="mb-2">
                  <span className="inline-block px-2 py-1 text-xs font-medium text-primary-700 bg-primary-100 rounded-full">
                    {faq.category}
                  </span>
                </div>

                {/* Question Text */}
                <span className="font-semibold text-gray-900 block text-base">
                  {faq.question}
                </span>
              </div>

              {/* Chevron Icon */}
              <svg
                className={`flex-shrink-0 w-5 h-5 text-primary-500 transition-transform duration-200 ${
                  open ? 'rotate-180' : ''
                }`}
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                aria-hidden="true"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M19 9l-7 7-7-7"
                />
              </svg>
            </Disclosure.Button>

            {/* Answer Panel */}
            <Transition
              show={open}
              enter="transition duration-100 ease-out"
              enterFrom="transform scale-95 opacity-0"
              enterTo="transform scale-100 opacity-100"
              leave="transition duration-75 ease-out"
              leaveFrom="transform scale-100 opacity-100"
              leaveTo="transform scale-95 opacity-0"
              unmount={false}
            >
              <Disclosure.Panel
                className="px-6 pb-4"
                data-testid={`faq-answer-${faq.id}`}
              >
                <div className="pt-4 border-t border-gray-100">
                  {/* Answer Text */}
                  <p className="text-gray-600 whitespace-pre-line leading-relaxed text-sm">
                    {faq.answer}
                  </p>

                  {/* Tags */}
                  {faq.tags && faq.tags.length > 0 && (
                    <div className="mt-4 pt-4 border-t border-gray-100">
                      <div className="flex flex-wrap gap-2">
                        {faq.tags.map((tag) => (
                          <span
                            key={tag}
                            className="inline-block px-2 py-1 text-xs text-gray-600 bg-gray-100 rounded"
                          >
                            #{tag}
                          </span>
                        ))}
                      </div>
                    </div>
                  )}

                  {/* Helpful Actions */}
                  <div className="mt-4 flex gap-4 text-sm">
                    <button
                      className="text-gray-500 hover:text-gray-700 transition-colors flex items-center gap-1"
                      data-testid={`faq-helpful-${faq.id}`}
                    >
                      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          strokeWidth={2}
                          d="M14 10h4.764a2 2 0 011.789 2.894l-3.646 7.23a2 2 0 01-1.789 1.106H7a2 2 0 01-2-2V7a2 2 0 012-2h.5a2 2 0 012 2v12"
                        />
                      </svg>
                      Helpful
                    </button>
                    <button
                      className="text-gray-500 hover:text-gray-700 transition-colors flex items-center gap-1"
                      data-testid={`faq-not-helpful-${faq.id}`}
                    >
                      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          strokeWidth={2}
                          d="M10 14H5.236a2 2 0 01-1.789-2.894l3.646-7.23a2 2 0 011.789-1.106H17a2 2 0 012 2v12a2 2 0 01-2 2h-.5a2 2 0 01-2-2v-12"
                        />
                      </svg>
                      Not Helpful
                    </button>
                  </div>
                </div>
              </Disclosure.Panel>
            </Transition>
          </div>
        );
      }}
    </Disclosure>
  );
}
