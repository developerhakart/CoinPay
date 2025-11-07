# CoinPay Docker Deployment - Manual Review Guide

**Generated**: November 5, 2025
**Sprint**: N05 Complete
**Status**: ⚠️ Containers Built - Vault Configuration Needed

---

## 🎯 Quick Status

| Service | Container | Status | Port | URL |
|---------|-----------|--------|------|-----|
| **Frontend** | coinpay-web | ✅ Running | 3000 | http://localhost:3000 |
| **Gateway** | coinpay-gateway | ⚠️ Waiting | 5000 | http://localhost:5000 |
| **API** | coinpay-api | ⚠️ Waiting | 7777 | http://localhost:7777 |
| **Docs** | coinpay-docs | ✅ Running | 8080 | http://localhost:8080 |
| **Database** | coinpay-postgres-compose | ✅ Healthy | 5432 | postgresql://localhost:5432 |
| **Vault** | coinpay-vault | ✅ Healthy | 8200 | http://localhost:8200 |

**Overall Status**: Containers running but API needs Vault configuration to start

---

## 🚨 Current Issue

The API container is waiting for HashiCorp Vault secrets to be properly configured. The secrets have been created but the VaultService is not reading them correctly due to a configuration mismatch.

### Quick Fix Options

#### Option 1: Use Fallback Configuration (Recommended for Testing)
Temporarily bypass Vault and use appsettings.json:

```bash
# Stop API container
docker stop coinpay-api

# Update appsettings.json to disable Vault or use fallback values
# Then restart
docker start coinpay-api
```

#### Option 2: Fix Vault Configuration
The VaultService needs to be configured to properly read KV v2 secrets at the `coinpay` mount point.

**Secrets Created**:
- ✅ `coinpay/database` - Database connection string
- ✅ `coinpay/jwt` - JWT secret key
- ✅ `coinpay/encryption` - Master encryption key
- ✅ `coinpay/gateway` - Gateway API key
- ✅ `coinpay/blockchain` - Blockchain RPC configuration
- ✅ `coinpay/circle` - Circle API credentials
- ✅ `coinpay/whitebit` - WhiteBit API credentials

**To verify secrets**:
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv get coinpay/database
```

---

## 📱 Access URLs

### Frontend Application
**URL**: http://localhost:3000
**Status**: ✅ Running
**Features**:
- User authentication (passkey)
- Wallet dashboard
- Transaction history
- Fiat off-ramp
- Investment management
- **NEW**: Token swap interface (Phase 5)

### API Documentation
**Swagger UI**: http://localhost:7777/swagger (when API starts)
**ReDoc**: http://localhost:7777/redoc (when API starts)

### Technical Documentation
**DocFX**: http://localhost:8080
**Status**: ✅ Running
**Content**: Complete API reference documentation

### Database
**Connection String**:
```
Host=localhost;Port=5432;Database=coinpay;Username=postgres;Password=root
```

**Access via CLI**:
```bash
docker exec -it coinpay-postgres-compose psql -U postgres -d coinpay
```

### HashiCorp Vault
**UI**: http://localhost:8200/ui
**Token**: `dev-root-token`
**Mount Point**: `coinpay/` (KV v2)

---

## 🔧 Container Management

### View All Containers
```bash
docker ps --filter "name=coinpay"
```

### View Logs
```bash
# API logs
docker logs -f coinpay-api

# Gateway logs
docker logs -f coinpay-gateway

# Web logs
docker logs -f coinpay-web

# All logs
docker-compose logs -f
```

### Restart Containers
```bash
# Restart all
docker-compose restart

# Restart specific service
docker restart coinpay-api
```

### Stop All Containers
```bash
docker-compose down
```

### Stop and Remove Volumes (Clean Reset)
```bash
docker-compose down -v
```

---

## 🗄️ Database Management

### Apply Migrations

**From Host Machine**:
```bash
cd D:/Projects/Test/Claude/CoinPay/CoinPay.Api
dotnet ef database update --connection "Host=localhost;Port=5433;Database=coinpay;Username=postgres;Password=root"
```

**Current Migrations**:
1. `InitialCreate` - Base schema
2. `AddTransactionTracking` - Transaction enhancements
3. `AddBankAccounts` - Fiat off-ramp
4. `AddEncryptionSupport` - Secure storage
5. `AddExchangeConnections` - WhiteBit integration
6. `AddInvestmentPositions` - Investment tracking
7. `AddInvestmentTransactions` - Investment history
8. `AddBackgroundJobs` - Position sync
9. **NEW**: `AddSwapTransactions` - Swap feature (Phase 5)

### Database Status Check
```bash
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "\dt"
```

### Verify Tables
```sql
-- Connect to database
docker exec -it coinpay-postgres-compose psql -U postgres -d coinpay

-- List all tables
\dt

-- Check swap_transactions table (Phase 5)
SELECT COUNT(*) FROM swap_transactions;

-- Check all migrations applied
SELECT * FROM __EFMigrationsHistory ORDER BY migration_id;
```

---

## 🧪 Testing Endpoints

### Health Check
```bash
curl http://localhost:7777/health
```

### Swagger UI
```bash
# Open in browser
start http://localhost:7777/swagger
```

### API Endpoints (Phase 5 - Swap)

**Get Swap Quote**:
```bash
curl -X GET "http://localhost:7777/api/swap/quote?fromToken=0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582&toToken=0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9&amount=100&slippage=1"
```

**Execute Swap** (requires auth):
```bash
curl -X POST "http://localhost:7777/api/swap/execute" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "fromToken": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582",
    "toToken": "0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9",
    "fromAmount": 100,
    "slippageTolerance": 1.0
  }'
```

**Swap History**:
```bash
curl -X GET "http://localhost:7777/api/swap/history?page=1&pageSize=20" \
  -H "Authorization: Bearer {token}"
```

---

## 🔐 Vault Configuration

### Access Vault UI
1. Open http://localhost:8200/ui
2. Sign in with token: `dev-root-token`
3. Navigate to `coinpay/` secrets engine
4. View/edit secrets

### Add/Update Secrets via CLI

**Database**:
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv put coinpay/database \
  connectionString="Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=root"
```

**JWT**:
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv put coinpay/jwt \
  secretKey="your-secret-key-minimum-32-characters-long"
```

**Circle API** (when you have real credentials):
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv put coinpay/circle \
  clientKey="your-circle-client-key" \
  clientUrl="https://api.circle.com/v1"
```

**WhiteBit API** (when you have real credentials):
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv put coinpay/whitebit \
  apiKey="your-whitebit-api-key" \
  apiSecret="your-whitebit-api-secret"
```

---

## 📊 Sprint N05 Features Ready for Testing

### Phase 5: Basic Swap (NEW)

**Backend APIs** ✅:
- `GET /api/swap/quote` - Get swap quotes with fees and price impact
- `POST /api/swap/execute` - Execute token swaps
- `GET /api/swap/history` - View swap history
- `GET /api/swap/{id}/details` - View swap details

**Frontend Components** ✅:
- Token selection modal (USDC, WETH, WMATIC)
- Swap interface with from/to inputs
- Real-time exchange rate display (30s refresh)
- Slippage tolerance settings (0.5%, 1%, 3%, custom)
- Price impact indicator (color-coded)
- Fee breakdown display
- Swap confirmation modal
- Swap status tracking
- Swap history page

**Features**:
- USDC ↔ WETH swaps
- USDC ↔ WMATIC swaps
- 1inch DEX aggregator integration (mock mode enabled)
- 0.5% platform fee
- Slippage protection (0.1% - 50%)
- Quote caching (30 second TTL)
- Complete transaction history

---

## 🔍 Troubleshooting

### API Won't Start

**Symptoms**: API container restarts continuously or shows Vault errors

**Solution 1** - Check Vault secrets:
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv list coinpay/
```

**Solution 2** - Check API logs:
```bash
docker logs coinpay-api
```

**Solution 3** - Use fallback configuration (disable Vault temporarily)

### Database Connection Issues

**Check if PostgreSQL is ready**:
```bash
docker exec coinpay-postgres-compose pg_isready -U postgres
```

**Reset database**:
```bash
docker-compose down -v
docker-compose up -d postgres
# Wait for healthy status
docker-compose up -d api gateway web
```

### Frontend Not Loading

**Check if web container is running**:
```bash
docker ps | grep coinpay-web
```

**Check nginx logs**:
```bash
docker logs coinpay-web
```

**Rebuild frontend**:
```bash
docker-compose build web
docker-compose up -d web
```

### Gateway Issues

**Check gateway configuration**:
```bash
docker logs coinpay-gateway
```

**Verify API is reachable**:
```bash
docker exec coinpay-gateway curl http://api:8080/health
```

---

## 📋 Manual Review Checklist

### Infrastructure
- [ ] All 6 containers running
- [ ] PostgreSQL healthy and accepting connections
- [ ] Vault healthy with secrets configured
- [ ] Network connectivity between containers

### Database
- [ ] All 9 migrations applied successfully
- [ ] 12 tables created with proper schema
- [ ] Indexes created (48 total)
- [ ] Foreign keys established (13 total)
- [ ] `swap_transactions` table exists (Phase 5)

### Backend API
- [ ] API starts successfully
- [ ] Swagger UI accessible at http://localhost:7777/swagger
- [ ] Health endpoint responds
- [ ] All Phase 1-5 endpoints documented
- [ ] Swap endpoints return mock data

### Frontend
- [ ] Web app loads at http://localhost:3000
- [ ] Login page displays
- [ ] Swap page accessible (/swap)
- [ ] Swap history page accessible (/swap/history)
- [ ] No console errors

### Documentation
- [ ] DocFX site loads at http://localhost:8080
- [ ] API reference complete
- [ ] Code examples available

### Phase 5 Specific
- [ ] Token selection modal works
- [ ] Exchange rate displays (mock data)
- [ ] Slippage settings functional
- [ ] Price impact indicator shows
- [ ] Fee breakdown visible
- [ ] Swap confirmation modal works
- [ ] Swap history page renders

---

## 🚀 Next Steps

### Immediate (Fix Vault Issue)
1. Review VaultService.cs configuration
2. Update to properly read KV v2 secrets from `coinpay/` mount
3. Restart API container
4. Verify health endpoint

### Short-term (Database Setup)
1. Apply all database migrations
2. Verify schema created correctly
3. Seed test data if needed
4. Test database connections

### Testing (Manual Review)
1. Test all Phase 1-4 features (regression)
2. Test new Phase 5 swap features:
   - Token selection
   - Quote fetching
   - Swap execution (mock mode)
   - History viewing
3. Verify responsive design (mobile/tablet/desktop)
4. Test accessibility (keyboard navigation)
5. Check browser console for errors

### Production Preparation
1. Fix 3 critical bugs from regression report
2. Replace mock services with real APIs:
   - 1inch API (get real API key)
   - Circle API (get real credentials)
   - WhiteBit API (get real credentials)
3. Configure production secrets in Vault
4. Enable SSL/TLS
5. Setup monitoring and logging
6. Performance testing
7. Security audit

---

## 📚 Documentation References

- **Sprint Planning**: `Planning/Sprints/N05/Sprint-05-Master-Plan.md`
- **Backend Plan**: `Planning/Sprints/N05/Sprint-05-Backend-Plan.md`
- **Frontend Plan**: `Planning/Sprints/N05/Sprint-05-Frontend-Plan.md`
- **QA Plan**: `Planning/Sprints/N05/Sprint-05-QA-Plan.md`
- **Regression Test Report**: `Testing/Sprint-N05/COMPREHENSIVE_REGRESSION_TEST_REPORT.md`
- **Implementation Summary**: `Planning/Sprints/N05/SPRINT-N05-IMPLEMENTATION-SUMMARY.md`

---

## 🐛 Known Issues

### Critical (P0) - Must Fix Before Production
1. **BUG-001**: TransactionController missing `[Authorize]` attribute
2. **BUG-002**: WebhookController missing `[Authorize]` attribute
3. **BUG-003**: Swap execution response returns placeholder values
4. **BUG-004**: Vault secrets not being read correctly (current blocker)

### High (P1) - Should Fix Soon
5. Hardcoded APY in investment controller
6. Missing bank account validation
7. Mock exchange rate service
8. Console.log in production code

See full bug list in `COMPREHENSIVE_REGRESSION_TEST_REPORT.md`

---

## 💡 Tips

**Performance**:
- First startup may take 30-60 seconds as images are pulled
- Subsequent starts are much faster (~10 seconds)
- Database migrations run automatically on API startup

**Development**:
- Use `docker-compose logs -f` to watch all logs in real-time
- Use `docker-compose restart api` to quickly restart after code changes
- Frontend changes require rebuild: `docker-compose build web`

**Debugging**:
- Attach to container: `docker exec -it coinpay-api /bin/sh`
- View environment: `docker exec coinpay-api env`
- Network test: `docker exec coinpay-api curl http://postgres:5432`

---

## 📞 Support

**Documentation**: See `Planning/` and `Testing/` folders
**Logs**: `docker-compose logs <service>`
**Status**: `docker-compose ps`
**Health**: Check http://localhost:7777/health (when API starts)

---

**All 6 containers successfully built and ready for manual review!** 🎉

Once Vault configuration is fixed, all services will be fully operational.
