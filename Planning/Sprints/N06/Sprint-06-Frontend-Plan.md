# CoinPay Wallet MVP - Sprint N06 Frontend Plan

**Version**: 1.0
**Sprint Duration**: 2 weeks (10 working days)
**Sprint Dates**: March 17 - March 28, 2025
**Document Status**: Ready for Execution
**Last Updated**: 2025-11-05
**Owner**: Frontend Team Lead

---

## Executive Summary

### Sprint N06 Frontend Goal

**Primary Objective**: Deliver a polished, accessible, and production-ready user interface with comprehensive onboarding experience and cross-platform compatibility.

**Key Focus Areas**:
1. User Onboarding & Help (Welcome wizard, FAQ, guided tours)
2. UI/UX Polish (Consistency, error messages, loading states)
3. Cross-Browser & Accessibility (4 browsers, WCAG 2.1 AA)
4. Performance & Build (Optimization, code splitting, CDN)

**Expected Outcomes**:
- Complete user onboarding experience
- Accessibility score > 95 (Lighthouse)
- Cross-browser compatibility verified
- Production build optimized
- Mobile-responsive and performant

---

## Team Capacity

### Frontend Team Composition

| Agent | Specialization | Capacity (days) | Allocation |
|-------|---------------|-----------------|------------|
| **FE-1** | UI Components | 10 | UI polish, responsive design, mobile optimization |
| **FE-2** | Integration | 10 | Onboarding, help pages, build optimization |
| **FE-3** | Optimization | 10 | Accessibility, cross-browser, performance |
| **Total** | | **30** | **20 days planned (67%)** |

**Buffer**: 10 days (33%) for iterations and bug fixes

---

## Sprint N06 Frontend Tasks

### Epic 1: User Onboarding & Help (7.00 days)

#### FE-601: User Onboarding Wizard (3-step) - 3.00 days

**Objective**: Create an intuitive onboarding experience for new users.

**Scope**:
- Welcome screen with value proposition
- 3-step wizard for account setup
- Feature highlights and tutorials
- Final confirmation and dashboard redirect

**Owner**: FE-2

**Wizard Steps**:

**Step 1: Welcome & Introduction**
- Welcome message
- Key features overview (Send, Swap, Invest)
- Benefits of using CoinPay
- "Get Started" CTA

**Step 2: Wallet Setup**
- Create wallet explanation
- Security best practices
- Backup and recovery information
- Test deposit suggestion

**Step 3: Feature Tour**
- Send funds walkthrough
- Swap tokens explanation
- Investment opportunities overview
- Support and help resources

**Acceptance Criteria**:
- [ ] 3-step wizard complete and functional
- [ ] Responsive design (mobile and desktop)
- [ ] Skip option available
- [ ] Progress indicator visible
- [ ] Smooth transitions between steps
- [ ] Final step redirects to dashboard
- [ ] User preferences saved (don't show again)
- [ ] Accessibility compliant (keyboard navigation)
- [ ] Code review completed

**Dependencies**: None

**Testing**:
- Manual testing on desktop and mobile
- Accessibility testing (keyboard, screen reader)
- User acceptance testing

**Implementation Details**:
```typescript
// OnboardingWizard.tsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface OnboardingStep {
  title: string;
  description: string;
  component: React.ComponentType;
}

export const OnboardingWizard: React.FC = () => {
  const [currentStep, setCurrentStep] = useState(0);
  const navigate = useNavigate();

  const steps: OnboardingStep[] = [
    { title: 'Welcome', description: 'Get started with CoinPay', component: WelcomeStep },
    { title: 'Wallet Setup', description: 'Create your wallet', component: WalletSetupStep },
    { title: 'Feature Tour', description: 'Explore features', component: FeatureTourStep },
  ];

  const handleNext = () => {
    if (currentStep < steps.length - 1) {
      setCurrentStep(prev => prev + 1);
    } else {
      handleComplete();
    }
  };

  const handleComplete = () => {
    localStorage.setItem('onboardingCompleted', 'true');
    navigate('/dashboard');
  };

  const handleSkip = () => {
    localStorage.setItem('onboardingCompleted', 'true');
    navigate('/dashboard');
  };

  return (
    <div className="onboarding-wizard">
      <ProgressBar current={currentStep + 1} total={steps.length} />
      <CurrentStepComponent />
      <Navigation onNext={handleNext} onSkip={handleSkip} />
    </div>
  );
};
```

---

#### FE-602: Help & FAQ Pages - 2.00 days

**Objective**: Provide comprehensive help documentation and FAQs.

**Scope**:
- Help center landing page
- FAQ sections (categorized)
- Search functionality
- Contact support form

**Owner**: FE-2

**Help Center Sections**:

1. **Getting Started**
   - Creating your wallet
   - Making your first deposit
   - Understanding your dashboard
   - Security best practices

2. **Send & Receive**
   - How to send funds
   - How to receive funds
   - Transaction fees explained
   - Transaction status tracking

3. **Swap Tokens**
   - How to swap tokens
   - Understanding slippage
   - Swap fees explained
   - Supported tokens

4. **Investments**
   - Connecting WhiteBit
   - Creating investments
   - Understanding APY and rewards
   - Withdrawing investments

5. **Security**
   - Account security
   - Two-factor authentication
   - Recognizing phishing attempts
   - Reporting security issues

6. **Troubleshooting**
   - Common errors and solutions
   - Transaction stuck or failed
   - Wallet connection issues
   - Contact support

**Acceptance Criteria**:
- [ ] Help center page with categorized sections
- [ ] 20+ FAQ entries across all categories
- [ ] Search functionality for FAQs
- [ ] Contact support form
- [ ] Responsive design
- [ ] Easy navigation between sections
- [ ] Code review completed

**Dependencies**: None

**Testing**:
- Manual testing
- Search functionality testing
- Mobile responsiveness testing

**Implementation Details**:
```typescript
// HelpPage.tsx
interface FAQ {
  id: string;
  category: string;
  question: string;
  answer: string;
  tags: string[];
}

export const HelpPage: React.FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  const categories = ['Getting Started', 'Send & Receive', 'Swap', 'Investments', 'Security'];

  const filteredFAQs = useMemo(() => {
    return faqs.filter(faq => {
      const matchesSearch = faq.question.toLowerCase().includes(searchQuery.toLowerCase()) ||
                           faq.answer.toLowerCase().includes(searchQuery.toLowerCase());
      const matchesCategory = !selectedCategory || faq.category === selectedCategory;
      return matchesSearch && matchesCategory;
    });
  }, [searchQuery, selectedCategory]);

  return (
    <div className="help-page">
      <SearchBar value={searchQuery} onChange={setSearchQuery} />
      <CategoryFilter categories={categories} selected={selectedCategory} onChange={setSelectedCategory} />
      <FAQList faqs={filteredFAQs} />
      <ContactSupport />
    </div>
  );
};
```

---

#### FE-603: Feature Guided Tours (Tooltips) - 2.00 days

**Objective**: Implement contextual guided tours for key features.

**Scope**:
- Tooltip-based feature tours
- Highlight key UI elements
- Step-by-step guidance
- Dismissible and resumable tours

**Owner**: FE-1

**Guided Tours**:

1. **Wallet Dashboard Tour** (5 steps)
   - Balance overview
   - Recent transactions
   - Quick actions (Send, Swap, Invest)
   - Navigation menu
   - Help and settings

2. **Send Money Tour** (4 steps)
   - Enter recipient address
   - Select token and amount
   - Review transaction details
   - Confirm and send

3. **Swap Tokens Tour** (5 steps)
   - Select tokens to swap
   - Enter swap amount
   - Review exchange rate and fees
   - Adjust slippage tolerance
   - Execute swap

4. **Investment Tour** (4 steps)
   - Connect exchange
   - Browse investment plans
   - Create investment
   - Track positions

**Acceptance Criteria**:
- [ ] Guided tours for 4 key features
- [ ] Tooltip library integrated (e.g., react-joyride)
- [ ] Tours dismissible and resumable
- [ ] Progress saved to localStorage
- [ ] Accessible (keyboard navigation)
- [ ] Mobile-friendly tooltips
- [ ] Code review completed

**Dependencies**: None

**Testing**:
- Manual testing on all tours
- Accessibility testing
- Mobile responsiveness testing

**Implementation Details**:
```typescript
// Using react-joyride
import Joyride, { Step } from 'react-joyride';

const walletDashboardSteps: Step[] = [
  {
    target: '.wallet-balance',
    content: 'Your total wallet balance is displayed here. You can see balances for USDC, ETH, and MATIC.',
    placement: 'bottom',
  },
  {
    target: '.recent-transactions',
    content: 'View your recent transactions here. Click on any transaction to see details.',
    placement: 'top',
  },
  // ... more steps
];

export const WalletDashboardWithTour: React.FC = () => {
  const [runTour, setRunTour] = useState(false);

  useEffect(() => {
    const hasSeenTour = localStorage.getItem('walletDashboardTourCompleted');
    if (!hasSeenTour) {
      setRunTour(true);
    }
  }, []);

  const handleTourEnd = () => {
    localStorage.setItem('walletDashboardTourCompleted', 'true');
    setRunTour(false);
  };

  return (
    <>
      <Joyride
        steps={walletDashboardSteps}
        run={runTour}
        continuous
        showProgress
        showSkipButton
        callback={handleTourEnd}
      />
      <WalletDashboard />
    </>
  );
};
```

---

### Epic 2: UI/UX Polish (6.00 days)

#### FE-604: UI Consistency Audit & Fixes - 2.00 days

**Objective**: Ensure consistent UI/UX across all pages.

**Scope**:
- Spacing and layout consistency
- Color scheme consistency
- Typography consistency
- Button and form styles
- Icon usage consistency

**Owner**: FE-1

**Audit Areas**:

1. **Spacing & Layout**
   - Consistent padding/margin (4px, 8px, 16px, 24px, 32px)
   - Consistent grid layouts
   - Consistent card spacing
   - Responsive breakpoints

2. **Colors**
   - Primary, secondary, accent colors
   - Text colors (primary, secondary, muted)
   - Background colors
   - Error, warning, success colors

3. **Typography**
   - Heading sizes (h1-h6)
   - Body text sizes
   - Font weights
   - Line heights

4. **Components**
   - Button variants (primary, secondary, outline, ghost)
   - Input field styles
   - Modal styles
   - Card styles
   - Badge styles

**Acceptance Criteria**:
- [ ] UI audit document created
- [ ] Inconsistencies identified and catalogued
- [ ] All inconsistencies fixed
- [ ] Design system documentation updated
- [ ] Code review completed

**Dependencies**: None

**Testing**:
- Visual regression testing
- Cross-page consistency review

**Implementation Approach**:
```typescript
// Tailwind config for consistent spacing
module.exports = {
  theme: {
    spacing: {
      '0': '0px',
      '1': '4px',
      '2': '8px',
      '3': '12px',
      '4': '16px',
      '6': '24px',
      '8': '32px',
      '12': '48px',
      '16': '64px',
    },
    colors: {
      primary: {
        50: '#f0f9ff',
        // ... full palette
        900: '#0c4a6e',
      },
      // ... other colors
    },
  },
};

// Consistent button component
export const Button: React.FC<ButtonProps> = ({ variant = 'primary', size = 'md', ...props }) => {
  const baseClasses = 'rounded-lg font-medium transition-colors focus:outline-none focus:ring-2';

  const variantClasses = {
    primary: 'bg-primary-600 text-white hover:bg-primary-700',
    secondary: 'bg-gray-200 text-gray-900 hover:bg-gray-300',
    outline: 'border border-primary-600 text-primary-600 hover:bg-primary-50',
  };

  const sizeClasses = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2 text-base',
    lg: 'px-6 py-3 text-lg',
  };

  return (
    <button className={cn(baseClasses, variantClasses[variant], sizeClasses[size])} {...props} />
  );
};
```

---

#### FE-605: Error Message Improvements - 1.50 days

**Objective**: Improve error messaging for better user experience.

**Scope**:
- User-friendly error messages
- Actionable error guidance
- Consistent error display
- Error tracking and logging

**Owner**: FE-2

**Error Message Guidelines**:

1. **Clear and Specific**
   - ❌ "An error occurred"
   - ✅ "Unable to send transaction. Insufficient USDC balance."

2. **Actionable**
   - ❌ "Transaction failed"
   - ✅ "Transaction failed. Please check your wallet balance and try again."

3. **Empathetic Tone**
   - ❌ "Invalid input"
   - ✅ "It looks like the recipient address is invalid. Please check and try again."

4. **Include Help Links**
   - "Need help? [Contact support](/help/contact)"

**Error Categories**:

1. **Validation Errors**
   - Form field validation
   - Input format errors
   - Required field errors

2. **Network Errors**
   - API timeout
   - Connection lost
   - Service unavailable

3. **Transaction Errors**
   - Insufficient balance
   - Transaction failed
   - Gas estimation failed

4. **Authentication Errors**
   - Session expired
   - Unauthorized access
   - Invalid credentials

**Acceptance Criteria**:
- [ ] Error message mapping document created
- [ ] All error messages updated
- [ ] Error display component improved
- [ ] Error logging implemented
- [ ] Code review completed

**Dependencies**: None

**Implementation Details**:
```typescript
// Error message service
const ERROR_MESSAGES: Record<string, string> = {
  INSUFFICIENT_BALANCE: 'You don\'t have enough {token} to complete this transaction. Please add funds to your wallet.',
  INVALID_ADDRESS: 'The recipient address appears to be invalid. Please check the address and try again.',
  TRANSACTION_FAILED: 'Your transaction couldn\'t be processed. Please try again in a few moments.',
  NETWORK_ERROR: 'We\'re having trouble connecting to the network. Please check your connection and try again.',
  SESSION_EXPIRED: 'Your session has expired. Please log in again to continue.',
};

export const getErrorMessage = (errorCode: string, context?: Record<string, string>): string => {
  let message = ERROR_MESSAGES[errorCode] || 'Something went wrong. Please try again.';

  if (context) {
    Object.entries(context).forEach(([key, value]) => {
      message = message.replace(`{${key}}`, value);
    });
  }

  return message;
};

// Error display component
export const ErrorAlert: React.FC<{ error: string; onRetry?: () => void }> = ({ error, onRetry }) => {
  return (
    <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-start gap-3">
      <AlertCircleIcon className="text-red-600 flex-shrink-0" />
      <div className="flex-1">
        <p className="text-red-900 font-medium">Error</p>
        <p className="text-red-700 text-sm mt-1">{error}</p>
        {onRetry && (
          <button onClick={onRetry} className="text-red-600 hover:text-red-800 text-sm font-medium mt-2">
            Try again
          </button>
        )}
        <a href="/help" className="text-red-600 hover:text-red-800 text-sm font-medium mt-2 ml-4">
          Need help?
        </a>
      </div>
    </div>
  );
};
```

---

#### FE-606: Loading State Consistency - 1.00 day

**Objective**: Ensure consistent loading states across the application.

**Scope**:
- Loading spinners
- Skeleton screens
- Progress indicators
- Loading text consistency

**Owner**: FE-1

**Loading State Patterns**:

1. **Skeleton Screens** (for initial page load)
   - Dashboard cards
   - Transaction list
   - Investment positions
   - User profile

2. **Spinners** (for button actions)
   - Submit buttons
   - Load more buttons
   - Refresh buttons

3. **Progress Indicators** (for multi-step processes)
   - Transaction status
   - Swap execution
   - Investment creation

**Acceptance Criteria**:
- [ ] Loading component library created
- [ ] Skeleton screens for all major views
- [ ] Loading spinners standardized
- [ ] Progress indicators consistent
- [ ] Loading text consistent
- [ ] Code review completed

**Dependencies**: None

**Implementation Details**:
```typescript
// Skeleton components
export const CardSkeleton: React.FC = () => (
  <div className="animate-pulse bg-white rounded-lg p-6 shadow">
    <div className="h-4 bg-gray-200 rounded w-3/4 mb-4" />
    <div className="h-8 bg-gray-200 rounded w-1/2 mb-2" />
    <div className="h-4 bg-gray-200 rounded w-full" />
  </div>
);

export const TransactionListSkeleton: React.FC = () => (
  <div className="space-y-3">
    {[...Array(5)].map((_, i) => (
      <div key={i} className="animate-pulse flex items-center gap-4 p-4 bg-white rounded-lg">
        <div className="w-10 h-10 bg-gray-200 rounded-full" />
        <div className="flex-1">
          <div className="h-4 bg-gray-200 rounded w-1/3 mb-2" />
          <div className="h-3 bg-gray-200 rounded w-1/4" />
        </div>
        <div className="h-6 bg-gray-200 rounded w-16" />
      </div>
    ))}
  </div>
);

// Loading button
export const LoadingButton: React.FC<ButtonProps & { loading?: boolean }> = ({
  loading,
  children,
  disabled,
  ...props
}) => (
  <Button disabled={loading || disabled} {...props}>
    {loading ? (
      <><Spinner className="mr-2" /> Processing...</>
    ) : children}
  </Button>
);
```

---

#### FE-607: Responsive Design Fixes - 1.50 days

**Objective**: Fix responsive design issues across all breakpoints.

**Scope**:
- Mobile layout fixes (< 768px)
- Tablet layout fixes (768px - 1024px)
- Desktop optimization (> 1024px)
- Touch target sizes
- Horizontal scrolling issues

**Owner**: FE-1

**Breakpoints**:
- **Mobile**: < 768px
- **Tablet**: 768px - 1024px
- **Desktop**: > 1024px

**Focus Areas**:

1. **Navigation**
   - Mobile hamburger menu
   - Tablet navigation
   - Desktop full menu

2. **Forms**
   - Input field sizing
   - Button placement
   - Label positioning
   - Error message display

3. **Tables & Lists**
   - Horizontal scrolling on mobile
   - Card layout on small screens
   - Column hiding on mobile

4. **Modals**
   - Full-screen on mobile
   - Centered on desktop
   - Proper spacing

**Acceptance Criteria**:
- [ ] All pages tested on mobile, tablet, desktop
- [ ] No horizontal scrolling on mobile
- [ ] Touch targets >= 44px
- [ ] Text readable on all screen sizes
- [ ] Images responsive
- [ ] Code review completed

**Dependencies**:
- QA-605: Mobile testing

**Testing**:
- Chrome DevTools device emulation
- Real device testing (iOS + Android)
- Responsive design testing tool

---

### Epic 3: Cross-Browser & Accessibility (4.00 days)

#### FE-608: Cross-Browser Compatibility Testing - 1.50 days

**Objective**: Ensure compatibility across major browsers.

**Scope**:
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

**Owner**: FE-3

**Testing Areas**:

1. **Layout & Styling**
   - CSS Grid/Flexbox
   - Custom fonts
   - Animations and transitions

2. **JavaScript Functionality**
   - ES6+ features
   - Async/await
   - DOM manipulation

3. **API Compatibility**
   - Fetch API
   - LocalStorage
   - WebCrypto

4. **Form Handling**
   - Input validation
   - Autofill
   - Form submission

**Acceptance Criteria**:
- [ ] Testing completed on 4 browsers
- [ ] Browser compatibility issues documented
- [ ] All critical issues fixed
- [ ] Polyfills added where needed
- [ ] Code review completed

**Dependencies**: None

**Testing Tools**:
- BrowserStack
- Manual testing on each browser

---

#### FE-609: Accessibility Improvements (WCAG 2.1 AA) - 2.00 days

**Objective**: Achieve WCAG 2.1 AA compliance and Lighthouse accessibility score > 95.

**Scope**:
- Keyboard navigation
- Screen reader support
- Color contrast
- ARIA labels and roles
- Focus management

**Owner**: FE-3

**WCAG 2.1 AA Requirements**:

1. **Perceivable**
   - Text alternatives for images
   - Captions and transcripts for media
   - Content adaptable to different presentations
   - Color contrast >= 4.5:1

2. **Operable**
   - Keyboard accessible
   - No keyboard traps
   - Skip navigation links
   - Descriptive page titles
   - Visible focus indicator

3. **Understandable**
   - Readable text (language attribute)
   - Predictable navigation
   - Input assistance (labels, errors, suggestions)
   - Error identification and recovery

4. **Robust**
   - Valid HTML
   - ARIA roles and attributes
   - Compatible with assistive technologies

**Acceptance Criteria**:
- [ ] Lighthouse accessibility score > 95
- [ ] All images have alt text
- [ ] Keyboard navigation works on all pages
- [ ] Screen reader tested (NVDA/JAWS)
- [ ] Color contrast >= 4.5:1
- [ ] ARIA labels on interactive elements
- [ ] Focus indicators visible
- [ ] Code review completed

**Dependencies**:
- QA-609: Accessibility audit

**Testing Tools**:
- Lighthouse
- axe DevTools
- NVDA screen reader
- Keyboard-only navigation testing

**Implementation Examples**:
```typescript
// Proper button with ARIA
<button
  aria-label="Send USDC to recipient"
  aria-describedby="send-button-description"
  onClick={handleSend}
>
  Send
</button>
<span id="send-button-description" className="sr-only">
  This will send USDC from your wallet to the recipient address
</span>

// Skip navigation link
<a href="#main-content" className="sr-only focus:not-sr-only">
  Skip to main content
</a>

// Proper form labels
<label htmlFor="recipient-address">
  Recipient Address
</label>
<input
  id="recipient-address"
  type="text"
  aria-required="true"
  aria-invalid={errors.address ? 'true' : 'false'}
  aria-describedby={errors.address ? 'address-error' : undefined}
/>
{errors.address && (
  <span id="address-error" className="text-red-600" role="alert">
    {errors.address}
  </span>
)}
```

---

#### FE-610: Mobile Optimization (iOS + Android) - 0.50 days

**Objective**: Optimize performance and UX for mobile devices.

**Scope**:
- Touch interactions
- Mobile performance
- Mobile-specific UI adjustments
- PWA optimizations

**Owner**: FE-1

**Optimization Areas**:

1. **Touch Interactions**
   - Touch target size >= 44px
   - Touch feedback (active states)
   - Gesture support (swipe, pinch)
   - Prevent zoom on input focus

2. **Performance**
   - Lazy load images
   - Code splitting
   - Reduce bundle size
   - Optimize rendering

3. **Mobile UI**
   - Bottom navigation
   - Fixed headers
   - Full-screen modals
   - Mobile-optimized forms

**Acceptance Criteria**:
- [ ] Touch targets >= 44px
- [ ] Mobile performance score > 90 (Lighthouse)
- [ ] Tested on iOS and Android devices
- [ ] No layout shifts
- [ ] Fast tap response
- [ ] Code review completed

**Dependencies**:
- QA-608: Mobile testing

---

### Epic 4: Performance & Build (3.00 days)

#### FE-611: Production Build Optimization - 1.50 days

**Objective**: Optimize production build for performance.

**Scope**:
- Bundle size optimization
- Tree shaking
- Minification
- Source map configuration

**Owner**: FE-2

**Optimization Strategies**:

1. **Bundle Analysis**
   - Identify large dependencies
   - Remove unused code
   - Replace heavy libraries

2. **Code Splitting**
   - Route-based splitting
   - Component-based splitting
   - Vendor chunk optimization

3. **Minification**
   - JavaScript minification
   - CSS minification
   - HTML minification

**Acceptance Criteria**:
- [ ] Bundle size < 500KB (gzipped)
- [ ] First contentful paint < 1.5s
- [ ] Time to interactive < 3s
- [ ] Lighthouse performance > 90
- [ ] Code review completed

**Dependencies**: None

**Implementation**:
```javascript
// vite.config.ts
export default defineConfig({
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          'react-vendor': ['react', 'react-dom', 'react-router-dom'],
          'ui-vendor': ['@headlessui/react', '@heroicons/react'],
          'tanstack': ['@tanstack/react-query'],
        },
      },
    },
    chunkSizeWarningLimit: 500,
    minify: 'terser',
    terserOptions: {
      compress: {
        drop_console: true,
      },
    },
  },
});
```

---

#### FE-612: Code Splitting & Lazy Loading - 1.00 day

**Objective**: Implement code splitting for faster initial load.

**Scope**:
- Route-based code splitting
- Component lazy loading
- Dynamic imports

**Owner**: FE-2

**Implementation Strategy**:

1. **Route-Level Splitting**
   ```typescript
   const Dashboard = lazy(() => import('./pages/Dashboard'));
   const SwapPage = lazy(() => import('./pages/SwapPage'));
   const InvestmentPage = lazy(() => import('./pages/InvestmentPage'));

   <Routes>
     <Route path="/dashboard" element={<Suspense fallback={<PageSkeleton />}><Dashboard /></Suspense>} />
     <Route path="/swap" element={<Suspense fallback={<PageSkeleton />}><SwapPage /></Suspense>} />
   </Routes>
   ```

2. **Component-Level Splitting**
   ```typescript
   const HeavyChart = lazy(() => import('./components/HeavyChart'));

   {showChart && (
     <Suspense fallback={<ChartSkeleton />}>
       <HeavyChart data={data} />
     </Suspense>
   )}
   ```

**Acceptance Criteria**:
- [ ] All routes lazy loaded
- [ ] Heavy components lazy loaded
- [ ] Loading fallbacks implemented
- [ ] Bundle chunks optimized
- [ ] Code review completed

**Dependencies**: None

---

#### FE-613: Image Optimization & CDN Setup - 0.50 days

**Objective**: Optimize images and setup CDN for static assets.

**Scope**:
- Image compression
- Modern image formats (WebP)
- Lazy loading images
- CDN configuration

**Owner**: FE-1

**Implementation**:

1. **Image Optimization**
   - Convert images to WebP
   - Provide fallbacks
   - Use responsive images
   - Lazy load images

2. **CDN Setup**
   - Configure CDN for static assets
   - Cache static resources
   - Setup cache headers

**Acceptance Criteria**:
- [ ] All images optimized
- [ ] WebP format with fallbacks
- [ ] Images lazy loaded
- [ ] CDN configured
- [ ] Code review completed

**Dependencies**: None

---

## Task Dependencies

```
Week 1:
  Day 1-3: FE-601, FE-602 (Onboarding & Help)
  Day 3-4: FE-603, FE-604 (Tours & UI Audit)
  Day 4-5: FE-605, FE-608 (Errors & Cross-browser)

Week 2:
  Day 6-7: FE-606, FE-607, FE-609 (Loading, Responsive, Accessibility)
  Day 8-9: FE-610, FE-611, FE-612, FE-613 (Mobile, Performance)
  Day 10:  Code review, final testing
```

---

## Testing Strategy

### Component Testing

**Framework**: Vitest + Testing Library

**Coverage**: All new components

**Example**:
```typescript
import { render, screen, fireEvent } from '@testing-library/react';
import { OnboardingWizard } from './OnboardingWizard';

describe('OnboardingWizard', () => {
  it('should render welcome step initially', () => {
    render(<OnboardingWizard />);
    expect(screen.getByText(/welcome/i)).toBeInTheDocument();
  });

  it('should navigate to next step on button click', () => {
    render(<OnboardingWizard />);
    fireEvent.click(screen.getByText(/next/i));
    expect(screen.getByText(/wallet setup/i)).toBeInTheDocument();
  });
});
```

---

### Accessibility Testing

**Tools**:
- Lighthouse
- axe DevTools
- NVDA screen reader

**Manual Testing**:
- Keyboard-only navigation
- Screen reader testing
- Color contrast checking

---

### Performance Testing

**Metrics**:
- First Contentful Paint < 1.5s
- Time to Interactive < 3s
- Lighthouse Performance > 90

**Tools**:
- Lighthouse
- WebPageTest
- Chrome DevTools Performance panel

---

## Definition of Done

### Code Quality
- [ ] All code reviewed and approved
- [ ] TypeScript strict mode (no `any` types)
- [ ] ESLint warnings resolved
- [ ] Consistent code style

### Testing
- [ ] Component tests passing
- [ ] Manual testing completed
- [ ] Accessibility testing passed
- [ ] Cross-browser testing passed

### Performance
- [ ] Lighthouse performance > 90
- [ ] Bundle size < 500KB (gzipped)
- [ ] First contentful paint < 1.5s
- [ ] Time to interactive < 3s

### Accessibility
- [ ] Lighthouse accessibility > 95
- [ ] WCAG 2.1 AA compliant
- [ ] Keyboard navigation working
- [ ] Screen reader tested

---

## Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Lighthouse Performance | > 90 | Lighthouse audit |
| Lighthouse Accessibility | > 95 | Lighthouse audit |
| Bundle size (gzipped) | < 500KB | Build analysis |
| First contentful paint | < 1.5s | Lighthouse |
| Time to interactive | < 3s | Lighthouse |
| Cross-browser compatibility | 100% | Manual testing |
| Mobile responsiveness | 100% | Device testing |

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-05 | Frontend Team Lead | Initial Sprint N06 Frontend Plan |

---

**End of Sprint N06 Frontend Plan**
