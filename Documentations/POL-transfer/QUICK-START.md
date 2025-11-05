# Quick Start: Test 0.001 POL Transfer

## ✅ What Works Right Now (No Setup Needed)

- ✅ **Balance Checks** - Check any wallet balance immediately
- ✅ **API Endpoints** - All read operations work
- ✅ **Mock Transfers** - Test transfer logic without blockchain

## ⏳ For Real Blockchain Transfers (Requires Setup)

### Prerequisites (5 minutes)

1. **PostgreSQL running** → `docker-compose up postgres -d`
2. **Test wallet with POL** → https://faucet.polygon.technology/
3. **Private key exported** → MetaMask → Account Details → Export Private Key

## Test in 3 Commands

```powershell
# 1. Set private key
$env:TEST_WALLET_PRIVATE_KEY = "your_64_character_private_key"

# 2. Start API (in one terminal)
cd CoinPay.Api
dotnet run

# 3. Run test (in another terminal)
cd ..
.\Test-POLTransfer.ps1
```

## Expected Result

```
[SUCCESS] Transfer initiated!
Transaction ID: 0x1234abcd5678ef...  ← Real blockchain hash
Status:         Confirmed
Amount:         0.001 POL

Verify on PolygonScan:
https://amoy.polygonscan.com/tx/0x1234abcd...
```

## Verification

Visit: https://amoy.polygonscan.com/address/0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579

You should see your 0.001 POL transfer! 🎉

## Documentation

- Full Guide: `POL-TRANSFER-TEST-GUIDE.md`
- Implementation Details: `IMPLEMENTATION-SUMMARY.md`
- Status Report: `FINAL-STATUS-REPORT.md`

## Support

**Issue**: PostgreSQL not running
**Fix**: `docker-compose up postgres -d`

**Issue**: Test user doesn't exist
**Fix**: Script creates it automatically

**Issue**: Insufficient POL
**Fix**: Get from https://faucet.polygon.technology/

**Issue**: "Mock transaction" instead of real
**Fix**: Set `$env:TEST_WALLET_PRIVATE_KEY`

---

**🎯 That's it! Three commands to test real POL blockchain transfers.**
