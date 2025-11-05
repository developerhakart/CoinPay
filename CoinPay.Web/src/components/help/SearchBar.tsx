import { useEffect, useRef } from 'react';

interface SearchBarProps {
  /**
   * Current search value
   */
  value: string;
  /**
   * Callback when search value changes
   */
  onChange: (value: string) => void;
  /**
   * Placeholder text
   */
  placeholder?: string;
  /**
   * Whether search is loading
   */
  isLoading?: boolean;
  /**
   * Additional CSS classes
   */
  className?: string;
  /**
   * Whether to auto-focus on mount
   */
  autoFocus?: boolean;
  /**
   * Debounce delay in milliseconds (default: 300)
   */
  debounceDelay?: number;
}

/**
 * SearchBar - Real-time search input with icon and clear button
 * Provides users with a simple interface to search FAQs
 */
export function SearchBar({
  value,
  onChange,
  placeholder = 'Search help articles...',
  isLoading = false,
  className = '',
  autoFocus = true,
  debounceDelay = 300,
}: SearchBarProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const debounceTimerRef = useRef<ReturnType<typeof setTimeout>>();

  // Focus input on mount if autoFocus is true
  useEffect(() => {
    if (autoFocus && inputRef.current) {
      inputRef.current.focus();
    }
  }, [autoFocus]);

  // Handle input change with debouncing
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = event.target.value;

    // Clear existing timer
    if (debounceTimerRef.current) {
      clearTimeout(debounceTimerRef.current);
    }

    // Set new timer for debounced callback
    if (debounceDelay > 0) {
      debounceTimerRef.current = setTimeout(() => {
        onChange(newValue);
      }, debounceDelay);
    } else {
      onChange(newValue);
    }
  };

  // Clear search
  const handleClear = () => {
    onChange('');
    if (inputRef.current) {
      inputRef.current.focus();
    }
  };

  // Cleanup debounce timer on unmount
  useEffect(() => {
    return () => {
      if (debounceTimerRef.current) {
        clearTimeout(debounceTimerRef.current);
      }
    };
  }, []);

  return (
    <div className={`relative ${className}`} data-testid="search-bar">
      {/* Search Icon */}
      <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
        <svg
          className={`h-5 w-5 ${isLoading ? 'animate-spin text-primary-400' : 'text-gray-400'}`}
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          aria-hidden="true"
        >
          {isLoading ? (
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            />
          ) : (
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
            />
          )}
        </svg>
      </div>

      {/* Input Field */}
      <input
        ref={inputRef}
        type="search"
        value={value}
        onChange={handleInputChange}
        className="block w-full pl-12 pr-10 py-3 border border-gray-300 rounded-lg bg-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-ring text-base"
        placeholder={placeholder}
        aria-label="Search FAQ"
        aria-autocomplete="list"
        data-testid="search-input"
        disabled={isLoading}
      />

      {/* Clear Button */}
      {value && !isLoading && (
        <button
          onClick={handleClear}
          className="absolute inset-y-0 right-0 pr-4 flex items-center text-gray-400 hover:text-gray-600 transition-colors"
          aria-label="Clear search"
          type="button"
          data-testid="search-clear"
        >
          <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      )}

      {/* Character Count (optional, for UX feedback) */}
      {value && (
        <div className="absolute inset-y-0 right-10 pr-1 flex items-center text-xs text-gray-400 pointer-events-none">
          {value.length}
        </div>
      )}
    </div>
  );
}

/**
 * SearchSuggestions - Component to display search suggestions
 * Can be composed with SearchBar for enhanced UX
 */
interface SearchSuggestion {
  text: string;
  icon?: React.ReactNode;
  onSelect: () => void;
}

interface SearchSuggestionsProps {
  suggestions: SearchSuggestion[];
  isOpen: boolean;
  isLoading?: boolean;
}

export function SearchSuggestions({ suggestions, isOpen, isLoading }: SearchSuggestionsProps) {
  if (!isOpen || suggestions.length === 0) return null;

  return (
    <div
      className="absolute top-full left-0 right-0 mt-1 bg-white border border-gray-300 rounded-lg shadow-lg z-10"
      role="listbox"
      data-testid="search-suggestions"
    >
      {isLoading ? (
        <div className="px-4 py-3 text-center text-gray-500 text-sm">
          <div className="inline-block">Searching...</div>
        </div>
      ) : (
        <ul className="divide-y divide-gray-100">
          {suggestions.map((suggestion, index) => (
            <li key={index}>
              <button
                onClick={suggestion.onSelect}
                className="w-full px-4 py-3 text-left flex items-center gap-3 hover:bg-gray-50 transition-colors focus:outline-none focus:bg-gray-50"
                role="option"
              >
                {suggestion.icon && <div className="flex-shrink-0">{suggestion.icon}</div>}
                <span className="text-sm text-gray-700">{suggestion.text}</span>
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
