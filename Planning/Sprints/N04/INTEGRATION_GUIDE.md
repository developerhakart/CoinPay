# Sprint N04: Exchange Investment - Integration Guide

**Status:** ✅ COMPLETE AND READY FOR TESTING
**Date:** 2025-11-04
**Version:** 1.0

---

## 🎯 Overview

This guide provides step-by-step instructions for integrating, testing, and deploying the **Exchange Investment** feature (Sprint N04). All code development is **100% complete**. This guide focuses on environment setup, integration testing, and deployment.

---

## 📋 Prerequisites Checklist

Before starting integration, ensure you have:

### Development Environment
- [ ] PostgreSQL 15+ installed and running
- [ ] .NET 9.0 SDK installed
- [ ] Node.js 18+ and npm installed
- [ ] HashiCorp Vault running (for production) or configured master key (for development)
- [ ] WhiteBit API test account (or production API keys)

### Knowledge Requirements
- [ ] Basic understanding of ASP.NET Core
- [ ] React and TypeScript fundamentals
- [ ] Database migration processes
- [ ] REST API testing (Postman/curl)

---

## 🚀 Quick Start (5 Minutes)

### 1. Database Setup

```bash
# Navigate to API project
cd CoinPay.Api

# Apply the investment infrastructure migration
dotnet ef database update

# Verify tables were created
psql -d coinpay -c "\dt investment*"
# Should show: investment_positions, investment_transactions, exchange_connections
```

### 2. Configuration

**Development Environment (`appsettings.Development.json`):**

```json
{
  "WhiteBit": {
    "ApiUrl": "https://whitebit.com/api/v4",
    "RateLimit": {
      "MaxRequestsPerMinute": 100,
      "MaxRequestsPerSecond": 10
    }
  },
  "ExchangeCredentialEncryption": {
    "MasterKey": "YOUR_BASE64_ENCODED_32_BYTE_KEY_HERE"
  }
}
```

Generate a secure master key:
```bash
# PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))

# Linux/macOS
openssl rand -base64 32
```

### 3. Start the Backend

```bash
cd CoinPay.Api
dotnet run

# Should see:
# Now listening on: http://localhost:5000
# Hosting environment: Development
```

### 4. Start the Frontend

```bash
cd CoinPay.Web
npm install
npm run dev

# Should see:
# VITE ready in XXX ms
# Local: http://localhost:3000
```

### 5. Access the Application

1. Navigate to `http://localhost:3000`
2. Login or register a new account
3. Click on the **Investment** card (gradient blue/purple)
4. You should see the Investment Dashboard

---

## 🧪 Testing Guide

### Backend API Testing

#### 1. Test WhiteBit Connection

**Endpoint:** `POST /api/exchange/whitebit/connect`

```bash
curl -X POST http://localhost:5000/api/exchange/whitebit/connect \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "apiKey": "your-whitebit-api-key",
    "apiSecret": "your-whitebit-api-secret"
  }'
```

**Expected Response (200 OK):**
```json
{
  "success": true,
  "connectionId": "guid",
  "exchangeName": "WhiteBit",
  "connectedAt": "2025-11-04T...",
  "isActive": true
}
```

#### 2. Get Investment Plans

**Endpoint:** `GET /api/exchange/whitebit/plans`

```bash
curl -X GET http://localhost:5000/api/exchange/whitebit/plans \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Expected Response (200 OK):**
```json
{
  "plans": [
    {
      "planId": "whitebit-flexible-btc",
      "asset": "BTC",
      "apy": 5.0,
      "apyFormatted": "5.00%",
      "minAmount": 0.001,
      "term": "Flexible",
      "description": "Flexible staking with daily rewards"
    }
  ]
}
```

#### 3. Create Investment

**Endpoint:** `POST /api/investment/create`

```bash
curl -X POST http://localhost:5000/api/investment/create \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "planId": "whitebit-flexible-btc",
    "amount": 0.01
  }'
```

**Expected Response (201 Created):**
```json
{
  "positionId": "guid",
  "planId": "whitebit-flexible-btc",
  "asset": "BTC",
  "principalAmount": 0.01,
  "apy": 5.0,
  "status": "Active",
  "estimatedDailyReward": 0.00000137,
  "startDate": "2025-11-04T..."
}
```

#### 4. Get User Positions

**Endpoint:** `GET /api/investment/positions`

```bash
curl -X GET http://localhost:5000/api/investment/positions \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Expected Response (200 OK):**
```json
{
  "positions": [
    {
      "id": "guid",
      "planId": "whitebit-flexible-btc",
      "asset": "BTC",
      "principalAmount": 0.01,
      "currentValue": 0.01000137,
      "accruedRewards": 0.00000137,
      "apy": 5.0,
      "status": "Active",
      "daysHeld": 1
    }
  ]
}
```

#### 5. Withdraw Investment

**Endpoint:** `POST /api/investment/withdraw/{positionId}`

```bash
curl -X POST http://localhost:5000/api/investment/withdraw/YOUR_POSITION_ID \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Expected Response (200 OK):**
```json
{
  "success": true,
  "positionId": "guid",
  "withdrawnAmount": 0.01000137,
  "principalAmount": 0.01,
  "rewards": 0.00000137,
  "status": "Closed"
}
```

### Frontend UI Testing

#### User Flow 1: Connect WhiteBit Account

1. Login to the application
2. Navigate to Investment page (`/investment`)
3. Fill in the **Connect WhiteBit** form:
   - API Key: (your test API key)
   - API Secret: (your test API secret)
4. Click **Connect Exchange**
5. ✅ Success message should appear
6. Form should hide and dashboard should show

#### User Flow 2: Create Investment

1. On Investment Dashboard, click **Create New Investment**
2. **Step 1 - Select Plan:**
   - Choose a plan from the grid (e.g., BTC Flexible)
   - Click **Next**
3. **Step 2 - Enter Amount:**
   - Use slider or input to set amount (e.g., 0.01 BTC)
   - Review projected earnings
   - Click **Next**
4. **Step 3 - Confirm:**
   - Review all details
   - Click **Create Investment**
5. ✅ Position should appear in Active Positions section

#### User Flow 3: View Position Details

1. Find an active position card
2. Click **View Details**
3. Modal should open showing:
   - Asset and plan information
   - Current value with real-time updates
   - Projected earnings (daily/monthly/yearly)
   - Transaction history
4. Click **Close** to return to dashboard

#### User Flow 4: Withdraw Investment

1. Find an active position card
2. Click **Withdraw** button
3. Confirmation dialog should appear showing:
   - Principal amount
   - Accrued rewards
   - Total withdrawal amount
4. Click **Confirm Withdrawal**
5. ✅ Position should move to "Closed Positions"

#### Real-Time Updates Test

1. Create an investment position
2. Observe the **Accrued Rewards** value on the position card
3. ✅ Value should update **every second** showing live reward accrual
4. Refresh the page
5. ✅ Background worker sync should fetch actual values from API

---

## 🔍 Verification Checklist

### Database Verification

```sql
-- Check exchange connections
SELECT id, user_id, exchange_name, is_active, last_validated_at
FROM exchange_connections;

-- Check investment positions
SELECT id, user_id, plan_id, asset, principal_amount, accrued_rewards, status
FROM investment_positions;

-- Check investment transactions
SELECT id, investment_position_id, type, amount, status, created_at
FROM investment_transactions;
```

### Backend Health Check

```bash
# API is running
curl http://localhost:5000/health

# Database connection works
dotnet ef database update --dry-run

# Background worker is running
# Check logs for: "InvestmentPositionSyncService is running"
```

### Frontend Build Verification

```bash
cd CoinPay.Web

# TypeScript compilation
npm run build
# Should complete with 0 errors

# Check bundle size (should be ~450KB)
ls -lh dist/assets/
```

---

## 🎨 UI Component Testing

### ConnectWhiteBitForm
- [ ] API Key input validation (required, min length)
- [ ] API Secret input masked (password type)
- [ ] Connection status indicator updates
- [ ] Success message appears on successful connection
- [ ] Error message appears on failed connection
- [ ] Form clears after successful connection

### InvestmentDashboard
- [ ] Portfolio summary cards display correct totals
- [ ] Active positions grid layout responsive
- [ ] Closed positions collapsible section works
- [ ] Quick actions buttons functional
- [ ] Empty state shows when no positions
- [ ] Loading states appear during API calls

### CreateInvestmentWizard
- [ ] Step 1: Plan selection highlights selected plan
- [ ] Step 2: Amount slider updates projections in real-time
- [ ] Step 3: Confirmation summary shows all details
- [ ] Navigation buttons (Back/Next) work correctly
- [ ] Submit creates position and closes wizard
- [ ] Cancel button closes wizard without creating

### PositionCard
- [ ] Real-time reward updates (every 1 second)
- [ ] Current value = principal + accrued rewards
- [ ] Profit/loss percentage calculation correct
- [ ] Status badge color matches status (green/gray/red)
- [ ] View Details button opens modal
- [ ] Withdraw button only shows for Active positions

### PositionDetailsModal
- [ ] Modal opens/closes correctly
- [ ] Position data loads on open
- [ ] Projected rewards display (daily/monthly/yearly)
- [ ] Transaction history shows all transactions
- [ ] Withdraw confirmation flow works
- [ ] Modal closes after successful withdrawal

---

## 🔐 Security Testing

### Encryption Verification

1. **Test Credential Encryption:**
```bash
# Create a connection and check database
SELECT api_key_encrypted, api_secret_encrypted
FROM exchange_connections
WHERE user_id = 'YOUR_USER_ID';

# Values should be Base64 encoded, not plain text
# Format: nonce(12) + tag(16) + ciphertext
```

2. **Test User-Level Encryption:**
```bash
# Create 2 users with same API credentials
# Check that encrypted values are DIFFERENT
SELECT user_id, api_key_encrypted
FROM exchange_connections;
```

3. **Test Decryption:**
```bash
# API should successfully decrypt and use credentials
# Check logs for successful WhiteBit API calls
# No decryption errors should appear
```

### Input Validation

Test these edge cases:

**WhiteBit Connection:**
- [ ] Empty API key → 400 Bad Request
- [ ] Empty API secret → 400 Bad Request
- [ ] Invalid API credentials → Connection error message
- [ ] SQL injection attempts → Sanitized

**Create Investment:**
- [ ] Amount below minimum → Error message
- [ ] Amount = 0 → Error message
- [ ] Negative amount → Error message
- [ ] Invalid plan ID → 404 Not Found
- [ ] Not connected to exchange → Error message

---

## 📊 Performance Testing

### Backend Performance

```bash
# Test response times (should be < 500ms)
time curl http://localhost:5000/api/investment/positions \
  -H "Authorization: Bearer TOKEN"

# Test background worker (check logs)
# Should sync every 60 seconds, processing all active positions
```

### Frontend Performance

```bash
# Build size check
npm run build
# Main bundle should be ~450KB (gzipped ~125KB)

# Lighthouse audit (Chrome DevTools)
# Performance score should be > 90
# Accessibility score should be > 90
```

### Load Testing

```bash
# Install artillery (if not already installed)
npm install -g artillery

# Create test scenario
cat > load-test.yml << EOF
config:
  target: 'http://localhost:5000'
  phases:
    - duration: 60
      arrivalRate: 10
scenarios:
  - name: "Get positions"
    flow:
      - get:
          url: "/api/investment/positions"
          headers:
            Authorization: "Bearer TOKEN"
EOF

# Run load test
artillery run load-test.yml

# Success criteria:
# - Response time p95 < 1000ms
# - Error rate < 1%
```

---

## 🐛 Common Issues and Troubleshooting

### Issue 1: Database Migration Failed

**Error:** `No such host is known` or `Could not connect to database`

**Solution:**
```bash
# Ensure PostgreSQL is running
# Windows
net start postgresql-x64-15

# Linux/macOS
sudo systemctl start postgresql
# OR
brew services start postgresql

# Check connection string in appsettings.json
# Verify host, port, database name, username, password
```

### Issue 2: Frontend Build Errors

**Error:** `TS6133: 'X' is declared but its value is never read`

**Solution:**
```bash
# This is strict TypeScript mode
# Remove unused imports and variables
# Or temporarily disable in tsconfig.json:
# "noUnusedLocals": false
```

### Issue 3: CORS Errors in Browser

**Error:** `Access-Control-Allow-Origin header missing`

**Solution:**
```csharp
// In CoinPay.Api/Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Issue 4: Background Worker Not Running

**Error:** Positions not syncing, rewards not updating

**Solution:**
```bash
# Check logs for:
# "InvestmentPositionSyncService is running"

# Ensure service is registered in Program.cs:
builder.Services.AddHostedService<InvestmentPositionSyncService>();

# Restart the API server
dotnet run
```

### Issue 5: Real-Time Updates Not Working

**Error:** Position card rewards not updating every second

**Solution:**
```javascript
// Check browser console for errors
// Verify interval is set correctly in PositionCard.tsx
// Ensure position has startDate and estimatedDailyReward
```

---

## 🚢 Deployment

### Production Deployment Checklist

#### Backend (API)

1. **Environment Configuration:**
```json
// appsettings.Production.json
{
  "WhiteBit": {
    "ApiUrl": "https://whitebit.com/api/v4",
    "RateLimit": {
      "MaxRequestsPerMinute": 100,
      "MaxRequestsPerSecond": 10
    }
  },
  "ExchangeCredentialEncryption": {
    "MasterKey": "VAULT_SECRET_PATH"  // Use Vault in production
  }
}
```

2. **Database Migration:**
```bash
# Production database
dotnet ef database update --connection "YOUR_PROD_CONNECTION_STRING"
```

3. **HashiCorp Vault Setup:**
```bash
# Store master encryption key in Vault
vault kv put secret/coinpay/encryption master_key="YOUR_KEY"

# Update Program.cs to read from Vault
```

4. **Deploy API:**
```bash
# Publish release build
dotnet publish -c Release -o ./publish

# Copy to production server
# Set environment: ASPNETCORE_ENVIRONMENT=Production
# Start service
```

#### Frontend (Web)

1. **Environment Configuration:**
```bash
# .env.production
VITE_API_URL=https://api.yourprod.com
```

2. **Build Production Bundle:**
```bash
npm run build
# Output: dist/ directory
```

3. **Deploy to CDN/Web Server:**
```bash
# Copy dist/ contents to web server
# Configure reverse proxy (nginx/Apache)
# Enable HTTPS
```

#### Post-Deployment Verification

- [ ] Health check endpoint responds
- [ ] Database connection successful
- [ ] Background worker logs show activity
- [ ] API endpoints accessible
- [ ] Frontend loads correctly
- [ ] Authentication works
- [ ] Investment creation flow works end-to-end
- [ ] Real-time updates functioning
- [ ] Error handling works (try invalid inputs)
- [ ] Encryption/decryption working (check logs)

---

## 📚 API Reference Summary

### Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/exchange/whitebit/connect` | Connect WhiteBit account | ✅ |
| GET | `/api/exchange/whitebit/status` | Get connection status | ✅ |
| POST | `/api/exchange/whitebit/disconnect` | Disconnect account | ✅ |
| GET | `/api/exchange/whitebit/plans` | Get investment plans | ✅ |
| POST | `/api/investment/create` | Create investment | ✅ |
| GET | `/api/investment/positions` | Get user positions | ✅ |
| GET | `/api/investment/{id}` | Get position details | ✅ |
| POST | `/api/investment/withdraw/{id}` | Withdraw investment | ✅ |

### Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful request |
| 201 | Created | Investment created |
| 400 | Bad Request | Invalid input data |
| 401 | Unauthorized | Missing/invalid auth token |
| 404 | Not Found | Position/plan not found |
| 500 | Server Error | Internal error |

---

## 📁 File Structure Reference

```
CoinPay/
├── CoinPay.Api/
│   ├── Controllers/
│   │   ├── ExchangeController.cs (7 endpoints, 185 lines)
│   │   └── InvestmentController.cs (4 endpoints, 325 lines)
│   ├── Models/
│   │   ├── ExchangeConnection.cs (65 lines)
│   │   └── InvestmentPosition.cs (125 lines)
│   ├── DTOs/Exchange/
│   │   └── WhiteBitDTOs.cs (20+ DTOs, 280 lines)
│   ├── Services/
│   │   ├── Exchange/WhiteBit/
│   │   │   ├── IWhiteBitApiClient.cs
│   │   │   ├── WhiteBitApiClient.cs (285 lines)
│   │   │   ├── IWhiteBitAuthService.cs
│   │   │   └── WhiteBitAuthService.cs (85 lines)
│   │   ├── Encryption/
│   │   │   ├── IExchangeCredentialEncryptionService.cs
│   │   │   └── ExchangeCredentialEncryptionService.cs (145 lines)
│   │   ├── Investment/
│   │   │   ├── IRewardCalculationService.cs
│   │   │   └── RewardCalculationService.cs (80 lines)
│   │   └── BackgroundWorkers/
│   │       └── InvestmentPositionSyncService.cs (185 lines)
│   ├── Repositories/
│   │   ├── IExchangeConnectionRepository.cs
│   │   ├── ExchangeConnectionRepository.cs
│   │   ├── IInvestmentRepository.cs
│   │   └── InvestmentRepository.cs
│   ├── Data/
│   │   └── AppDbContext.cs (updated with investment tables)
│   └── Migrations/
│       └── AddInvestmentInfrastructure.cs
├── CoinPay.Web/
│   ├── src/
│   │   ├── pages/
│   │   │   └── InvestmentPage.tsx (24 lines)
│   │   ├── components/Investment/
│   │   │   ├── index.ts (8 exports)
│   │   │   ├── ConnectWhiteBitForm.tsx (370 lines)
│   │   │   ├── InvestmentPlans.tsx (390 lines)
│   │   │   ├── InvestmentCalculator.tsx (440 lines)
│   │   │   ├── CreateInvestmentWizard.tsx (520 lines)
│   │   │   ├── InvestmentDashboard.tsx (350 lines)
│   │   │   ├── PositionCard.tsx (230 lines)
│   │   │   └── PositionDetailsModal.tsx (400 lines)
│   │   ├── services/
│   │   │   └── investmentService.ts (140 lines, 12 methods)
│   │   ├── store/
│   │   │   └── investmentStore.ts (240 lines, 20 actions)
│   │   ├── types/
│   │   │   └── investment.ts (200 lines, 15+ interfaces)
│   │   └── routes/
│   │       └── router.tsx (updated with /investment route)
└── Planning/Sprints/N04/
    ├── SPRINT_N04_COMPLETE.md (comprehensive completion doc)
    ├── BACKEND_IMPLEMENTATION_COMPLETE.md (backend reference)
    ├── SPRINT_N04_PROGRESS_SUMMARY.md (progress tracking)
    └── INTEGRATION_GUIDE.md (this file)
```

**Total Code Metrics:**
- Backend: 22 files, ~2,500 lines
- Frontend: 11 files, ~3,100 lines
- Documentation: 5 files, ~164KB

---

## ✅ Final Sign-Off Checklist

Before marking Sprint N04 as production-ready:

### Development
- [x] All backend code complete (22 files)
- [x] All frontend code complete (11 files)
- [x] Database migration created
- [x] TypeScript compilation passes (0 errors)
- [x] Backend build succeeds (0 errors)
- [x] Frontend build succeeds (0 errors)

### Testing
- [ ] All API endpoints manually tested
- [ ] Frontend user flows tested
- [ ] Real-time updates verified
- [ ] Encryption/decryption verified
- [ ] Error handling tested
- [ ] Edge cases tested
- [ ] Performance benchmarks met
- [ ] Security audit passed

### Deployment
- [ ] Database migration applied to production
- [ ] Environment variables configured
- [ ] HashiCorp Vault configured
- [ ] Backend deployed to production
- [ ] Frontend deployed to production
- [ ] Post-deployment smoke tests passed
- [ ] Monitoring/logging configured
- [ ] Rollback plan documented

### Documentation
- [x] API documentation complete
- [x] Integration guide complete (this file)
- [x] Code comments added
- [x] User guide created
- [ ] Team training completed

---

## 🎓 Team Handoff Notes

### For QA Team

**Priority Test Areas:**
1. WhiteBit API integration (connection, plans, create, withdraw)
2. Real-time reward calculations (verify accuracy)
3. User-level encryption (verify different users have different encrypted values)
4. Background worker sync (verify updates every 60 seconds)
5. UI responsiveness (test on mobile, tablet, desktop)

**Known Limitations:**
- Background worker runs every 60 seconds (not configurable yet)
- Only WhiteBit exchange supported (architecture ready for more)
- Investment plans are fetched from WhiteBit API (no local caching)

### For DevOps Team

**Infrastructure Requirements:**
- PostgreSQL 15+ with connection pooling
- HashiCorp Vault for production secrets
- HTTPS/TLS for both API and Web
- CORS configured for frontend domain
- Reverse proxy (nginx recommended)

**Monitoring Recommendations:**
- Monitor background worker logs for errors
- Alert on API rate limit warnings (WhiteBit)
- Track position sync failures
- Monitor database connection pool usage

### For Future Developers

**Architecture Notes:**
- Service layer pattern used throughout
- Repository pattern for data access
- Zustand for state management (frontend)
- AES-256-GCM for credential encryption
- HMAC-SHA256 for API signatures

**Extension Points:**
- Add more exchanges: Implement `IExchangeApiClient` interface
- Add more investment types: Extend `InvestmentPlan` model
- Add notifications: Hook into position update events
- Add analytics: Query `investment_transactions` table

---

## 📞 Support

For questions or issues:

1. **Technical Issues:** Check the "Common Issues" section above
2. **Architecture Questions:** Review SPRINT_N04_COMPLETE.md
3. **API Reference:** See BACKEND_IMPLEMENTATION_COMPLETE.md
4. **Code Examples:** All components have inline comments

---

**Document Version:** 1.0
**Last Updated:** 2025-11-04
**Author:** Sprint N04 Implementation Team
**Status:** ✅ READY FOR INTEGRATION TESTING
