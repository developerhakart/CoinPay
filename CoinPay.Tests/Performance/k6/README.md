# CoinPay Performance Testing - K6

**QA-205: Performance Testing (100+ users)**

Comprehensive performance test suite using K6 to validate system performance under load.

---

## 📋 Test Suite Overview

### Load Test (load-test.js)
**Purpose**: Validate system performance under expected load

**Configuration**:
- Ramp-up: 0 → 50 → 100 → 150 users
- Duration: ~17 minutes
- Steady state: 5 minutes at peak
- Think time: 1-5 seconds between requests

**Performance Targets**:
- P95 balance check: < 1 second
- P95 transaction list: < 1.5 seconds
- P95 transfer submit: < 2 seconds
- Error rate: < 1%
- Transfer success rate: > 95%

---

### Stress Test (stress-test.js)
**Purpose**: Find system breaking points

**Configuration**:
- Progressive load: 50 → 100 → 200 → 300 → 400 users
- Duration: ~30 minutes
- Aggressive request patterns (rapid-fire)
- Recovery test at end

**Acceptable Degradation**:
- P99 response time: < 5 seconds
- Error rate: < 10% at peak stress
- System should recover after load drops

---

### Spike Test (spike-test.js)
**Purpose**: Test sudden traffic surges

**Configuration**:
- Normal: 20 users
- Spike 1: 20 → 200 users (10x increase in 10s)
- Spike 2: 20 → 300 users (15x increase in 10s)
- Duration: ~8 minutes

**Recovery Metrics**:
- System responds during spike
- Error rate: < 5%
- System recovers after spike
- No cascading failures

---

## 🚀 Installation

### Prerequisites

- K6 installed ([Installation Guide](https://k6.io/docs/getting-started/installation/))
- CoinPay backend running on http://localhost:5000
- Test user accounts created

### Install K6

**macOS**:
```bash
brew install k6
```

**Windows** (Chocolatey):
```bash
choco install k6
```

**Linux**:
```bash
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

**Docker**:
```bash
docker pull grafana/k6
```

---

## 🧪 Running Tests

### Load Test

```bash
# Run load test
k6 run load-test.js

# Run with custom base URL
k6 run --env BASE_URL=https://api.coinpay.app load-test.js

# Run with HTML report
k6 run --out html=report.html load-test.js
```

### Stress Test

```bash
# Run stress test
k6 run stress-test.js

# Monitor in real-time
k6 run --out json=metrics.json stress-test.js
```

### Spike Test

```bash
# Run spike test
k6 run spike-test.js
```

### All Tests

```bash
# Run all performance tests
./run-all-tests.sh

# Or manually:
k6 run load-test.js
k6 run stress-test.js
k6 run spike-test.js
```

---

## 📊 Understanding Results

### Key Metrics

#### HTTP Request Duration
```
http_req_duration..............: avg=245ms min=50ms med=200ms max=2s p(90)=450ms p(95)=650ms
```

- **avg**: Average response time
- **med**: Median (50th percentile)
- **p(90)**: 90th percentile - 90% of requests faster than this
- **p(95)**: 95th percentile - performance target
- **p(99)**: 99th percentile - worst case (excluding outliers)

#### Request Rate
```
http_reqs......................: 12000   100/s
```

- Total requests and requests per second

#### Error Rate
```
http_req_failed................: 0.45%   54/12000
```

- Percentage of failed requests
- Target: < 1% for production

#### Custom Metrics
```
transfer_success_rate..........: 97.5%
balance_check_duration.........: avg=150ms p(95)=800ms
```

---

## 📈 Performance Reports

### HTML Report

```bash
# Generate HTML report
k6 run --out html=performance-report.html load-test.js

# Open report
open performance-report.html  # macOS
start performance-report.html # Windows
xdg-open performance-report.html # Linux
```

### JSON Export

```bash
# Export metrics to JSON
k6 run --out json=metrics.json load-test.js

# Parse with jq
cat metrics.json | jq '.metrics.http_req_duration'
```

### InfluxDB + Grafana

```bash
# Run with InfluxDB output
k6 run --out influxdb=http://localhost:8086/k6 load-test.js

# View in Grafana dashboard
```

### K6 Cloud

```bash
# Run with cloud monitoring
k6 cloud load-test.js

# Or stream to cloud
k6 run --out cloud load-test.js
```

---

## 🎯 Performance Targets

### Critical Endpoints

| Endpoint | P95 Target | P99 Target | Error Rate |
|----------|------------|------------|------------|
| GET /api/wallet/balance | < 1s | < 2s | < 0.5% |
| GET /api/transactions/history | < 1.5s | < 3s | < 0.5% |
| POST /api/transfer/submit | < 2s | < 4s | < 1% |
| GET /api/transactions/{id}/status | < 1s | < 2s | < 0.5% |
| GET /api/transactions/{id}/details | < 1s | < 2s | < 0.5% |

### System Capacity

| Metric | Target | Acceptable |
|--------|--------|------------|
| Concurrent Users | 150+ | 200+ |
| Requests/Second | 100+ | 150+ |
| Average CPU Usage | < 70% | < 85% |
| Average Memory | < 70% | < 85% |
| Database Connections | < 80% pool | < 90% pool |

---

## 🔧 Configuration

### Environment Variables

```bash
# Set base URL
export BASE_URL=http://localhost:5000

# Set frontend URL
export FRONTEND_URL=http://localhost:3000

# Run test
k6 run load-test.js
```

### Custom Options

Modify in test file:

```javascript
export const options = {
  stages: [
    { duration: '1m', target: 50 },  // Customize duration and target
  ],
  thresholds: {
    'http_req_duration': ['p(95)<1000'],  // Customize thresholds
  },
};
```

---

## 🐛 Troubleshooting

### High Error Rates

**Issue**: Error rate > 5%
**Solutions**:
- Check API logs for errors
- Verify database connections not exhausted
- Check rate limiting configuration
- Review server resources (CPU, memory)

### Slow Response Times

**Issue**: P95 > threshold
**Solutions**:
- Check database query performance
- Review Redis cache hit rate
- Check RPC provider rate limits
- Review application logging overhead

### Connection Timeouts

**Issue**: Requests timing out
**Solutions**:
- Increase timeout in K6 script
- Check server timeout configuration
- Verify network connectivity
- Review connection pool settings

### K6 Installation Issues

**Issue**: K6 not found
**Solution**:
```bash
# Verify installation
k6 version

# Reinstall if needed
brew reinstall k6  # macOS
```

---

## 📚 Test Scenarios

### User Workflow (Load Test)

1. **Login** (100% of users)
   - POST /api/auth/login
   - Receive auth token

2. **Check Balance** (100% of users, frequent)
   - GET /api/wallet/balance
   - Cache hit rate monitored

3. **View Transactions** (80% of users)
   - GET /api/transactions/history
   - Pagination tested

4. **Submit Transfer** (30% of users)
   - POST /api/transfer/submit
   - Check transaction status

5. **View Details** (50% of users)
   - GET /api/transactions/{id}/details

### Think Time

Realistic pauses between requests:
- After login: 1-2s
- Between balance checks: 3-5s
- After viewing transactions: 2-4s
- After transfer: 5-10s

---

## 📊 Sample Results

### Successful Load Test

```
✓ login status is 200
✓ balance check status is 200
✓ balance response time < 1s
✓ transactions status is 200
✓ transfer status is 200 or 201

http_req_duration..............: avg=356ms min=102ms med=298ms max=1.8s p(95)=842ms
http_req_failed................: 0.23% 28/12000
transfer_success_rate..........: 98.5%
balance_check_duration.........: avg=245ms p(95)=687ms
transaction_list_duration......: avg=412ms p(95)=1.2s

Total Requests: 12,000
Requests/Second: 95
Duration: 17m 30s
Peak Users: 150
```

### Failed Load Test (Example)

```
✗ balance response time < 1s
  ↳  85% — 1700 / 2000

http_req_duration..............: avg=1.8s min=150ms med=1.5s max=8s p(95)=3.2s ❌
http_req_failed................: 5.2% ❌ 624/12000
transfer_success_rate..........: 88.5% ❌

Issues Found:
- P95 response time exceeds 1s threshold
- Error rate > 1% threshold
- Transfer success rate < 95%
```

**Action Items**:
- Investigate slow queries
- Check database connection pool
- Review application logs
- Optimize expensive operations

---

## 🎨 Best Practices

### Test Data

- Use dedicated test accounts
- Pre-populate data for realistic scenarios
- Clean up test data between runs
- Use unique identifiers to avoid collisions

### Test Environment

- Run tests in isolated environment
- Match production configuration
- Use production-like data volumes
- Monitor server resources during tests

### Baseline Testing

1. Run baseline test with no load
2. Run with expected load (50-100 users)
3. Run with peak load (100-150 users)
4. Compare results over time

### Continuous Testing

- Run nightly performance tests
- Compare against baseline
- Alert on regression (>10% slowdown)
- Track trends over sprints

---

## 📞 Support

For issues or questions:
- Review K6 documentation: https://k6.io/docs/
- Check server logs for errors
- Monitor system resources
- Contact DevOps team for infrastructure issues

---

## 📄 License

MIT License - See LICENSE file for details

---

**Last Updated**: 2025-10-29
**Version**: 1.0.0
**Sprint**: N02 - QA Phase 2 (Optional)
**Target**: 150+ concurrent users, P95 < 1s
