# Sprint N04 - Frontend Engineering Plan
# Phase 4: Exchange Investment (WhiteBit Integration)

**Sprint**: N04
**Duration**: 2 weeks (10 working days)
**Sprint Dates**: February 17 - February 28, 2025
**Total Effort**: 22.00 days
**Team Size**: 2-3 engineers
**Utilization**: 73% ✅ (comfortable capacity)

---

## Sprint Goal

Build intuitive investment management UI enabling users to connect WhiteBit accounts, create USDC investments, and track real-time reward accrual with clear APY display and projected earnings visualization.

---

## Epic Breakdown

### Epic 1: WhiteBit Account Connection (4.50 days)

**Description**: Enable users to securely connect their WhiteBit accounts with API credentials and view connection status.

---

#### FE-401: WhiteBit Connection Form (2.00 days)
**Owner**: Frontend Engineer 1
**Priority**: P0 (Critical Path)

**Description**:
Create secure form for users to input WhiteBit API credentials with clear instructions and validation feedback.

**Requirements**:
- API Key input field (masked after entry)
- API Secret input field (password type)
- Clear instructions on obtaining WhiteBit credentials
- Link to WhiteBit API settings page
- Visual security indicators (lock icons, HTTPS badge)
- Connection test before saving
- Loading state during validation
- Success/error feedback

**UI Mockup Structure**:
```tsx
// src/pages/InvestmentPage.tsx
// src/components/investment/WhiteBitConnectionForm.tsx

<WhiteBitConnectionForm>
  <Header>
    <Title>Connect WhiteBit Account</Title>
    <Subtitle>Securely connect your WhiteBit account to start earning yield on USDC</Subtitle>
  </Header>

  <Instructions>
    <Step>1. Log in to WhiteBit and go to Settings → API Keys</Step>
    <Step>2. Create a new API key with "Trade" and "Balance" permissions</Step>
    <Step>3. Copy your API Key and Secret below</Step>
    <Link>How to get WhiteBit API credentials</Link>
  </Instructions>

  <Form>
    <Input label="API Key" type="text" placeholder="whitebit-api-key-..." />
    <Input label="API Secret" type="password" placeholder="Enter your API secret" />
    <SecurityNote>Your credentials are encrypted and stored securely</SecurityNote>
    <Button primary loading={connecting}>Connect WhiteBit</Button>
  </Form>
</WhiteBitConnectionForm>
```

**State Management**:
```tsx
// src/store/investmentStore.ts
interface InvestmentState {
  whiteBitConnection: {
    connected: boolean;
    connectionId: string | null;
    connectedAt: string | null;
    lastValidated: string | null;
  };
  connectWhiteBit: (apiKey: string, apiSecret: string) => Promise<void>;
  disconnectWhiteBit: () => Promise<void>;
  checkConnectionStatus: () => Promise<void>;
}
```

**API Integration**:
```tsx
// src/services/investmentService.ts
export const connectWhiteBit = async (credentials: {
  apiKey: string;
  apiSecret: string;
}): Promise<ConnectionResponse> => {
  const response = await apiClient.post('/api/exchange/whitebit/connect', credentials);
  return response.data;
};
```

**Acceptance Criteria**:
- [ ] Form validates API key format (non-empty, reasonable length)
- [ ] API Secret is masked (password input type)
- [ ] Clear instructions displayed with external link
- [ ] Connection test shows loading state
- [ ] Success message on successful connection
- [ ] Error message on invalid credentials
- [ ] Security indicators visible (lock icon, HTTPS)
- [ ] Responsive design (mobile, tablet, desktop)
- [ ] Accessible (WCAG AA) with proper labels
- [ ] Component tests pass

**Dependencies**: BE-404 (Connect Endpoint)

---

#### FE-402: API Credential Validation (Client-Side) (1.50 days)
**Owner**: Frontend Engineer 1
**Priority**: P0 (Critical Path)

**Description**:
Implement client-side validation for WhiteBit API credentials before submission.

**Validation Rules**:
```tsx
// src/utils/validation.ts
export const validateWhiteBitCredentials = (apiKey: string, apiSecret: string) => {
  const errors: string[] = [];

  // API Key validation
  if (!apiKey || apiKey.trim().length === 0) {
    errors.push('API Key is required');
  } else if (apiKey.length < 20) {
    errors.push('API Key appears to be too short');
  }

  // API Secret validation
  if (!apiSecret || apiSecret.trim().length === 0) {
    errors.push('API Secret is required');
  } else if (apiSecret.length < 32) {
    errors.push('API Secret appears to be too short');
  }

  return errors;
};
```

**Real-time Validation**:
- Field-level validation on blur
- Form-level validation on submit
- Visual feedback (red border, error icon)
- Error messages below fields
- Disabled submit button if validation fails

**Acceptance Criteria**:
- [ ] Real-time field validation on blur
- [ ] Clear error messages for each field
- [ ] Submit button disabled if invalid
- [ ] Visual feedback (colors, icons)
- [ ] Error messages cleared on input change
- [ ] Unit tests for validation functions

**Dependencies**: FE-401

---

#### FE-403: Exchange Connection Status Display (1.00 day)
**Owner**: Frontend Engineer 1
**Priority**: P0 (Critical Path)

**Description**:
Display WhiteBit connection status with visual indicators and management options.

**UI Components**:
```tsx
// src/components/investment/ConnectionStatus.tsx

<ConnectionStatusCard>
  {connected ? (
    <>
      <StatusBadge status="connected">
        <CheckIcon />
        Connected to WhiteBit
      </StatusBadge>
      <Details>
        <Item>Connected: {formatDate(connectedAt)}</Item>
        <Item>Last validated: {formatDate(lastValidated)}</Item>
      </Details>
      <Actions>
        <Button secondary onClick={testConnection}>Test Connection</Button>
        <Button danger onClick={disconnect}>Disconnect</Button>
      </Actions>
    </>
  ) : (
    <>
      <StatusBadge status="disconnected">
        <XIcon />
        Not Connected
      </StatusBadge>
      <Message>Connect your WhiteBit account to start earning yield on USDC</Message>
      <Button primary onClick={openConnectionForm}>Connect WhiteBit</Button>
    </>
  )}
</ConnectionStatusCard>
```

**States**:
- **Connected**: Green badge, connection details, disconnect button
- **Disconnected**: Gray badge, call-to-action, connect button
- **Testing**: Blue badge, loading spinner
- **Error**: Red badge, error message, retry button

**Acceptance Criteria**:
- [ ] Displays current connection status
- [ ] Shows connection timestamp
- [ ] Test connection button functional
- [ ] Disconnect button with confirmation modal
- [ ] Visual status indicators (color, icons)
- [ ] Responsive design
- [ ] Component tests pass

**Dependencies**: BE-405 (Status Endpoint)

---

### Epic 2: Investment Plans & Creation (9.50 days)

**Description**: Display available investment plans, enable amount calculation with projected earnings, and implement multi-step investment creation wizard.

---

#### FE-404: Investment Plans Display Component (2.50 days)
**Owner**: Frontend Engineer 2
**Priority**: P0 (Critical Path)

**Description**:
Display available WhiteBit Flex investment plans with APY comparison, minimums, and plan details.

**UI Design**:
```tsx
// src/components/investment/InvestmentPlansGrid.tsx

<InvestmentPlansGrid>
  <Header>
    <Title>Available Investment Plans</Title>
    <Subtitle>Choose a plan to start earning yield on your USDC</Subtitle>
  </Header>

  <PlansGrid>
    {plans.map(plan => (
      <PlanCard key={plan.planId} highlighted={plan.apy > 8}>
        <Badge>{plan.asset}</Badge>
        <APY>{plan.apy}%</APY>
        <Label>Annual Percentage Yield</Label>

        <Details>
          <Item>
            <Icon>MinAmount</Icon>
            <Text>Min: {plan.minAmount} USDC</Text>
          </Item>
          <Item>
            <Icon>MaxAmount</Icon>
            <Text>Max: {plan.maxAmount} USDC</Text>
          </Item>
          <Item>
            <Icon>Term</Icon>
            <Text>Term: {plan.term}</Text>
          </Item>
        </Details>

        <Description>{plan.description}</Description>

        <Button primary onClick={() => selectPlan(plan)}>
          Select Plan
        </Button>
      </PlanCard>
    ))}
  </PlansGrid>

  {loading && <LoadingSkeleton />}
  {error && <ErrorMessage retry={refetch} />}
</InvestmentPlansGrid>
```

**Features**:
- Responsive grid layout (1 column mobile, 2-3 columns desktop)
- Highlighted best APY plan
- Visual comparison of APY rates
- Clear minimum/maximum amounts
- Term information (flexible vs fixed)
- Loading skeleton placeholders
- Error handling with retry

**Acceptance Criteria**:
- [ ] Displays all available plans in grid
- [ ] APY prominently displayed with formatting
- [ ] Minimum and maximum amounts visible
- [ ] Best plan visually highlighted
- [ ] Responsive grid (1-3 columns)
- [ ] Loading state with skeletons
- [ ] Error state with retry button
- [ ] Plan selection triggers wizard
- [ ] Component tests pass

**Dependencies**: BE-406 (Plans Endpoint)

---

#### FE-405: Investment Amount Calculator (2.00 days)
**Owner**: Frontend Engineer 1
**Priority**: P0 (Critical Path)

**Description**:
Interactive calculator for investment amount with real-time projected earnings display.

**UI Component**:
```tsx
// src/components/investment/InvestmentCalculator.tsx

<InvestmentCalculator>
  <AmountInput>
    <Label>Investment Amount</Label>
    <InputGroup>
      <Input
        type="number"
        value={amount}
        onChange={setAmount}
        placeholder="0.00"
        min={plan.minAmount}
        max={plan.maxAmount}
      />
      <Currency>USDC</Currency>
    </InputGroup>
    <Range>
      <Slider
        min={plan.minAmount}
        max={plan.maxAmount}
        value={amount}
        onChange={setAmount}
      />
      <Labels>
        <span>{plan.minAmount}</span>
        <span>{plan.maxAmount}</span>
      </Labels>
    </Range>
    <QuickAmounts>
      <Button onClick={() => setAmount(100)}>100</Button>
      <Button onClick={() => setAmount(500)}>500</Button>
      <Button onClick={() => setAmount(1000)}>1,000</Button>
      <Button onClick={() => setAmountToMax()}>Max</Button>
    </QuickAmounts>
  </AmountInput>

  <Validation>
    {amount < plan.minAmount && (
      <Error>Minimum amount is {plan.minAmount} USDC</Error>
    )}
    {amount > walletBalance && (
      <Error>Insufficient balance (Available: {walletBalance} USDC)</Error>
    )}
  </Validation>
</InvestmentCalculator>
```

**Calculation Logic**:
```tsx
const calculateProjectedEarnings = (amount: number, apy: number) => {
  const dailyReward = (amount * apy) / 365 / 100;
  const monthlyReward = dailyReward * 30;
  const yearlyReward = dailyReward * 365;

  return {
    daily: dailyReward,
    monthly: monthlyReward,
    yearly: yearlyReward,
  };
};
```

**Acceptance Criteria**:
- [ ] Amount input with number validation
- [ ] Slider for visual amount selection
- [ ] Quick amount buttons (100, 500, 1000, Max)
- [ ] Real-time validation (min, max, balance)
- [ ] Clear error messages
- [ ] Max button fills wallet balance
- [ ] Projected earnings update on amount change
- [ ] Responsive design
- [ ] Unit tests for calculations
- [ ] Component tests pass

**Dependencies**: BE-406 (Plans), Wallet balance

---

#### FE-406: Projected Earnings Visualization (2.00 days)
**Owner**: Frontend Engineer 2
**Priority**: P0 (Critical Path)

**Description**:
Visual display of projected earnings (daily, monthly, yearly) with charts and breakdowns.

**UI Component**:
```tsx
// src/components/investment/ProjectedEarnings.tsx

<ProjectedEarningsCard>
  <Header>
    <Title>Projected Earnings</Title>
    <APYBadge>{plan.apy}% APY</APYBadge>
  </Header>

  <EarningsGrid>
    <EarningItem>
      <Period>Daily</Period>
      <Amount>{formatCurrency(projectedEarnings.daily)} USDC</Amount>
      <SubText>~${formatUSD(projectedEarnings.daily)}</SubText>
    </EarningItem>

    <EarningItem>
      <Period>Monthly</Period>
      <Amount>{formatCurrency(projectedEarnings.monthly)} USDC</Amount>
      <SubText>~${formatUSD(projectedEarnings.monthly)}</SubText>
    </EarningItem>

    <EarningItem highlighted>
      <Period>Yearly</Period>
      <Amount>{formatCurrency(projectedEarnings.yearly)} USDC</Amount>
      <SubText>~${formatUSD(projectedEarnings.yearly)}</SubText>
    </EarningItem>
  </EarningsGrid>

  <Breakdown>
    <Title>Earnings Breakdown</Title>
    <BarChart>
      <Bar label="Principal" value={amount} color="blue" />
      <Bar label="Yearly Reward" value={projectedEarnings.yearly} color="green" />
    </BarChart>
    <Total>
      Total after 1 year: {formatCurrency(amount + projectedEarnings.yearly)} USDC
    </Total>
  </Breakdown>

  <Disclaimer>
    * Projected earnings are estimates based on current APY rates. Actual earnings may vary.
  </Disclaimer>
</ProjectedEarningsCard>
```

**Visual Features**:
- Color-coded earning periods (daily, monthly, yearly)
- Highlighted yearly earnings (most important)
- Simple bar chart comparing principal vs rewards
- USD equivalent values
- Responsive grid layout
- Animated number updates

**Acceptance Criteria**:
- [ ] Displays daily, monthly, yearly projections
- [ ] Updates in real-time as amount changes
- [ ] Formats numbers with proper decimals
- [ ] Shows USD equivalent values
- [ ] Bar chart visualizes principal vs rewards
- [ ] Responsive layout
- [ ] Smooth number animations
- [ ] Disclaimer visible
- [ ] Component tests pass

**Dependencies**: FE-405 (Calculator)

---

#### FE-407: Investment Creation Wizard (3-step) (3.00 days)
**Owner**: Frontend Engineer 2
**Priority**: P0 (Critical Path)

**Description**:
Multi-step wizard guiding users through investment creation with clear progress indicators.

**Wizard Steps**:

**Step 1: Plan & Amount Selection**
```tsx
<WizardStep number={1} title="Choose Plan & Amount">
  <InvestmentPlansGrid onSelectPlan={setPlan} />
  <InvestmentCalculator plan={plan} onAmountChange={setAmount} />
  <ProjectedEarnings amount={amount} apy={plan.apy} />
  <Actions>
    <Button primary onClick={nextStep} disabled={!isValidAmount}>
      Continue to Review
    </Button>
  </Actions>
</WizardStep>
```

**Step 2: Review & Confirm**
```tsx
<WizardStep number={2} title="Review Investment">
  <SummaryCard>
    <SummaryItem label="Investment Plan" value={plan.planName} />
    <SummaryItem label="Investment Amount" value={`${amount} USDC`} />
    <SummaryItem label="APY" value={`${plan.apy}%`} emphasized />
    <SummaryItem label="Estimated Daily Reward" value={`${dailyReward} USDC`} />
    <SummaryItem label="Estimated Yearly Reward" value={`${yearlyReward} USDC`} />
    <Divider />
    <SummaryItem label="Total After 1 Year" value={`${totalAfterYear} USDC`} emphasized />
  </SummaryCard>

  <RiskWarning>
    <Icon>Warning</Icon>
    <Text>Investments carry risk. APY rates may change. Your capital will be transferred to WhiteBit.</Text>
  </RiskWarning>

  <Checkbox>
    <input type="checkbox" checked={agreedToTerms} onChange={setAgreedToTerms} />
    <Label>I understand the risks and agree to the terms</Label>
  </Checkbox>

  <Actions>
    <Button secondary onClick={prevStep}>Back</Button>
    <Button primary onClick={createInvestment} disabled={!agreedToTerms || creating}>
      {creating ? 'Creating Investment...' : 'Confirm & Create'}
    </Button>
  </Actions>
</WizardStep>
```

**Step 3: Success & Next Steps**
```tsx
<WizardStep number={3} title="Investment Created">
  <SuccessAnimation>
    <CheckCircleIcon size="large" color="green" />
  </SuccessAnimation>

  <Message>
    <Title>Investment Successfully Created!</Title>
    <Text>Your USDC is being transferred to WhiteBit. This may take a few minutes.</Text>
  </Message>

  <InvestmentSummary>
    <Item label="Investment ID" value={investmentId} copyable />
    <Item label="Amount Invested" value={`${amount} USDC`} />
    <Item label="Expected APY" value={`${plan.apy}%`} />
    <Item label="Status" value="Processing" badge />
  </InvestmentSummary>

  <NextSteps>
    <Title>What's Next?</Title>
    <Step>1. Your USDC will be transferred to WhiteBit (2-10 minutes)</Step>
    <Step>2. WhiteBit Flex investment will be created automatically</Step>
    <Step>3. You'll start earning rewards immediately once active</Step>
    <Step>4. View your position on the Investment Dashboard</Step>
  </NextSteps>

  <Actions>
    <Button primary onClick={goToDashboard}>View Investment Dashboard</Button>
    <Button secondary onClick={createAnother}>Create Another Investment</Button>
  </Actions>
</WizardStep>
```

**Wizard State Management**:
```tsx
// src/store/investmentStore.ts
interface InvestmentWizardState {
  currentStep: number;
  selectedPlan: InvestmentPlan | null;
  amount: number;
  agreedToTerms: boolean;
  isCreating: boolean;
  createdInvestmentId: string | null;
}
```

**Acceptance Criteria**:
- [ ] 3-step wizard with clear progress indicator
- [ ] Step 1: Plan selection and amount calculation
- [ ] Step 2: Review summary with risk warning
- [ ] Step 3: Success message with next steps
- [ ] Back button navigation (except Step 3)
- [ ] Form validation prevents progression
- [ ] Terms agreement checkbox required
- [ ] Loading state during investment creation
- [ ] Success animation on completion
- [ ] Responsive design (mobile stacks steps vertically)
- [ ] Accessible navigation
- [ ] Component tests for each step

**Dependencies**: FE-404, FE-405, FE-406, BE-408 (Create Investment)

---

### Epic 3: Investment Dashboard (7.00 days)

**Description**: Display active investment positions, real-time reward accrual, and detailed position information.

---

#### FE-408: Active Investment Position Cards (2.50 days)
**Owner**: Frontend Engineer 1
**Priority**: P0 (Critical Path)

**Description**:
Display active investment positions in card format with key metrics and quick actions.

**UI Component**:
```tsx
// src/components/investment/InvestmentPositionCard.tsx

<PositionCard>
  <Header>
    <PlanBadge>{position.planId}</PlanBadge>
    <StatusBadge status={position.status}>
      {position.status === 'active' && <PulseIcon />}
      {position.status}
    </StatusBadge>
  </Header>

  <MainMetrics>
    <Metric primary>
      <Label>Current Value</Label>
      <Value>{formatCurrency(position.currentValue)} USDC</Value>
      <Change positive={position.accruedRewards > 0}>
        +{formatCurrency(position.accruedRewards)} USDC
      </Change>
    </Metric>

    <Metric>
      <Label>Principal</Label>
      <Value>{formatCurrency(position.principalAmount)} USDC</Value>
    </Metric>
  </MainMetrics>

  <APYSection>
    <APYBadge>{position.apy}%</APYBadge>
    <Label>Current APY</Label>
  </APYSection>

  <RewardsSection>
    <RewardItem>
      <Icon>Calendar</Icon>
      <Text>Daily: ~{formatCurrency(position.estimatedDailyReward)} USDC</Text>
    </RewardItem>
    <RewardItem>
      <Icon>TrendingUp</Icon>
      <Text>Days Held: {position.daysHeld}</Text>
    </RewardItem>
  </RewardsSection>

  <Actions>
    <Button secondary onClick={() => viewDetails(position.id)}>
      View Details
    </Button>
    <Button primary onClick={() => withdrawPosition(position.id)}>
      Withdraw
    </Button>
  </Actions>

  <Footer>
    <Text>Last synced: {formatRelativeTime(position.lastSyncedAt)}</Text>
  </Footer>
</PositionCard>
```

**Position Dashboard Layout**:
```tsx
// src/pages/InvestmentDashboardPage.tsx

<InvestmentDashboard>
  <Summary>
    <TotalValueCard>
      <Label>Total Investment Value</Label>
      <Value>{formatCurrency(totalCurrentValue)} USDC</Value>
      <Subtext>Principal: {formatCurrency(totalPrincipal)} USDC</Subtext>
    </TotalValueCard>

    <TotalRewardsCard>
      <Label>Total Accrued Rewards</Label>
      <Value positive>{formatCurrency(totalAccruedRewards)} USDC</Value>
      <Subtext>Across {activePositions.length} positions</Subtext>
    </TotalRewardsCard>
  </Summary>

  <PositionsGrid>
    {positions.map(position => (
      <InvestmentPositionCard key={position.id} position={position} />
    ))}
  </PositionsGrid>

  {positions.length === 0 && (
    <EmptyState>
      <Icon>Investment</Icon>
      <Title>No Active Investments</Title>
      <Text>Create your first investment to start earning yield on USDC</Text>
      <Button primary onClick={createInvestment}>Create Investment</Button>
    </EmptyState>
  )}
</InvestmentDashboard>
```

**Acceptance Criteria**:
- [ ] Displays all active positions in grid
- [ ] Shows current value and principal
- [ ] Displays accrued rewards with color coding
- [ ] APY prominently displayed
- [ ] Days held and daily reward estimate
- [ ] View details and withdraw buttons
- [ ] Last synced timestamp
- [ ] Responsive grid (1-3 columns)
- [ ] Empty state for no positions
- [ ] Summary cards show totals
- [ ] Component tests pass

**Dependencies**: BE-412 (List Positions)

---

#### FE-409: Investment Detail Modal (1.50 days)
**Owner**: Frontend Engineer 1
**Priority**: P1 (High)

**Description**:
Modal displaying comprehensive investment position details with transaction history.

**UI Component**:
```tsx
// src/components/investment/InvestmentDetailModal.tsx

<Modal onClose={close} size="large">
  <Header>
    <Title>Investment Details</Title>
    <StatusBadge status={position.status}>{position.status}</StatusBadge>
  </Header>

  <Section>
    <Title>Overview</Title>
    <Grid>
      <DetailItem label="Plan" value={position.planName} />
      <DetailItem label="Asset" value={position.asset} />
      <DetailItem label="APY" value={`${position.apy}%`} emphasized />
      <DetailItem label="Status" value={position.status} />
    </Grid>
  </Section>

  <Section>
    <Title>Investment Amount</Title>
    <AmountBreakdown>
      <Item label="Principal" value={formatCurrency(position.principalAmount)} />
      <Item label="Accrued Rewards" value={formatCurrency(position.accruedRewards)} positive />
      <Divider />
      <Item label="Current Value" value={formatCurrency(position.currentValue)} emphasized />
    </AmountBreakdown>
  </Section>

  <Section>
    <Title>Timeline</Title>
    <Timeline>
      <Event date={position.startDate} label="Investment Started" />
      <Event date={position.lastSyncedAt} label="Last Synced" />
      <Event date={new Date()} label="Current" active />
      <Metric>Days Held: {position.daysHeld}</Metric>
    </Timeline>
  </Section>

  <Section>
    <Title>Projected Rewards</Title>
    <RewardsGrid>
      <Item label="Daily" value={formatCurrency(position.projectedRewards.daily)} />
      <Item label="Monthly" value={formatCurrency(position.projectedRewards.monthly)} />
      <Item label="Yearly" value={formatCurrency(position.projectedRewards.yearly)} />
    </RewardsGrid>
  </Section>

  <Section>
    <Title>Transaction History</Title>
    <TransactionList>
      {position.transactions.map(tx => (
        <TransactionItem key={tx.id}>
          <Type>{tx.type}</Type>
          <Amount>{formatCurrency(tx.amount)} USDC</Amount>
          <Status>{tx.status}</Status>
          <Date>{formatDate(tx.createdAt)}</Date>
        </TransactionItem>
      ))}
    </TransactionList>
  </Section>

  <Actions>
    <Button secondary onClick={close}>Close</Button>
    <Button primary onClick={() => withdrawPosition(position.id)}>
      Withdraw Investment
    </Button>
  </Actions>
</Modal>
```

**Acceptance Criteria**:
- [ ] Modal displays complete position details
- [ ] Amount breakdown clearly visible
- [ ] Timeline shows investment lifecycle
- [ ] Projected rewards for all periods
- [ ] Transaction history with all events
- [ ] Withdraw button functional
- [ ] Responsive modal design
- [ ] Accessible (ESC to close, focus trap)
- [ ] Component tests pass

**Dependencies**: BE-413 (Position Details)

---

#### FE-410: Reward Accrual Display (Real-time) (2.00 days)
**Owner**: Frontend Engineer 2
**Priority**: P0 (Critical Path)

**Description**:
Real-time reward accrual visualization with animated counters and refresh mechanism.

**Implementation**:
```tsx
// src/components/investment/RewardAccrualDisplay.tsx

const RewardAccrualDisplay = ({ position }: Props) => {
  const [currentReward, setCurrentReward] = useState(position.accruedRewards);
  const [isUpdating, setIsUpdating] = useState(false);

  // Update rewards every 60 seconds to match backend sync
  useEffect(() => {
    const interval = setInterval(() => {
      fetchLatestPosition();
    }, 60000); // 60 seconds

    return () => clearInterval(interval);
  }, [position.id]);

  // Calculate real-time reward (client-side estimation between syncs)
  useEffect(() => {
    const dailyReward = calculateDailyReward(position.principalAmount, position.apy);
    const rewardPerSecond = dailyReward / 86400;

    const timer = setInterval(() => {
      setCurrentReward(prev => prev + rewardPerSecond);
    }, 1000); // Update every second for smooth animation

    return () => clearInterval(timer);
  }, [position]);

  return (
    <RewardAccrualCard>
      <Header>
        <Title>Rewards Accruing</Title>
        <RefreshButton onClick={manualRefresh} spinning={isUpdating}>
          <RefreshIcon />
        </RefreshButton>
      </Header>

      <CurrentReward>
        <AnimatedNumber value={currentReward} decimals={8} />
        <Currency>USDC</Currency>
      </CurrentReward>

      <LiveIndicator>
        <PulseDot />
        <Text>Live rewards updating</Text>
      </LiveIndicator>

      <BreakdownChart>
        <ProgressBar
          percentage={(currentReward / (position.principalAmount * position.apy / 100)) * 100}
          label="Progress to yearly goal"
        />
      </BreakdownChart>

      <Footer>
        <Text>Last synced: {formatRelativeTime(position.lastSyncedAt)}</Text>
        <Text>Next sync: {formatRelativeTime(nextSyncTime)}</Text>
      </Footer>
    </RewardAccrualCard>
  );
};
```

**Animation Features**:
- Smooth number counting animation
- Pulse animation on live indicator
- Progress bar fills over time
- Refresh button with spin animation
- Color transitions on value increase

**Acceptance Criteria**:
- [ ] Rewards update every 60 seconds (backend sync)
- [ ] Client-side estimation updates every second for smooth animation
- [ ] Animated number counter (smooth transitions)
- [ ] Live indicator with pulse animation
- [ ] Manual refresh button functional
- [ ] Progress bar shows yearly goal progress
- [ ] Last synced and next sync time displayed
- [ ] Handles sync failures gracefully
- [ ] Component tests pass

**Dependencies**: BE-412, BE-413, BE-411 (Reward calculations)

---

#### FE-411: Investment Withdrawal Flow (1.50 days)
**Owner**: Frontend Engineer 1
**Priority**: P0 (Critical Path)

**Description**:
User flow to withdraw investment with confirmation and status tracking.

**UI Flow**:
```tsx
// Step 1: Withdraw Confirmation Modal
<WithdrawConfirmationModal>
  <Header>
    <Icon>Warning</Icon>
    <Title>Confirm Withdrawal</Title>
  </Header>

  <Summary>
    <Item label="Current Value" value={formatCurrency(position.currentValue)} />
    <Item label="Principal" value={formatCurrency(position.principalAmount)} />
    <Item label="Rewards Earned" value={formatCurrency(position.accruedRewards)} positive />
    <Divider />
    <Item label="Total to Withdraw" value={formatCurrency(position.currentValue)} emphasized />
  </Summary>

  <WalletSelection>
    <Label>Withdraw to Wallet</Label>
    <Select value={selectedWallet} onChange={setSelectedWallet}>
      {wallets.map(wallet => (
        <option value={wallet.id}>{wallet.address} ({wallet.balance} USDC)</option>
      ))}
    </Select>
  </WalletSelection>

  <Timeline>
    <Title>Withdrawal Timeline</Title>
    <Step>1. Close WhiteBit investment position (2-10 minutes)</Step>
    <Step>2. Transfer USDC to your Circle wallet (2-10 minutes)</Step>
    <Step>3. Total estimated time: 5-20 minutes</Step>
  </Timeline>

  <WarningBox>
    <Icon>Info</Icon>
    <Text>Once withdrawn, you'll stop earning rewards. You can create a new investment anytime.</Text>
  </WarningBox>

  <Actions>
    <Button secondary onClick={cancel}>Cancel</Button>
    <Button danger onClick={confirmWithdraw} loading={withdrawing}>
      {withdrawing ? 'Processing Withdrawal...' : 'Confirm Withdrawal'}
    </Button>
  </Actions>
</WithdrawConfirmationModal>

// Step 2: Withdrawal Status Tracking
<WithdrawalStatusModal>
  <StatusAnimation status={withdrawalStatus} />

  <StatusMessage>
    {withdrawalStatus === 'closing_position' && 'Closing WhiteBit investment position...'}
    {withdrawalStatus === 'transferring' && 'Transferring USDC to your wallet...'}
    {withdrawalStatus === 'completed' && 'Withdrawal completed successfully!'}
    {withdrawalStatus === 'failed' && 'Withdrawal failed. Please try again.'}
  </StatusMessage>

  <ProgressSteps>
    <Step completed={step >= 1} active={step === 1}>Close Position</Step>
    <Step completed={step >= 2} active={step === 2}>Transfer USDC</Step>
    <Step completed={step >= 3} active={step === 3}>Complete</Step>
  </ProgressSteps>

  {withdrawalStatus === 'completed' && (
    <WithdrawalSummary>
      <Item label="Withdrawn Amount" value={formatCurrency(withdrawnAmount)} />
      <Item label="Destination Wallet" value={walletAddress} />
      <Item label="Transaction ID" value={transactionId} copyable />
      <Item label="Total Rewards Earned" value={formatCurrency(totalRewards)} positive />
    </WithdrawalSummary>
  )}

  <Actions>
    {withdrawalStatus === 'completed' && (
      <Button primary onClick={goToDashboard}>Return to Dashboard</Button>
    )}
    {withdrawalStatus === 'failed' && (
      <Button primary onClick={retry}>Retry Withdrawal</Button>
    )}
  </Actions>
</WithdrawalStatusModal>
```

**Acceptance Criteria**:
- [ ] Confirmation modal shows withdrawal summary
- [ ] Wallet selection dropdown functional
- [ ] Timeline displays estimated completion time
- [ ] Warning message about stopping rewards
- [ ] Status tracking during withdrawal process
- [ ] Progress steps visual indicator
- [ ] Success message with withdrawal details
- [ ] Error handling with retry option
- [ ] Transaction ID copyable
- [ ] Component tests pass

**Dependencies**: BE-415 (Withdrawal endpoint)

---

#### FE-412: Investment History Page (2.50 days)
**Owner**: Frontend Engineer 2
**Priority**: P2 (Medium - can defer if needed)

**Description**:
Display historical investment positions with filtering and pagination.

**UI Layout**:
```tsx
// src/pages/InvestmentHistoryPage.tsx

<InvestmentHistoryPage>
  <Header>
    <Title>Investment History</Title>
    <Filters>
      <StatusFilter value={statusFilter} onChange={setStatusFilter}>
        <Option value="all">All</Option>
        <Option value="active">Active</Option>
        <Option value="closed">Closed</Option>
      </StatusFilter>

      <SortDropdown value={sortBy} onChange={setSortBy}>
        <Option value="startDate">Start Date</Option>
        <Option value="endDate">End Date</Option>
        <Option value="amount">Amount</Option>
      </SortDropdown>
    </Filters>
  </Header>

  <HistoryList>
    {positions.map(position => (
      <HistoryCard key={position.id}>
        <Header>
          <PlanBadge>{position.planId}</PlanBadge>
          <StatusBadge status={position.status}>{position.status}</StatusBadge>
        </Header>

        <Amounts>
          <Item label="Principal" value={formatCurrency(position.principalAmount)} />
          <Item label="Final Value" value={formatCurrency(position.finalValue)} />
          <Item label="Total Rewards" value={formatCurrency(position.totalRewards)} positive />
        </Amounts>

        <Timeline>
          <Item label="Started" value={formatDate(position.startDate)} />
          <Item label="Ended" value={formatDate(position.endDate)} />
          <Item label="Duration" value={`${position.daysHeld} days`} />
        </Timeline>

        <Actions>
          <Button secondary onClick={() => viewDetails(position.id)}>View Details</Button>
        </Actions>
      </HistoryCard>
    ))}
  </HistoryList>

  <Pagination
    currentPage={page}
    totalPages={totalPages}
    onPageChange={setPage}
  />
</InvestmentHistoryPage>
```

**Acceptance Criteria**:
- [ ] Displays all investment history with pagination
- [ ] Status filter (all, active, closed)
- [ ] Sort by start date, end date, amount
- [ ] Each card shows principal, final value, rewards
- [ ] Duration and timeline displayed
- [ ] Pagination controls functional
- [ ] Empty state for no history
- [ ] Responsive design
- [ ] Component tests pass

**Dependencies**: BE-416 (History endpoint)

**⚠️ Deferral Candidate**: Can be deferred to Sprint N05 if capacity constrained.

---

## Task Dependencies (Critical Path)

```
FE-401 (Connection Form)
  └── FE-402 (Validation)
        ├── FE-403 (Connection Status)
        └── FE-404 (Investment Plans)
              └── FE-405 (Calculator)
                    └── FE-406 (Projected Earnings)
                          └── FE-407 (Creation Wizard)
                                ├── FE-408 (Position Cards)
                                │     ├── FE-409 (Detail Modal)
                                │     ├── FE-410 (Reward Display)
                                │     └── FE-411 (Withdrawal Flow)
                                └── FE-412 (History) ⚠️ Can defer
```

**Critical Path Duration**: ~19.5 days (without FE-412)
**With Deferral**: ~17 days (defer FE-412 to Sprint N05)

---

## Sprint Backlog (Priority Order)

### Must Have (P0) - Sprint Cannot Ship Without These
1. FE-401: WhiteBit Connection Form
2. FE-402: API Credential Validation
3. FE-403: Connection Status Display
4. FE-404: Investment Plans Display
5. FE-405: Investment Amount Calculator
6. FE-406: Projected Earnings Visualization
7. FE-407: Investment Creation Wizard
8. FE-408: Active Investment Position Cards
9. FE-410: Reward Accrual Display (Real-time)
10. FE-411: Investment Withdrawal Flow

### Should Have (P1) - Important But Can Ship Without
11. FE-409: Investment Detail Modal

### Nice to Have (P2) - Defer to Sprint N05 If Needed
12. FE-412: Investment History Page

---

## Component Library & Reusable Components

### New Components to Create
- `<InvestmentPlanCard />` - Display single investment plan
- `<InvestmentPositionCard />` - Display active position
- `<AnimatedNumber />` - Smooth number counting animation
- `<APYBadge />` - Styled APY display
- `<RewardAccrualDisplay />` - Real-time reward counter
- `<ProjectedEarningsChart />` - Simple bar chart for earnings
- `<InvestmentWizard />` - Multi-step wizard container
- `<WithdrawalConfirmationModal />` - Withdrawal confirmation
- `<ConnectionStatusCard />` - WhiteBit connection status

### Reusable from Previous Sprints
- `<Button />` - Primary, secondary, danger variants
- `<Input />` - Text, number, password inputs
- `<Modal />` - Base modal component
- `<LoadingSkeleton />` - Loading placeholders
- `<StatusBadge />` - Status indicators
- `<ErrorMessage />` - Error display with retry

---

## State Management Updates

### Investment Store
```typescript
// src/store/investmentStore.ts
interface InvestmentState {
  // Connection
  whiteBitConnection: {
    connected: boolean;
    connectionId: string | null;
    connectedAt: string | null;
    lastValidated: string | null;
  };

  // Plans
  investmentPlans: InvestmentPlan[];
  loadingPlans: boolean;
  plansError: string | null;

  // Positions
  positions: InvestmentPosition[];
  loadingPositions: boolean;
  positionsError: string | null;

  // Wizard
  wizardState: {
    currentStep: number;
    selectedPlan: InvestmentPlan | null;
    amount: number;
    agreedToTerms: boolean;
    isCreating: boolean;
    createdInvestmentId: string | null;
  };

  // Actions
  connectWhiteBit: (apiKey: string, apiSecret: string) => Promise<void>;
  disconnectWhiteBit: () => Promise<void>;
  fetchInvestmentPlans: () => Promise<void>;
  fetchPositions: () => Promise<void>;
  createInvestment: (planId: string, amount: number, walletId: string) => Promise<string>;
  withdrawInvestment: (positionId: string, walletId: string) => Promise<void>;
  fetchPositionDetails: (positionId: string) => Promise<InvestmentPosition>;
}
```

---

## API Integration

### Investment Service
```typescript
// src/services/investmentService.ts
export const investmentService = {
  // Connection
  connectWhiteBit: (credentials) => apiClient.post('/api/exchange/whitebit/connect', credentials),
  getConnectionStatus: () => apiClient.get('/api/exchange/whitebit/status'),
  disconnectWhiteBit: () => apiClient.delete('/api/exchange/whitebit/disconnect'),

  // Plans
  getInvestmentPlans: () => apiClient.get('/api/exchange/whitebit/plans'),

  // Positions
  createInvestment: (data) => apiClient.post('/api/investment/create', data),
  getPositions: () => apiClient.get('/api/investment/positions'),
  getPositionDetails: (id) => apiClient.get(`/api/investment/${id}/details`),
  withdrawInvestment: (id, walletId) => apiClient.post(`/api/investment/${id}/withdraw`, { walletId }),

  // History
  getInvestmentHistory: (params) => apiClient.get('/api/investment/history', { params }),
};
```

---

## Testing Requirements

### Unit Tests
- [ ] Reward calculation functions
- [ ] Form validation logic
- [ ] Number formatting utilities
- [ ] API service methods

### Component Tests (React Testing Library)
- [ ] FE-401: Connection form validation
- [ ] FE-405: Calculator amount validation
- [ ] FE-407: Wizard navigation
- [ ] FE-408: Position card display
- [ ] FE-410: Reward counter updates
- [ ] FE-411: Withdrawal confirmation

### Integration Tests (Cypress)
- [ ] Complete investment creation flow
- [ ] Position dashboard display
- [ ] Withdrawal flow
- [ ] Real-time reward updates

---

## Performance Optimization

### Code Splitting
```typescript
// Lazy load investment pages
const InvestmentPage = lazy(() => import('./pages/InvestmentPage'));
const InvestmentDashboard = lazy(() => import('./pages/InvestmentDashboardPage'));
const InvestmentHistory = lazy(() => import('./pages/InvestmentHistoryPage'));
```

### Memoization
- Memoize expensive calculations (projected earnings)
- Use `React.memo` for position cards
- Use `useMemo` for filtered/sorted lists
- Use `useCallback` for event handlers

### Optimization Targets
- [ ] Initial page load < 2s
- [ ] Position dashboard render < 500ms
- [ ] Reward counter updates smooth (60fps)
- [ ] Bundle size < 500KB (gzip)

---

## Accessibility Requirements (WCAG 2.1 AA)

### Keyboard Navigation
- [ ] All interactive elements keyboard accessible
- [ ] Focus visible indicators
- [ ] Logical tab order
- [ ] ESC to close modals

### Screen Reader Support
- [ ] ARIA labels on all inputs
- [ ] ARIA live regions for real-time updates
- [ ] Proper heading hierarchy
- [ ] Alt text for icons

### Visual Accessibility
- [ ] Color contrast ratio > 4.5:1
- [ ] Focus indicators visible
- [ ] Error messages clearly associated with fields
- [ ] Status indicators have text labels

---

## Definition of Done (Frontend)

Sprint N04 frontend work is **DONE** when:

- [ ] All P0 tasks completed (10 tasks)
- [ ] All components implemented and styled
- [ ] WhiteBit connection flow functional
- [ ] Investment creation wizard completes in < 7 clicks
- [ ] Position dashboard displays active investments
- [ ] Reward accrual updates in real-time (60s)
- [ ] Mobile responsive (tested on 3 devices)
- [ ] Accessibility score > 90 (Lighthouse)
- [ ] Zero console errors
- [ ] Component tests pass (>80% coverage)
- [ ] Code reviewed and approved
- [ ] Design review passed
- [ ] User acceptance testing passed

---

## Design Assets Needed

### Before Sprint Start
- [ ] Investment plan card designs
- [ ] Position card layouts
- [ ] Wizard step designs
- [ ] Projected earnings visualization mockups
- [ ] Color palette for APY/rewards (green shades)
- [ ] Icons (investment, reward, withdraw)
- [ ] Loading animations
- [ ] Success/error animations

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-04 | Frontend Lead | Initial Sprint N04 Frontend Plan |

---

**FRONTEND TEAM STATUS**: **READY TO START**

**NEXT STEPS**:
1. **Day 1 Morning**: Sprint kickoff meeting
2. **Day 1**: Review design assets with designer
3. **Day 1 Afternoon**: Begin FE-401 (Connection Form) and FE-404 (Plans Display) in parallel
4. **Day 2-5**: Complete Epic 1 & 2 (connection and creation flow)
5. **Day 6-10**: Complete Epic 3 (dashboard and real-time updates)

---

**End of Sprint N04 Frontend Plan**
