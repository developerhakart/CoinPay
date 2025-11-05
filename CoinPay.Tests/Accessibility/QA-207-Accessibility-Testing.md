# QA-207: Accessibility Testing (WCAG 2.1 AA)

**Test Owner**: QA Engineer 1
**Effort**: 2.00 days
**Status**: Ready for Execution
**Date Created**: 2025-10-29
**Priority**: HIGH

---

## Test Objectives

Ensure CoinPay is accessible to users with disabilities:
- WCAG 2.1 Level AA compliance
- Lighthouse accessibility score > 90
- Screen reader compatibility
- Keyboard navigation support
- Color contrast compliance
- Focus management

---

## WCAG 2.1 Principles (POUR)

### 1. Perceivable
Users must be able to perceive the information being presented

### 2. Operable
Users must be able to operate the interface

### 3. Understandable
Users must be able to understand the information and interface

### 4. Robust
Content must be robust enough to be interpreted by assistive technologies

---

## WCAG 2.1 Level AA Checklist

### Principle 1: Perceivable

#### 1.1 Text Alternatives

##### 1.1.1 Non-text Content (A)
**Requirement**: All non-text content has text alternatives

**Test Cases**:
- [ ] All images have alt text
- [ ] Icons have aria-label or aria-labelledby
- [ ] Decorative images have `alt=""` or `role="presentation"`
- [ ] Form inputs have associated labels
- [ ] Buttons have descriptive text or aria-label

**Pages to Test**:
- /dashboard
- /wallet
- /transfer
- /transactions

**Test Procedure**:
1. Inspect each image in DevTools
2. Verify alt attribute present and descriptive
3. Use screen reader to verify text announced
4. Check ARIA labels on icon buttons

**Status**: ⏳ Pending

---

#### 1.2 Time-based Media

##### 1.2.1 Audio-only and Video-only (A)
**Requirement**: Provide alternatives for audio/video content

**Assessment**: N/A - CoinPay has no audio/video content

**Status**: ✅ N/A

---

#### 1.3 Adaptable

##### 1.3.1 Info and Relationships (A)
**Requirement**: Information, structure, and relationships can be programmatically determined

**Test Cases**:
- [ ] Proper heading hierarchy (h1 → h2 → h3)
- [ ] Lists use ul/ol elements
- [ ] Tables use proper table markup (th, tbody, etc.)
- [ ] Form labels associated with inputs
- [ ] Landmark regions (header, nav, main, footer)

**Heading Hierarchy Test**:
```
Page: /dashboard
- h1: "Dashboard"
  - h2: "Wallet Balance"
  - h2: "Recent Transactions"
    - h3: "Transaction #123"
```

**Tools**: Accessibility Insights, HeadingsMap extension

**Status**: ⏳ Pending

---

##### 1.3.2 Meaningful Sequence (A)
**Requirement**: Content appears in meaningful order

**Test Cases**:
- [ ] Reading order matches visual order
- [ ] Tab order is logical
- [ ] CSS positioning doesn't break reading order

**Test Procedure**:
1. Disable CSS in browser
2. Verify content still makes sense
3. Tab through page and verify logical order

**Status**: ⏳ Pending

---

##### 1.3.3 Sensory Characteristics (A)
**Requirement**: Instructions don't rely solely on sensory characteristics

**Test Cases**:
- [ ] Instructions don't say "click the red button" (also provide text)
- [ ] Instructions don't say "on the right side" (also provide label)
- [ ] Form errors identified by text, not just color

**Examples to Check**:
- Error messages (not just red text)
- Success messages (not just green checkmark)
- Status indicators (Pending, Completed, Failed)

**Status**: ⏳ Pending

---

##### 1.3.4 Orientation (AA)
**Requirement**: Content not restricted to single orientation

**Test Cases**:
- [ ] Works in portrait and landscape
- [ ] No orientation lock (unless essential)
- [ ] Responsive design adapts to orientation

**Test on**:
- Mobile devices (iPhone, Android)
- Tablets (iPad)
- Rotate device to test both orientations

**Status**: ⏳ Pending

---

##### 1.3.5 Identify Input Purpose (AA)
**Requirement**: Input fields have autocomplete attributes

**Test Cases**:
- [ ] Email field: `autocomplete="email"`
- [ ] Name field: `autocomplete="name"`
- [ ] Address field: `autocomplete="address"`

**Code Example**:
```html
<input
  type="email"
  name="email"
  autocomplete="email"
  aria-label="Email address"
/>
```

**Status**: ⏳ Pending

---

#### 1.4 Distinguishable

##### 1.4.1 Use of Color (A)
**Requirement**: Color is not the only means of conveying information

**Test Cases**:
- [ ] Form errors shown with text AND color
- [ ] Status badges have text labels (not just colors)
- [ ] Links distinguishable by underline (not just color)
- [ ] Charts have patterns or labels (not just colors)

**Examples**:
- ❌ Red text for error (color only)
- ✅ Red text + "Error:" prefix + icon

**Status**: ⏳ Pending

---

##### 1.4.2 Audio Control (A)
**Requirement**: Audio can be paused/stopped

**Assessment**: N/A - No auto-playing audio

**Status**: ✅ N/A

---

##### 1.4.3 Contrast (Minimum) (AA)
**Requirement**: Text has contrast ratio of at least 4.5:1 (3:1 for large text)

**Test Cases**:
- [ ] Body text: 4.5:1 contrast ratio
- [ ] Large text (18pt+): 3:1 contrast ratio
- [ ] Button text: 4.5:1 contrast ratio
- [ ] Link text: 4.5:1 contrast ratio
- [ ] Placeholder text: 4.5:1 contrast ratio

**Colors to Test**:
- Primary: Indigo (#6366f1) on white
- Success: Green (#10b981) on white
- Error: Red (#ef4444) on white
- Text: Gray (#1f2937) on white

**Tools**: WebAIM Contrast Checker, Lighthouse

**Test Results**:
- Text on white background: ____:1 (Pass/Fail)
- Button text: ____:1 (Pass/Fail)
- Link text: ____:1 (Pass/Fail)

**Status**: ⏳ Pending

---

##### 1.4.4 Resize Text (AA)
**Requirement**: Text can be resized up to 200% without loss of functionality

**Test Cases**:
- [ ] Zoom to 200% in browser
- [ ] All text remains readable
- [ ] No horizontal scrolling
- [ ] Buttons still clickable
- [ ] Forms still usable

**Test Procedure**:
1. Set browser zoom to 200% (Ctrl + + or Cmd + +)
2. Navigate all pages
3. Verify layout doesn't break
4. Test all interactive elements

**Status**: ⏳ Pending

---

##### 1.4.5 Images of Text (AA)
**Requirement**: Use actual text instead of images of text

**Test Cases**:
- [ ] Headings are HTML text (not images)
- [ ] Buttons use HTML text (not images)
- [ ] Only logos/branding use images of text

**Exceptions**: Logos, essential visuals

**Status**: ⏳ Pending

---

##### 1.4.10 Reflow (AA)
**Requirement**: Content reflows without horizontal scrolling at 320px width

**Test Cases**:
- [ ] Mobile view (320px) has no horizontal scroll
- [ ] Content stacks vertically on narrow screens
- [ ] All functionality available on mobile

**Test Procedure**:
1. Open DevTools responsive mode
2. Set width to 320px
3. Navigate all pages
4. Verify no horizontal scrolling

**Status**: ⏳ Pending

---

##### 1.4.11 Non-text Contrast (AA)
**Requirement**: UI components have 3:1 contrast ratio

**Test Cases**:
- [ ] Button borders: 3:1 contrast
- [ ] Input borders: 3:1 contrast
- [ ] Focus indicators: 3:1 contrast
- [ ] Icon buttons: 3:1 contrast

**Status**: ⏳ Pending

---

##### 1.4.12 Text Spacing (AA)
**Requirement**: Content adapts to increased text spacing

**Test Cases**:
- [ ] Line height 1.5x font size
- [ ] Paragraph spacing 2x font size
- [ ] Letter spacing 0.12x font size
- [ ] Word spacing 0.16x font size

**Test with CSS**:
```css
* {
  line-height: 1.5 !important;
  letter-spacing: 0.12em !important;
  word-spacing: 0.16em !important;
}

p {
  margin-bottom: 2em !important;
}
```

**Status**: ⏳ Pending

---

##### 1.4.13 Content on Hover or Focus (AA)
**Requirement**: Content that appears on hover/focus is dismissible and persistent

**Test Cases**:
- [ ] Tooltips remain visible when hovering over them
- [ ] Tooltips can be dismissed with Escape key
- [ ] Tooltips don't obscure other content

**Status**: ⏳ Pending

---

### Principle 2: Operable

#### 2.1 Keyboard Accessible

##### 2.1.1 Keyboard (A)
**Requirement**: All functionality available via keyboard

**Test Cases**:
- [ ] Can navigate entire app with Tab/Shift+Tab
- [ ] Can activate buttons with Enter/Space
- [ ] Can close modals with Escape
- [ ] Can use dropdowns with arrow keys
- [ ] No keyboard traps

**Test Procedure**:
1. Unplug mouse
2. Navigate app using only keyboard
3. Complete full user workflow (login → transfer → logout)
4. Document any issues

**Keyboard Shortcuts**:
- Tab: Next element
- Shift+Tab: Previous element
- Enter: Activate button/link
- Space: Activate button/checkbox
- Escape: Close modal/dialog
- Arrow keys: Navigate menus/dropdowns

**Status**: ⏳ Pending

---

##### 2.1.2 No Keyboard Trap (A)
**Requirement**: Keyboard focus is never trapped

**Test Cases**:
- [ ] Can Tab out of all modals
- [ ] Can Tab out of all forms
- [ ] Can Tab out of all dropdowns
- [ ] Can Escape from modal dialogs

**Status**: ⏳ Pending

---

##### 2.1.4 Character Key Shortcuts (A)
**Requirement**: Single-key shortcuts can be disabled or remapped

**Assessment**: CoinPay doesn't use single-key shortcuts

**Status**: ✅ N/A

---

#### 2.2 Enough Time

##### 2.2.1 Timing Adjustable (A)
**Requirement**: Users can extend time limits

**Test Cases**:
- [ ] Session timeout has warning before expiry
- [ ] Users can extend session
- [ ] No time limits on completing forms

**CoinPay Implementation**:
- Session timeout: 30 minutes
- Warning shown: 5 minutes before expiry
- Can extend session by clicking "Stay logged in"

**Status**: ⏳ Pending

---

##### 2.2.2 Pause, Stop, Hide (A)
**Requirement**: Auto-updating content can be paused

**Test Cases**:
- [ ] Auto-refreshing transaction list can be paused
- [ ] Balance auto-refresh can be disabled
- [ ] Loading spinners don't last >5 seconds

**Status**: ⏳ Pending

---

#### 2.3 Seizures and Physical Reactions

##### 2.3.1 Three Flashes or Below Threshold (A)
**Requirement**: No content flashes more than 3 times per second

**Test Cases**:
- [ ] Loading spinners don't flash rapidly
- [ ] Animations don't trigger seizures
- [ ] No strobe effects

**Status**: ⏳ Pending

---

#### 2.4 Navigable

##### 2.4.1 Bypass Blocks (A)
**Requirement**: Skip navigation link present

**Test Cases**:
- [ ] "Skip to main content" link present
- [ ] Link appears on first Tab
- [ ] Link jumps to main content

**Implementation**:
```html
<a href="#main-content" class="skip-link">
  Skip to main content
</a>

<main id="main-content">
  <!-- Page content -->
</main>
```

**Status**: ⏳ Pending

---

##### 2.4.2 Page Titled (A)
**Requirement**: Pages have descriptive titles

**Test Cases**:
- [ ] All pages have unique `<title>`
- [ ] Titles describe page content
- [ ] Titles follow pattern: "Page Name - CoinPay"

**Expected Titles**:
- `"Dashboard - CoinPay"`
- `"Wallet - CoinPay"`
- `"Transfer - CoinPay"`
- `"Transactions - CoinPay"`

**Status**: ⏳ Pending

---

##### 2.4.3 Focus Order (A)
**Requirement**: Focus order is logical

**Test Cases**:
- [ ] Tab order matches visual order
- [ ] Form fields in logical sequence
- [ ] Buttons in logical position

**Status**: ⏳ Pending

---

##### 2.4.4 Link Purpose (In Context) (A)
**Requirement**: Link purpose clear from link text or context

**Test Cases**:
- [ ] No "click here" links
- [ ] Link text describes destination
- [ ] Icon links have aria-label

**Examples**:
- ❌ "Click here for details"
- ✅ "View transaction details"

**Status**: ⏳ Pending

---

##### 2.4.5 Multiple Ways (AA)
**Requirement**: Multiple ways to find pages

**Test Cases**:
- [ ] Navigation menu (all pages)
- [ ] Search functionality (if applicable)
- [ ] Breadcrumbs (if applicable)
- [ ] Sitemap (if applicable)

**CoinPay Navigation**:
- Top navigation bar
- Sidebar menu
- Quick actions

**Status**: ⏳ Pending

---

##### 2.4.6 Headings and Labels (AA)
**Requirement**: Headings and labels are descriptive

**Test Cases**:
- [ ] Headings describe topic or purpose
- [ ] Form labels describe what to enter
- [ ] Button labels describe what happens

**Examples**:
- Heading: "Recent Transactions" (descriptive)
- Label: "Recipient Wallet Address" (descriptive)
- Button: "Send USDC" (action-oriented)

**Status**: ⏳ Pending

---

##### 2.4.7 Focus Visible (AA)
**Requirement**: Keyboard focus indicator is visible

**Test Cases**:
- [ ] All interactive elements have visible focus
- [ ] Focus indicator has 3:1 contrast ratio
- [ ] Focus outline not removed with CSS

**CSS Implementation**:
```css
/* Good: Custom focus style */
button:focus {
  outline: 2px solid #6366f1;
  outline-offset: 2px;
}

/* Bad: Removes focus */
button:focus {
  outline: none; /* ❌ Don't do this */
}
```

**Status**: ⏳ Pending

---

#### 2.5 Input Modalities

##### 2.5.1 Pointer Gestures (A)
**Requirement**: All functionality available with single-pointer actions

**Test Cases**:
- [ ] No complex gestures required
- [ ] No drag-and-drop (or alternative available)
- [ ] No multi-touch gestures

**Status**: ⏳ Pending

---

##### 2.5.2 Pointer Cancellation (A)
**Requirement**: Click action occurs on up-event

**Test Cases**:
- [ ] Buttons activate on mouse up
- [ ] Can abort click by moving away

**Status**: ⏳ Pending

---

##### 2.5.3 Label in Name (A)
**Requirement**: Visible label text is in accessible name

**Test Cases**:
- [ ] Button visible text matches aria-label
- [ ] Icon + text buttons: text in accessible name

**Example**:
```html
<!-- Good -->
<button aria-label="Send USDC">Send USDC</button>

<!-- Bad -->
<button aria-label="Submit">Send USDC</button>
```

**Status**: ⏳ Pending

---

##### 2.5.4 Motion Actuation (A)
**Requirement**: Functions triggered by motion can be disabled

**Assessment**: N/A - No motion-triggered functionality

**Status**: ✅ N/A

---

### Principle 3: Understandable

#### 3.1 Readable

##### 3.1.1 Language of Page (A)
**Requirement**: Page language is specified

**Test Case**:
- [ ] `<html lang="en">` present

**Status**: ⏳ Pending

---

##### 3.1.2 Language of Parts (AA)
**Requirement**: Language of content parts is specified

**Assessment**: All content in English - N/A

**Status**: ✅ N/A

---

#### 3.2 Predictable

##### 3.2.1 On Focus (A)
**Requirement**: Focus doesn't trigger unexpected changes

**Test Cases**:
- [ ] Focusing input doesn't submit form
- [ ] Focusing element doesn't change page
- [ ] Focus doesn't open dialogs

**Status**: ⏳ Pending

---

##### 3.2.2 On Input (A)
**Requirement**: Changing input doesn't cause unexpected changes

**Test Cases**:
- [ ] Typing in input doesn't submit form
- [ ] Selecting dropdown option doesn't navigate away
- [ ] Checkboxes don't submit form on change

**Status**: ⏳ Pending

---

##### 3.2.3 Consistent Navigation (AA)
**Requirement**: Navigation is consistent across pages

**Test Cases**:
- [ ] Navigation menu in same location on all pages
- [ ] Navigation items in same order
- [ ] Header/footer consistent

**Status**: ⏳ Pending

---

##### 3.2.4 Consistent Identification (AA)
**Requirement**: Same functionality has consistent labeling

**Test Cases**:
- [ ] "Submit" button always labeled "Submit" (not "Send" elsewhere)
- [ ] Icons used consistently (checkmark = success everywhere)
- [ ] Error messages use consistent format

**Status**: ⏳ Pending

---

#### 3.3 Input Assistance

##### 3.3.1 Error Identification (A)
**Requirement**: Errors are identified in text

**Test Cases**:
- [ ] Form validation errors shown in text
- [ ] Error location identified (field name)
- [ ] Error description provided

**Example**:
```
❌ "Invalid input" (vague)
✅ "Email address: Please enter a valid email address (example: user@example.com)"
```

**Status**: ⏳ Pending

---

##### 3.3.2 Labels or Instructions (A)
**Requirement**: Labels or instructions provided

**Test Cases**:
- [ ] All form fields have labels
- [ ] Complex fields have instructions
- [ ] Required fields indicated

**Example**:
```html
<label for="amount">
  Amount (USDC) <span aria-label="required">*</span>
</label>
<input
  id="amount"
  type="number"
  required
  aria-describedby="amount-help"
/>
<span id="amount-help">
  Enter amount between 0.000001 and 1,000,000 USDC
</span>
```

**Status**: ⏳ Pending

---

##### 3.3.3 Error Suggestion (AA)
**Requirement**: Suggestions provided when error detected

**Test Cases**:
- [ ] Invalid email: "Please enter a valid email address"
- [ ] Invalid address: "Wallet address must be 42 characters starting with 0x"
- [ ] Amount too low: "Minimum amount is 0.000001 USDC"

**Status**: ⏳ Pending

---

##### 3.3.4 Error Prevention (Legal, Financial, Data) (AA)
**Requirement**: Confirmation required for important actions

**Test Cases**:
- [ ] Transfer has confirmation step
- [ ] Refund requires confirmation
- [ ] Account deletion requires confirmation
- [ ] Can review before submitting

**CoinPay Implementation**:
1. Fill transfer form
2. **Review screen** (can go back)
3. Confirm & send
4. Success message

**Status**: ⏳ Pending

---

### Principle 4: Robust

#### 4.1 Compatible

##### 4.1.1 Parsing (A)
**Requirement**: HTML is valid and properly nested

**Test Cases**:
- [ ] Run W3C HTML Validator
- [ ] No duplicate IDs
- [ ] Properly nested elements
- [ ] Correct ARIA usage

**Tools**: W3C Validator, axe DevTools

**Status**: ⏳ Pending

---

##### 4.1.2 Name, Role, Value (A)
**Requirement**: UI components have accessible name, role, and value

**Test Cases**:
- [ ] Custom components have proper ARIA roles
- [ ] Form inputs have labels
- [ ] Buttons have accessible names
- [ ] Status messages use role="status" or role="alert"

**Status**: ⏳ Pending

---

##### 4.1.3 Status Messages (AA)
**Requirement**: Status messages can be programmatically determined

**Test Cases**:
- [ ] Success messages use `role="status"`
- [ ] Error messages use `role="alert"`
- [ ] Loading states announced
- [ ] Form validation announced

**Example**:
```html
<div role="alert" aria-live="assertive">
  Transfer failed: Insufficient balance
</div>

<div role="status" aria-live="polite">
  Balance updated
</div>
```

**Status**: ⏳ Pending

---

## Automated Testing Tools

### 1. Lighthouse (Chrome DevTools)

**Run Lighthouse**:
1. Open Chrome DevTools (F12)
2. Go to "Lighthouse" tab
3. Select "Accessibility" category
4. Click "Analyze page load"

**Target Score**: > 90

**Test Pages**:
- /dashboard
- /wallet
- /transfer
- /transactions

**Status**: ⏳ Pending

---

### 2. axe DevTools

**Installation**:
- Browser extension: axe DevTools
- Or use axe-core library

**Run Tests**:
1. Install axe DevTools extension
2. Open DevTools
3. Go to "axe DevTools" tab
4. Click "Scan ALL of my page"

**Target**: 0 violations

**Status**: ⏳ Pending

---

### 3. WAVE (WebAIM)

**Run WAVE**:
1. Visit: https://wave.webaim.org/
2. Enter page URL
3. Review results

**Or install browser extension**

**Target**: 0 errors

**Status**: ⏳ Pending

---

### 4. Pa11y

**Installation**:
```bash
npm install -g pa11y
```

**Run Tests**:
```bash
pa11y http://localhost:3000/dashboard
```

**Status**: ⏳ Pending

---

## Manual Testing

### Screen Reader Testing

#### NVDA (Windows - Free)

**Installation**: https://www.nvaccess.org/download/

**Test Procedure**:
1. Start NVDA (Ctrl + Alt + N)
2. Navigate with Tab/Arrow keys
3. Listen to announcements
4. Verify all content accessible

**Common Commands**:
- H: Next heading
- K: Next link
- B: Next button
- F: Next form field
- Insert + Down: Read from cursor

**Status**: ⏳ Pending

---

#### JAWS (Windows - Paid)

**Status**: ⏳ Pending

---

#### VoiceOver (macOS - Built-in)

**Enable**: Cmd + F5

**Test Procedure**:
1. Enable VoiceOver
2. Use Ctrl + Option + Arrow keys
3. Listen to announcements

**Common Commands**:
- Ctrl + Option + Space: Activate
- Ctrl + Option + Right: Next item
- Ctrl + Option + Cmd + H: Next heading

**Status**: ⏳ Pending

---

### Keyboard Navigation Testing

**Test Procedure**:
1. Unplug mouse
2. Use only keyboard
3. Complete user workflows

**Workflows to Test**:
1. Login
2. View dashboard
3. Check balance
4. Submit transfer
5. View transactions
6. Logout

**Status**: ⏳ Pending

---

### Color Blindness Testing

**Tools**:
- Color Oracle (simulator)
- Chrome DevTools (Rendering tab → Emulate vision deficiencies)

**Test Types**:
- Protanopia (red-blind)
- Deuteranopia (green-blind)
- Tritanopia (blue-blind)
- Achromatopsia (no color)

**Status**: ⏳ Pending

---

## Test Execution Summary

| WCAG Criterion | Level | Status | Priority |
|----------------|-------|--------|----------|
| 1.1.1 Non-text Content | A | ⏳ | Critical |
| 1.3.1 Info and Relationships | A | ⏳ | Critical |
| 1.4.3 Contrast (Minimum) | AA | ⏳ | Critical |
| 2.1.1 Keyboard | A | ⏳ | Critical |
| 2.4.2 Page Titled | A | ⏳ | High |
| 2.4.7 Focus Visible | AA | ⏳ | Critical |
| 3.3.1 Error Identification | A | ⏳ | High |
| 3.3.2 Labels or Instructions | A | ⏳ | High |
| 3.3.4 Error Prevention | AA | ⏳ | Critical |
| 4.1.2 Name, Role, Value | A | ⏳ | Critical |
| 4.1.3 Status Messages | AA | ⏳ | High |

**Total Criteria**: 50 (WCAG 2.1 Level AA)
**Tested**: 0
**Passed**: 0
**Failed**: 0

---

## Accessibility Test Report Template

### Execution Date: _______________
### Tester: _______________
### Tools Used: _______________

**Summary**:
- Total criteria tested: ___/50
- Passed: ___
- Failed: ___
- Lighthouse score: ___/100
- axe violations: ___

**Critical Issues Found**:
1. Issue: _______
   - WCAG: _______
   - Impact: _______
   - Recommendation: _______

2. Issue: _______
   - WCAG: _______
   - Impact: _______
   - Recommendation: _______

**Screen Reader Testing**:
- [ ] NVDA tested
- [ ] VoiceOver tested
- [ ] All content announced correctly
- [ ] Navigation clear and logical

**Keyboard Navigation**:
- [ ] All functions accessible
- [ ] Tab order logical
- [ ] No keyboard traps
- [ ] Focus indicators visible

**Color Contrast**:
- [ ] All text meets 4.5:1 ratio
- [ ] Large text meets 3:1 ratio
- [ ] UI components meet 3:1 ratio

**Recommendations**:
1. _______
2. _______
3. _______

**Sign-off**: _______________ Date: _______________

---

## Notes

- Test in multiple browsers (Chrome, Firefox, Safari)
- Test with real assistive technologies
- Involve users with disabilities if possible
- Accessibility is ongoing, not one-time
- Fix critical issues first (A level)
- Document all findings with screenshots

---

**Last Updated**: 2025-10-29
**Version**: 1.0.0
**Sprint**: N02 - QA Phase 2 (Optional)
