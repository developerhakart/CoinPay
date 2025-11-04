# Sprint N04 - Next Steps Summary

**Date**: 2025-11-04
**Current Progress**: Foundation Complete (10%)
**Priority**: Critical Path Tasks

---

## 🎯 Immediate Next Steps (Priority Order)

### Backend Critical Path (Est: 7-10 days with team)

#### STEP 1: Database Infrastructure (Day 1)
**Task**: Update AppDbContext and create migration

**Actions**:
```bash
# 1. Add new DbSets to AppDbContext.cs:
- DbSet<ExchangeConnection> ExchangeConnections
- DbSet<InvestmentPosition> InvestmentPositions
- DbSet<InvestmentTransaction> InvestmentTransactions

# 2. Add indexes and relationships in OnModelCreating

# 3. Create and run migration
cd CoinPay.Api
dotnet ef migrations add AddInvestmentInfrastructure
dotnet ef database update
```

**Files to Modify**:
- `CoinPay.Api/Data/AppDbContext.cs` ✅ (located, needs update)

---

#### STEP 2: Encryption Service (Day 2) - CRITICAL
**Task**: BE-403 - Implement user-level encryption for API credentials

**Files to Create**:
```
CoinPay.Api/Services/Encryption/
├── IExchangeCredentialEncryptionService.cs
└── ExchangeCredentialEncryptionService.cs
```

**Implementation**:
- AES-256-GCM encryption
- User-specific encryption keys
- Key derivation from user ID + master key
- Secure key storage (environment variables or AWS KMS)

**Code Template**:
```csharp
public interface IExchangeCredentialEncryptionService
{
    Task<string> EncryptAsync(string plaintext, Guid userId);
    Task<string> DecryptAsync(string ciphertext, Guid userId);
}
```

---

#### STEP 3: Repositories (Day 2-3)
**Task**: BE-407 - Create investment data access layer

**Files to Create**:
```
CoinPay.Api/Repositories/
├── IExchangeConnectionRepository.cs
├── ExchangeConnectionRepository.cs
├── IInvestmentRepository.cs
└── InvestmentRepository.cs
```

**Key Methods**:
- ExchangeConnection: CRUD operations, GetByUserAndExchange
- InvestmentPosition: CRUD, GetByUserId, GetActivePositions
- InvestmentTransaction: Create, GetByPositionId

---

#### STEP 4: Authentication Service (Day 3)
**Task**: BE-402 - WhiteBit authentication helper

**Files to Create**:
```
CoinPay.Api/Services/Exchange/WhiteBit/
├── IWhiteBitAuthService.cs
└── WhiteBitAuthService.cs
```

**Features**:
- Validate API credentials
- Test connection
- Signature generation (already in WhiteBitApiClient)

---

#### STEP 5: Controllers (Day 4-5)
**Task**: BE-404, BE-405, BE-406, BE-408, BE-412, BE-413, BE-415

**Files to Create**:
```
CoinPay.Api/Controllers/
├── ExchangeController.cs  (3 endpoints)
└── InvestmentController.cs (5 endpoints)
```

**ExchangeController Endpoints**:
- POST /api/exchange/whitebit/connect
- GET /api/exchange/whitebit/status
- GET /api/exchange/whitebit/plans

**InvestmentController Endpoints**:
- POST /api/investment/create
- GET /api/investment/positions
- GET /api/investment/{id}/details
- POST /api/investment/{id}/withdraw
- GET /api/investment/history (optional - can defer)

---

#### STEP 6: Reward Calculation Engine (Day 5-6)
**Task**: BE-411 - Financial calculations

**Files to Create**:
```
CoinPay.Api/Services/Investment/
├── IRewardCalculationService.cs
└── RewardCalculationService.cs
```

**Key Calculations**:
```csharp
// Daily reward = Principal × (APY / 365 / 100)
// Accrued reward = Daily reward × Days held
// Projected rewards (1d, 30d, 365d)
```

**Accuracy**: Must be accurate to 8 decimal places

---

#### STEP 7: Background Worker (Day 6-7)
**Task**: BE-410 - Position sync service

**Files to Create**:
```
CoinPay.Api/Services/BackgroundWorkers/
└── InvestmentPositionSyncService.cs
```

**Features**:
- Runs every 60 seconds
- Syncs active positions with WhiteBit
- Updates current values and accrued rewards
- Handles API failures gracefully

---

#### STEP 8: Program.cs Configuration (Day 7)
**Task**: Register all services in DI container

**Add to Program.cs**:
```csharp
// Register WhiteBit services
builder.Services.AddScoped<IWhiteBitApiClient, WhiteBitApiClient>();
builder.Services.AddScoped<IWhiteBitAuthService, WhiteBitAuthService>();

// Register encryption
builder.Services.AddScoped<IExchangeCredentialEncryptionService,
    ExchangeCredentialEncryptionService>();

// Register repositories
builder.Services.AddScoped<IExchangeConnectionRepository,
    ExchangeConnectionRepository>();
builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();

// Register investment services
builder.Services.AddScoped<IRewardCalculationService, RewardCalculationService>();
builder.Services.AddScoped<IUsdcTransferService, UsdcTransferService>();

// Register background workers
builder.Services.AddHostedService<InvestmentPositionSyncService>();

// Add appsettings configuration
builder.Services.Configure<WhiteBitOptions>(
    builder.Configuration.GetSection("WhiteBit"));
```

---

### Frontend Critical Path (Est: 5-7 days with team)

#### STEP 9: Setup Investment Infrastructure (Day 1)

**Files to Create**:
```
CoinPay.Web/src/
├── store/investmentStore.ts          (Zustand store)
├── services/investmentService.ts     (API client)
└── types/investment.ts               (TypeScript types)
```

**investmentStore.ts Structure**:
```typescript
interface InvestmentState {
  whiteBitConnection: {...};
  investmentPlans: InvestmentPlan[];
  positions: InvestmentPosition[];
  // Actions
  connectWhiteBit: (credentials) => Promise<void>;
  fetchPlans: () => Promise<void>;
  createInvestment: (data) => Promise<void>;
  fetchPositions: () => Promise<void>;
}
```

---

#### STEP 10: WhiteBit Connection UI (Day 2-3)
**Task**: FE-401, FE-402, FE-403

**Files to Create**:
```
CoinPay.Web/src/
├── pages/InvestmentPage.tsx
├── components/investment/
│   ├── WhiteBitConnectionForm.tsx
│   ├── ConnectionStatus.tsx
│   └── index.ts
└── utils/validation.ts (add credential validation)
```

**Features**:
- Secure credential input form
- Real-time validation
- Connection status display
- Error handling

---

#### STEP 11: Investment Plans & Calculator (Day 4-5)
**Task**: FE-404, FE-405, FE-406

**Files to Create**:
```
CoinPay.Web/src/components/investment/
├── InvestmentPlansGrid.tsx
├── InvestmentCalculator.tsx
└── ProjectedEarnings.tsx
```

**Features**:
- Display available plans with APY
- Interactive amount calculator
- Real-time earnings projections
- Responsive grid layout

---

#### STEP 12: Investment Creation Wizard (Day 6-7)
**Task**: FE-407 - 3-step wizard

**Files to Create**:
```
CoinPay.Web/src/components/investment/
└── InvestmentWizard.tsx (complex component)
```

**Steps**:
1. Plan & Amount Selection
2. Review & Confirmation
3. Success & Next Steps

---

#### STEP 13: Investment Dashboard (Day 8-10)
**Task**: FE-408, FE-409, FE-410, FE-411

**Files to Create**:
```
CoinPay.Web/src/
├── pages/InvestmentDashboardPage.tsx
└── components/investment/
    ├── InvestmentPositionCard.tsx
    ├── InvestmentDetailModal.tsx
    ├── RewardAccrualDisplay.tsx (real-time)
    └── WithdrawalFlow.tsx
```

**Features**:
- Position cards with real-time rewards
- Detailed position view
- Withdrawal flow with confirmation
- 60-second refresh for rewards

---

### QA Critical Path (Est: 3-5 days parallel with dev)

#### STEP 14: Test Planning (Day 1)
**Task**: QA-401

**File to Create**:
```
Testing/QA/QA-401-Phase4-Test-Plan.md
```

**Contents**:
- 140+ test cases documented
- Test data preparation
- Test environment setup

---

#### STEP 15: API Integration Tests (Day 2-4)
**Task**: QA-402

**Files to Create**:
```
Testing/Integration/
└── Investment.API.Tests/
    ├── WhiteBitApiTests.cs
    ├── ExchangeControllerTests.cs
    └── InvestmentControllerTests.cs
```

---

#### STEP 16: Financial Validation (Day 3-5)
**Task**: QA-404

**Create**:
- Manual calculation spreadsheet
- Automated calculation tests
- Edge case validation

---

#### STEP 17: E2E Automation (Day 5-7)
**Task**: QA-406

**File to Create**:
```
Testing/E2E/playwright/investment-lifecycle.spec.ts
```

**Scenarios**:
- Full investment creation flow
- Position tracking and reward accrual
- Withdrawal flow
- Error handling

---

## 📋 Quick Reference Checklist

### Backend (14 tasks remaining)
- [ ] Update AppDbContext with new DbSets
- [ ] Create EF migration
- [ ] Implement encryption service (BE-403)
- [ ] Implement authentication service (BE-402)
- [ ] Create exchange connection repository
- [ ] Create investment repository
- [ ] Create ExchangeController (BE-404, BE-405, BE-406)
- [ ] Create InvestmentController (BE-408, BE-412, BE-413, BE-415)
- [ ] Implement reward calculation service (BE-411)
- [ ] Implement USDC transfer service (BE-409)
- [ ] Implement position sync background worker (BE-410)
- [ ] Configure Program.cs services
- [ ] Update appsettings.json
- [ ] Test all endpoints

### Frontend (12 tasks remaining)
- [ ] Create investment Zustand store
- [ ] Create investment service (API client)
- [ ] Create TypeScript types
- [ ] Build WhiteBit connection form (FE-401)
- [ ] Build credential validation (FE-402)
- [ ] Build connection status display (FE-403)
- [ ] Build investment plans grid (FE-404)
- [ ] Build investment calculator (FE-405)
- [ ] Build projected earnings (FE-406)
- [ ] Build 3-step wizard (FE-407)
- [ ] Build investment dashboard (FE-408)
- [ ] Build position cards + detail modal + rewards + withdrawal (FE-409-411)

### QA (4 tasks remaining)
- [ ] Write Phase 4 test plan (QA-401)
- [ ] Create API integration tests (QA-402)
- [ ] Validate financial calculations (QA-404)
- [ ] Create E2E automation (QA-406)

---

## 🚀 Team Assignment Recommendations

### Backend Team (3 engineers)
- **Engineer 1 (Senior)**: BE-403 (Encryption), BE-402 (Auth), BE-410 (Background Worker)
- **Engineer 2**: Repositories, ExchangeController, BE-411 (Reward Calc)
- **Engineer 3**: InvestmentController, BE-409 (USDC Transfer), testing

### Frontend Team (2 engineers)
- **Engineer 1**: Infrastructure (store, service), Connection UI (FE-401-403), Calculator (FE-405-406)
- **Engineer 2**: Plans Grid (FE-404), Wizard (FE-407), Dashboard (FE-408-411)

### QA Team (2 engineers)
- **Engineer 1**: Test plan (QA-401), API tests (QA-402), E2E automation (QA-406)
- **Engineer 2**: Financial validation (QA-404), regression testing, bug triage

---

## ⏱️ Estimated Timeline

**With Full Team (7 engineers)**:
- Days 1-7: Backend implementation
- Days 5-12: Frontend implementation (overlapping)
- Days 1-12: QA parallel testing
- **Total**: 12 days (slightly over 10-day sprint, but achievable with focus)

**With Deferrals** (remove BE-414, BE-416, BE-417, FE-412):
- **Total**: 10 days (fits sprint perfectly)

---

## 📝 Files Already Created ✅

1. ✅ `Services/Exchange/WhiteBit/IWhiteBitApiClient.cs`
2. ✅ `Services/Exchange/WhiteBit/WhiteBitApiClient.cs`
3. ✅ `DTOs/Exchange/WhiteBitDTOs.cs`
4. ✅ `Models/ExchangeConnection.cs`
5. ✅ `Models/InvestmentPosition.cs`
6. ✅ All Sprint N04 planning documents (5 files)

---

**Next Action**: Start with STEP 1 (Update AppDbContext) and proceed sequentially through critical path.

---

**Last Updated**: 2025-11-04
**Status**: Ready for team execution
**Progress**: 10% complete (foundation established)

**End of Next Steps Summary**
