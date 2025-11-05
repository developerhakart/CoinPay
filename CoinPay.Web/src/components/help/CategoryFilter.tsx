import { useMemo } from 'react';
import { FAQ } from '@/data/faqData';

export interface Category {
  id: string;
  name: string;
  description?: string;
  icon?: React.ReactNode;
  count?: number;
}

interface CategoryFilterProps {
  /**
   * List of categories to display
   */
  categories: Category[];
  /**
   * Currently active category (or 'all')
   */
  activeCategory: string;
  /**
   * Callback when category is selected
   */
  onCategoryClick: (categoryId: string) => void;
  /**
   * FAQs to count per category (optional, for display)
   */
  faqs?: FAQ[];
  /**
   * Display as tabs or pills
   * @default 'pills'
   */
  variant?: 'tabs' | 'pills';
  /**
   * Additional CSS classes
   */
  className?: string;
  /**
   * Whether to show count badges
   */
  showCounts?: boolean;
}

/**
 * CategoryFilter - Category selection tabs/buttons with optional icons
 * Allows users to filter FAQs by category
 */
export function CategoryFilter({
  categories,
  activeCategory,
  onCategoryClick,
  faqs = [],
  variant = 'pills',
  className = '',
  showCounts = true,
}: CategoryFilterProps) {
  // Count items per category
  const categoryCounts = useMemo(() => {
    const counts: Record<string, number> = { all: faqs.length };
    faqs.forEach(faq => {
      counts[faq.category] = (counts[faq.category] || 0) + 1;
    });
    return counts;
  }, [faqs]);

  // Combine category info with counts
  const enrichedCategories = useMemo(
    () =>
      categories.map(cat => ({
        ...cat,
        count: showCounts ? categoryCounts[cat.id] : undefined,
      })),
    [categories, categoryCounts, showCounts]
  );

  if (variant === 'tabs') {
    return (
      <div className={`border-b border-gray-200 ${className}`} data-testid="category-filter">
        <nav className="flex gap-1" aria-label="Category filter">
          {enrichedCategories.map(category => (
            <button
              key={category.id}
              onClick={() => onCategoryClick(category.id)}
              className={`flex items-center gap-2 px-4 py-3 border-b-2 font-medium text-sm transition-colors ${
                activeCategory === category.id
                  ? 'border-primary-500 text-primary-600'
                  : 'border-transparent text-gray-600 hover:text-gray-900 hover:border-gray-300'
              }`}
              aria-current={activeCategory === category.id ? 'page' : undefined}
              data-testid={`category-${category.id}`}
            >
              {category.icon && <span className="flex-shrink-0">{category.icon}</span>}
              <span>{category.name}</span>
              {category.count !== undefined && (
                <span className="ml-1 text-xs px-2 py-0.5 bg-gray-100 rounded-full text-gray-600">
                  {category.count}
                </span>
              )}
            </button>
          ))}
        </nav>
      </div>
    );
  }

  // Pills variant (default)
  return (
    <div className={`flex flex-wrap gap-2 ${className}`} data-testid="category-filter">
      {enrichedCategories.map(category => (
        <button
          key={category.id}
          onClick={() => onCategoryClick(category.id)}
          className={`inline-flex items-center gap-2 px-4 py-2 rounded-full font-medium text-sm transition-all ${
            activeCategory === category.id
              ? 'bg-primary-500 text-white shadow-md hover:shadow-lg hover:bg-primary-600'
              : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50 hover:border-gray-400'
          }`}
          aria-current={activeCategory === category.id ? 'page' : undefined}
          data-testid={`category-${category.id}`}
          title={category.description}
        >
          {category.icon && <span className="flex-shrink-0 w-5 h-5">{category.icon}</span>}
          <span>{category.name}</span>
          {category.count !== undefined && (
            <span
              className={`ml-1 text-xs font-semibold px-2 py-0.5 rounded-full ${
                activeCategory === category.id ? 'bg-white/30 text-white' : 'bg-gray-100 text-gray-600'
              }`}
            >
              {category.count}
            </span>
          )}
        </button>
      ))}
    </div>
  );
}

/**
 * Default category definitions for CoinPay help center
 */
export const defaultCategories: Category[] = [
  {
    id: 'all',
    name: 'All Topics',
    description: 'View all help articles',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
      </svg>
    ),
  },
  {
    id: 'getting-started',
    name: 'Getting Started',
    description: 'Learn the basics and setup',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M13 10V3L4 14h7v7l9-11h-7z"
        />
      </svg>
    ),
  },
  {
    id: 'send-receive',
    name: 'Send & Receive',
    description: 'Transfer cryptocurrencies',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4"
        />
      </svg>
    ),
  },
  {
    id: 'swap',
    name: 'Swap Tokens',
    description: 'Exchange tokens',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M7 16V4m0 0L3 8m4-4l4 4m6 0v12m0 0l4-4m-4 4l-4-4"
        />
      </svg>
    ),
  },
  {
    id: 'investments',
    name: 'Investments',
    description: 'Investment strategies',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"
        />
      </svg>
    ),
  },
  {
    id: 'security',
    name: 'Security',
    description: 'Protect your account',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
        />
      </svg>
    ),
  },
  {
    id: 'troubleshooting',
    name: 'Troubleshooting',
    description: 'Solve common issues',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
        />
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
      </svg>
    ),
  },
];
