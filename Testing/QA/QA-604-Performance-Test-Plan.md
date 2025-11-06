# QA-604: Performance Testing Plan
**CoinPay - Sprint N06 Quality Assurance**

---

## Document Information
- **Task ID**: QA-604
- **Document Type**: Performance Test Plan & Benchmarks
- **Created**: 2025-11-06
- **QA Engineer**: Claude QA Agent
- **Testing Tool**: Grafana K6
- **Status**: Ready for Execution

---

## Table of Contents
1. [Performance Targets](#1-performance-targets)
2. [Test Scenarios](#2-test-scenarios)
3. [API Endpoints to Test](#3-api-endpoints-to-test)
4. [K6 Test Scripts](#4-k6-test-scripts)
5. [Frontend Performance Metrics](#5-frontend-performance-metrics)
6. [Database Performance](#6-database-performance)
7. [Execution Schedule](#7-execution-schedule)
8. [Results Analysis](#8-results-analysis)

---

## 1. Performance Targets

### 1.1 API Performance Targets

| Metric | Target | Critical Threshold |
|--------|--------|-------------------|
| **Response Time P50** | < 500ms | < 1000ms |
| **Response Time P95** | < 2000ms | < 3000ms |
| **Response Time P99** | < 5000ms | < 8000ms |
| **API Error Rate** | < 0.1% | < 1% |
| **Throughput** | > 100 req/s | > 50 req/s |
| **Concurrent Users** | 50 users | 100 users (stress) |

### 1.2 Database Performance Targets

| Metric | Target | Critical Threshold |
|--------|--------|-------------------|
| **Query Time P95** | < 500ms | < 1000ms |
| **Query Time P99** | < 1000ms | < 2000ms |
| **Connection Pool Usage** | < 70% | < 90% |
| **Transaction Throughput** | > 200 TPS | > 100 TPS |
| **Index Hit Ratio** | > 95% | > 90% |

### 1.3 Frontend Performance Targets (Web Vitals)

| Metric | Target | Critical Threshold |
|--------|--------|-------------------|
| **First Contentful Paint (FCP)** | < 1.5s | < 2.5s |
| **Largest Contentful Paint (LCP)** | < 2.5s | < 4.0s |
| **Time to Interactive (TTI)** | < 3.0s | < 5.0s |
| **Total Blocking Time (TBT)** | < 200ms | < 500ms |
| **Cumulative Layout Shift (CLS)** | < 0.1 | < 0.25 |
| **First Input Delay (FID)** | < 100ms | < 300ms |
| **Lighthouse Performance Score** | > 90 | > 75 |

### 1.4 Network Performance Targets

| Metric | Target | Critical Threshold |
|--------|--------|-------------------|
| **API Payload Size** | < 100KB | < 500KB |
| **WebSocket Latency** | < 100ms | < 500ms |
| **Bundle Size (JS)** | < 300KB gzipped | < 500KB gzipped |
| **Bundle Size (CSS)** | < 50KB gzipped | < 100KB gzipped |
| **Image Optimization** | > 80% WebP/AVIF | > 50% modern formats |

### 1.5 Third-Party API Performance

| Service | Target Response Time | Critical Threshold |
|---------|---------------------|-------------------|
| **Circle API - Wallet Creation** | < 3s | < 10s |
| **Circle API - Transfer** | < 5s | < 15s |
| **Circle API - Balance Check** | < 2s | < 5s |
| **WhiteBit API - Authentication** | < 2s | < 5s |
| **WhiteBit API - Investment Create** | < 3s | < 10s |
| **Blockchain RPC - ETH Balance** | < 5s | < 15s |
| **Blockchain RPC - Transaction** | < 10s | < 30s |

---

## 2. Test Scenarios

### 2.1 Baseline Performance Test
**Objective**: Establish baseline performance metrics with minimal load

**Configuration**:
- Duration: 5 minutes
- Virtual Users: 1
- Ramp-up: None
- Target: Document baseline response times for all endpoints

**Success Criteria**:
- All endpoints respond successfully
- Baseline metrics documented
- No errors

---

### 2.2 Normal Load Test
**Objective**: Validate performance under typical daily traffic

**Configuration**:
- Duration: 10 minutes
- Virtual Users: 10 concurrent
- Ramp-up: 1 minute
- Steady State: 8 minutes
- Ramp-down: 1 minute

**Traffic Pattern**:
- 40% read operations (balance, history, view)
- 30% write operations (send, swap)
- 20% authentication operations
- 10% investment operations

**Success Criteria**:
- P95 response time < 2s
- Error rate < 0.1%
- All transactions complete successfully
- Database connections stable
- No memory leaks

---

### 2.3 Peak Load Test
**Objective**: Test performance during peak traffic periods

**Configuration**:
- Duration: 15 minutes
- Virtual Users: 50 concurrent
- Ramp-up: 2 minutes
- Steady State: 10 minutes
- Ramp-down: 3 minutes

**Traffic Pattern**:
- Simulate market volatility (increased swap activity)
- 30% read operations
- 40% write operations (swaps, sends)
- 20% authentication
- 10% investment operations

**Success Criteria**:
- P95 response time < 3s (acceptable degradation)
- Error rate < 1%
- System remains stable
- Graceful degradation observed
- Recovery time < 1 minute after ramp-down

---

### 2.4 Stress Test
**Objective**: Determine system breaking point and behavior under extreme load

**Configuration**:
- Duration: 20 minutes
- Virtual Users: Start at 10, increase to 100+
- Ramp-up: Incremental (add 10 users every 2 minutes)
- Steady State: 5 minutes at peak
- Ramp-down: 3 minutes

**Stages**:
1. 10 users - 2 minutes
2. 20 users - 2 minutes
3. 30 users - 2 minutes
4. 50 users - 2 minutes
5. 75 users - 2 minutes
6. 100 users - 5 minutes (peak)
7. Ramp down to 0 - 3 minutes

**Success Criteria**:
- Identify maximum sustainable user load
- Document degradation pattern
- System fails gracefully (no crashes)
- Error messages clear and helpful
- Recovery successful after load reduction
- No data corruption

---

### 2.5 Soak Test (Endurance Test)
**Objective**: Verify system stability over extended period

**Configuration**:
- Duration: 30 minutes (extended: 2 hours for full test)
- Virtual Users: 50 concurrent (moderate sustained load)
- Ramp-up: 2 minutes
- Steady State: 26 minutes
- Ramp-down: 2 minutes

**Monitoring Focus**:
- Memory usage over time
- Database connection pool
- Thread/process counts
- Disk I/O
- Network bandwidth
- API response times consistency

**Success Criteria**:
- No memory leaks (memory usage stable)
- Response times consistent throughout test
- No degradation over time
- No resource exhaustion
- Error rate remains < 0.1%
- Database connections properly released

---

### 2.6 Spike Test
**Objective**: Test system behavior during sudden traffic spikes

**Configuration**:
- Duration: 10 minutes
- Pattern: Low → Spike → Low → Spike
- Virtual Users: 5 → 100 → 5 → 100

**Stages**:
1. 5 users - 2 minutes (baseline)
2. 100 users - 2 minutes (spike)
3. 5 users - 2 minutes (recovery)
4. 100 users - 2 minutes (second spike)
5. 5 users - 2 minutes (final recovery)

**Success Criteria**:
- System handles spikes without crashing
- Auto-scaling triggers (if implemented)
- Recovery to normal within 1 minute
- No failed transactions
- Rate limiting works correctly
- Queue management effective

---

## 3. API Endpoints to Test

### 3.1 Authentication Endpoints

| Endpoint | Method | Target P95 | Expected Throughput | Payload |
|----------|--------|-----------|-------------------|---------|
| `/api/auth/register` | POST | 1000ms | 10 req/s | ~500 bytes |
| `/api/auth/login` | POST | 800ms | 50 req/s | ~300 bytes |
| `/api/auth/logout` | POST | 500ms | 30 req/s | ~100 bytes |
| `/api/auth/refresh` | POST | 600ms | 100 req/s | ~200 bytes |
| `/api/auth/reset-password` | POST | 1000ms | 5 req/s | ~400 bytes |

**Test Focus**:
- JWT token generation time
- Password hashing performance (bcrypt rounds)
- Session management overhead
- Rate limiting on login attempts

---

### 3.2 Wallet Endpoints

| Endpoint | Method | Target P95 | Expected Throughput | Payload |
|----------|--------|-----------|-------------------|---------|
| `/api/wallet/create` | POST | 3000ms | 5 req/s | ~200 bytes |
| `/api/wallet/balance/:address` | GET | 1500ms | 100 req/s | ~1KB response |
| `/api/wallet/balance/refresh/:address` | POST | 2000ms | 50 req/s | ~1KB response |
| `/api/wallet/:address` | GET | 800ms | 80 req/s | ~500 bytes |

**Test Focus**:
- Circle API integration latency
- Balance calculation performance
- Multi-token balance aggregation
- Caching effectiveness

---

### 3.3 Transaction Endpoints

| Endpoint | Method | Target P95 | Expected Throughput | Payload |
|----------|--------|-----------|-------------------|---------|
| `/api/transactions/send` | POST | 5000ms | 10 req/s | ~800 bytes |
| `/api/transactions/history` | GET | 1000ms | 100 req/s | ~5KB response |
| `/api/transactions/:id` | GET | 600ms | 80 req/s | ~1KB response |
| `/api/transactions/status/:id` | GET | 500ms | 150 req/s | ~300 bytes |

**Test Focus**:
- Transaction initiation speed
- Circle API transfer latency
- Database write performance
- Pagination efficiency
- Filtering and sorting performance

---

### 3.4 Swap Endpoints

| Endpoint | Method | Target P95 | Expected Throughput | Payload |
|----------|--------|-----------|-------------------|---------|
| `/api/swap/quote` | GET | 1500ms | 80 req/s | ~800 bytes |
| `/api/swap/execute` | POST | 6000ms | 10 req/s | ~1KB |
| `/api/swap/history` | GET | 1000ms | 50 req/s | ~5KB response |
| `/api/swap/:id` | GET | 600ms | 60 req/s | ~1KB response |

**Test Focus**:
- Quote generation speed
- Liquidity provider API latency
- Swap execution time
- Price calculation accuracy under load
- Slippage handling

---

### 3.5 Investment Endpoints

| Endpoint | Method | Target P95 | Expected Throughput | Payload |
|----------|--------|-----------|-------------------|---------|
| `/api/exchange/connect` | POST | 2000ms | 5 req/s | ~600 bytes |
| `/api/exchange/plans` | GET | 1500ms | 30 req/s | ~3KB response |
| `/api/investment/create` | POST | 3000ms | 10 req/s | ~800 bytes |
| `/api/investment/positions` | GET | 1000ms | 50 req/s | ~5KB response |
| `/api/investment/withdraw/:id` | POST | 3000ms | 10 req/s | ~600 bytes |

**Test Focus**:
- WhiteBit API integration performance
- Position synchronization time
- Reward calculation speed
- Multi-position queries

---

### 3.6 Webhook Endpoints

| Endpoint | Method | Target P95 | Expected Throughput | Payload |
|----------|--------|-----------|-------------------|---------|
| `/api/webhooks/circle` | POST | 800ms | 50 req/s | ~2KB |
| `/api/webhooks/whitebit` | POST | 800ms | 30 req/s | ~2KB |

**Test Focus**:
- Webhook processing speed
- Signature verification time
- Idempotency handling
- Database update performance

---

## 4. K6 Test Scripts

### 4.1 Main Performance Test Script

**File**: `Testing/Performance/k6-api-performance.js`

```javascript
import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js';

// Custom metrics
const errorRate = new Rate('errors');
const apiDuration = new Trend('api_duration');
const requestCounter = new Counter('requests_total');

// Test configuration
export const options = {
  scenarios: {
    // Normal Load Test
    normal_load: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '1m', target: 10 },  // Ramp up to 10 users
        { duration: '8m', target: 10 },  // Stay at 10 users
        { duration: '1m', target: 0 },   // Ramp down to 0
      ],
      gracefulRampDown: '30s',
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<2000', 'p(99)<5000'], // 95% under 2s, 99% under 5s
    http_req_failed: ['rate<0.01'],                   // Error rate < 1%
    errors: ['rate<0.1'],                             // Custom error rate < 10%
    api_duration: ['p(95)<2000'],                     // API response P95 < 2s
  },
};

// Environment configuration
const BASE_URL = __ENV.BASE_URL || 'https://api.coinpay.app';
const API_KEY = __ENV.API_KEY || '';

// Test data
const TEST_WALLET_ADDRESS = '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1';
const TEST_USER_EMAIL = `loadtest_${__VU}_${Date.now()}@example.com`;
const TEST_PASSWORD = 'LoadTest123!@#';

// Helper function to make authenticated requests
function authenticatedRequest(url, token, method = 'GET', payload = null) {
  const params = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    tags: { name: url },
  };

  let response;
  if (method === 'POST') {
    response = http.post(`${BASE_URL}${url}`, payload ? JSON.stringify(payload) : null, params);
  } else {
    response = http.get(`${BASE_URL}${url}`, params);
  }

  return response;
}

// Setup function - runs once per VU
export function setup() {
  // Optionally create test users or get auth tokens
  return {
    baseUrl: BASE_URL,
  };
}

// Main test function
export default function (data) {
  let authToken;

  group('Authentication Flow', function () {
    // Login test
    const loginPayload = {
      email: 'testuser@example.com', // Use pre-created test user
      password: 'TestPassword123!',
    };

    const loginRes = http.post(`${BASE_URL}/api/auth/login`, JSON.stringify(loginPayload), {
      headers: { 'Content-Type': 'application/json' },
      tags: { name: 'POST_/api/auth/login' },
    });

    const loginSuccess = check(loginRes, {
      'login status 200': (r) => r.status === 200,
      'login response time < 800ms': (r) => r.timings.duration < 800,
      'login returns token': (r) => r.json('token') !== undefined,
    });

    if (!loginSuccess) {
      errorRate.add(1);
      console.error(`Login failed: ${loginRes.status} - ${loginRes.body}`);
      return; // Skip rest of test for this VU iteration
    }

    authToken = loginRes.json('token');
    apiDuration.add(loginRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  if (!authToken) {
    console.error('No auth token, skipping authenticated tests');
    return;
  }

  group('Wallet Operations', function () {
    // Get wallet balance
    const balanceRes = authenticatedRequest(`/api/wallet/balance/${TEST_WALLET_ADDRESS}`, authToken);

    const balanceSuccess = check(balanceRes, {
      'balance status 200': (r) => r.status === 200,
      'balance response time < 1500ms': (r) => r.timings.duration < 1500,
      'balance has USDC': (r) => r.json('balances.USDC') !== undefined,
      'balance has ETH': (r) => r.json('balances.ETH') !== undefined,
      'balance has MATIC': (r) => r.json('balances.MATIC') !== undefined,
    });

    if (!balanceSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(balanceRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  group('Transaction History', function () {
    // Get transaction history with pagination
    const historyRes = authenticatedRequest('/api/transactions/history?page=1&pageSize=20', authToken);

    const historySuccess = check(historyRes, {
      'history status 200': (r) => r.status === 200,
      'history response time < 1000ms': (r) => r.timings.duration < 1000,
      'history returns array': (r) => Array.isArray(r.json('transactions')),
      'history has pagination': (r) => r.json('totalPages') !== undefined,
    });

    if (!historySuccess) {
      errorRate.add(1);
    }

    apiDuration.add(historyRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  group('Swap Operations', function () {
    // Get swap quote
    const quoteParams = 'fromToken=USDC&toToken=ETH&amount=100';
    const quoteRes = authenticatedRequest(`/api/swap/quote?${quoteParams}`, authToken);

    const quoteSuccess = check(quoteRes, {
      'swap quote status 200': (r) => r.status === 200,
      'swap quote response time < 1500ms': (r) => r.timings.duration < 1500,
      'quote has exchange rate': (r) => r.json('exchangeRate') !== undefined,
      'quote has estimated amount': (r) => r.json('estimatedAmount') !== undefined,
      'quote has fees': (r) => r.json('fees') !== undefined,
    });

    if (!quoteSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(quoteRes.timings.duration);
    requestCounter.add(1);

    sleep(2);
  });

  group('Investment Operations', function () {
    // Get investment plans
    const plansRes = authenticatedRequest('/api/exchange/plans', authToken);

    const plansSuccess = check(plansRes, {
      'plans status 200': (r) => r.status === 200,
      'plans response time < 1500ms': (r) => r.timings.duration < 1500,
      'plans returns array': (r) => Array.isArray(r.json('plans')),
    });

    if (!plansSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(plansRes.timings.duration);
    requestCounter.add(1);

    sleep(1);

    // Get investment positions
    const positionsRes = authenticatedRequest('/api/investment/positions', authToken);

    const positionsSuccess = check(positionsRes, {
      'positions status 200': (r) => r.status === 200,
      'positions response time < 1000ms': (r) => r.timings.duration < 1000,
      'positions returns array': (r) => Array.isArray(r.json('positions')),
    });

    if (!positionsSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(positionsRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  // Think time between iterations
  sleep(Math.random() * 3 + 2); // Random sleep between 2-5 seconds
}

// Teardown function
export function teardown(data) {
  // Cleanup if needed
}

// Generate HTML report
export function handleSummary(data) {
  return {
    'performance-report.html': htmlReport(data),
    'performance-summary.txt': textSummary(data, { indent: ' ', enableColors: true }),
    stdout: textSummary(data, { indent: ' ', enableColors: true }),
  };
}
```

---

### 4.2 Stress Test Script

**File**: `Testing/Performance/k6-stress-test.js`

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const errorRate = new Rate('errors');
const apiDuration = new Trend('api_duration');

export const options = {
  stages: [
    { duration: '2m', target: 10 },   // Ramp up to 10 users
    { duration: '2m', target: 20 },   // Ramp up to 20 users
    { duration: '2m', target: 30 },   // Ramp up to 30 users
    { duration: '2m', target: 50 },   // Ramp up to 50 users
    { duration: '2m', target: 75 },   // Ramp up to 75 users
    { duration: '5m', target: 100 },  // Ramp up to 100 users (peak stress)
    { duration: '3m', target: 0 },    // Ramp down to 0
  ],
  thresholds: {
    http_req_duration: ['p(95)<3000'], // Accept degradation: 95% under 3s
    http_req_failed: ['rate<0.05'],    // Accept 5% error rate under stress
  },
};

const BASE_URL = __ENV.BASE_URL || 'https://api.coinpay.app';

export default function () {
  // Simplified stress test - focus on key endpoints
  const responses = http.batch([
    ['GET', `${BASE_URL}/api/wallet/balance/0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1`],
    ['GET', `${BASE_URL}/api/transactions/history?page=1&pageSize=20`],
    ['GET', `${BASE_URL}/api/swap/quote?fromToken=USDC&toToken=ETH&amount=100`],
  ]);

  responses.forEach((res, index) => {
    const success = check(res, {
      'status is 200': (r) => r.status === 200,
      'response time acceptable': (r) => r.timings.duration < 5000,
    });

    if (!success) {
      errorRate.add(1);
    }

    apiDuration.add(res.timings.duration);
  });

  sleep(1);
}
```

---

### 4.3 Soak Test Script

**File**: `Testing/Performance/k6-soak-test.js`

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';

const errorRate = new Rate('errors');
const apiDuration = new Trend('api_duration');
const memoryUsage = new Trend('memory_usage');

export const options = {
  stages: [
    { duration: '2m', target: 50 },   // Ramp up to 50 users
    { duration: '26m', target: 50 },  // Stay at 50 users for 26 minutes
    { duration: '2m', target: 0 },    // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'],     // Maintain performance throughout
    http_req_failed: ['rate<0.001'],       // Very low error rate
    errors: ['rate<0.01'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'https://api.coinpay.app';

export default function () {
  // Simulate realistic user behavior over extended period
  const token = 'test_auth_token'; // Replace with actual token

  // Read operations (more frequent)
  const balanceRes = http.get(`${BASE_URL}/api/wallet/balance/0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1`);
  check(balanceRes, {
    'balance OK': (r) => r.status === 200,
  }) || errorRate.add(1);

  sleep(2);

  const historyRes = http.get(`${BASE_URL}/api/transactions/history?page=1&pageSize=20`);
  check(historyRes, {
    'history OK': (r) => r.status === 200,
  }) || errorRate.add(1);

  sleep(3);

  // Occasional write operations
  if (__ITER % 10 === 0) { // Every 10th iteration
    const quoteRes = http.get(`${BASE_URL}/api/swap/quote?fromToken=USDC&toToken=ETH&amount=50`);
    check(quoteRes, {
      'quote OK': (r) => r.status === 200,
    }) || errorRate.add(1);
  }

  sleep(5);
}
```

---

### 4.4 Spike Test Script

**File**: `Testing/Performance/k6-spike-test.js`

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const errorRate = new Rate('errors');

export const options = {
  stages: [
    { duration: '2m', target: 5 },    // Baseline: 5 users
    { duration: '30s', target: 100 }, // Spike to 100 users
    { duration: '2m', target: 5 },    // Recovery to 5 users
    { duration: '30s', target: 100 }, // Second spike to 100 users
    { duration: '2m', target: 5 },    // Final recovery
    { duration: '1m', target: 0 },    // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<4000'], // Accept temporary degradation during spikes
    http_req_failed: ['rate<0.1'],     // 10% error rate acceptable during spikes
  },
};

const BASE_URL = __ENV.BASE_URL || 'https://api.coinpay.app';

export default function () {
  const res = http.get(`${BASE_URL}/api/wallet/balance/0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1`);

  check(res, {
    'status is 200 or 429': (r) => r.status === 200 || r.status === 429, // Accept rate limiting
    'has response': (r) => r.body.length > 0,
  }) || errorRate.add(1);

  sleep(1);
}
```

---

## 5. Frontend Performance Metrics

### 5.1 Lighthouse Performance Audit

**Target Pages**:
- Landing page
- Login page
- Dashboard
- Wallet page
- Send transaction page
- Swap page
- Investment page

**Measurement Tool**: Google Lighthouse (Chrome DevTools)

**Metrics to Capture**:
```json
{
  "performance": {
    "score": "> 90",
    "firstContentfulPaint": "< 1.5s",
    "largestContentfulPaint": "< 2.5s",
    "timeToInteractive": "< 3.0s",
    "totalBlockingTime": "< 200ms",
    "cumulativeLayoutShift": "< 0.1",
    "speedIndex": "< 3.0s"
  },
  "accessibility": {
    "score": "> 95"
  },
  "bestPractices": {
    "score": "> 90"
  },
  "seo": {
    "score": "> 90"
  }
}
```

**Automated Script**: `Testing/Performance/lighthouse-audit.js`

```javascript
// Run with: node lighthouse-audit.js
const lighthouse = require('lighthouse');
const chromeLauncher = require('chrome-launcher');
const fs = require('fs');

const urls = [
  'https://coinpay.app',
  'https://coinpay.app/login',
  'https://coinpay.app/dashboard',
  'https://coinpay.app/wallet',
  'https://coinpay.app/send',
  'https://coinpay.app/swap',
  'https://coinpay.app/invest',
];

async function runLighthouse() {
  const chrome = await chromeLauncher.launch({ chromeFlags: ['--headless'] });
  const options = {
    logLevel: 'info',
    output: 'html',
    onlyCategories: ['performance', 'accessibility', 'best-practices', 'seo'],
    port: chrome.port,
  };

  const results = [];

  for (const url of urls) {
    console.log(`Auditing: ${url}`);
    const runnerResult = await lighthouse(url, options);

    const report = runnerResult.report;
    const scores = runnerResult.lhr.categories;
    const metrics = runnerResult.lhr.audits;

    const result = {
      url,
      timestamp: new Date().toISOString(),
      scores: {
        performance: scores.performance.score * 100,
        accessibility: scores.accessibility.score * 100,
        bestPractices: scores['best-practices'].score * 100,
        seo: scores.seo.score * 100,
      },
      metrics: {
        fcp: metrics['first-contentful-paint'].numericValue,
        lcp: metrics['largest-contentful-paint'].numericValue,
        tti: metrics['interactive'].numericValue,
        tbt: metrics['total-blocking-time'].numericValue,
        cls: metrics['cumulative-layout-shift'].numericValue,
        speedIndex: metrics['speed-index'].numericValue,
      },
    };

    results.push(result);

    // Save individual report
    const filename = `lighthouse-${url.replace(/[^a-z0-9]/gi, '_')}-${Date.now()}.html`;
    fs.writeFileSync(`Testing/Performance/reports/${filename}`, report);
  }

  // Save summary
  fs.writeFileSync(
    'Testing/Performance/reports/lighthouse-summary.json',
    JSON.stringify(results, null, 2)
  );

  await chrome.kill();

  console.log('Lighthouse audits completed!');
  console.log(JSON.stringify(results, null, 2));
}

runLighthouse();
```

---

### 5.2 Bundle Size Analysis

**Tool**: Webpack Bundle Analyzer

**Target Metrics**:
- Main bundle: < 300KB gzipped
- Vendor bundle: < 200KB gzipped
- CSS bundle: < 50KB gzipped
- Total initial load: < 500KB gzipped

**Analysis Command**:
```bash
npm run build -- --analyze
```

**Optimization Checklist**:
- [ ] Code splitting implemented
- [ ] Lazy loading for routes
- [ ] Tree shaking enabled
- [ ] Compression (gzip/brotli) enabled
- [ ] Source maps excluded from production
- [ ] Unused dependencies removed
- [ ] Images optimized (WebP/AVIF)
- [ ] Fonts optimized (woff2, subsetting)

---

## 6. Database Performance

### 6.1 Query Performance Analysis

**Tool**: PostgreSQL EXPLAIN ANALYZE

**Critical Queries to Profile**:

1. **User Authentication Query**
```sql
EXPLAIN ANALYZE
SELECT id, email, password_hash, created_at
FROM users
WHERE email = 'testuser@example.com';
-- Target: < 10ms
```

2. **Wallet Balance Query**
```sql
EXPLAIN ANALYZE
SELECT w.*, b.token, b.balance, b.updated_at
FROM wallets w
LEFT JOIN balances b ON w.id = b.wallet_id
WHERE w.address = '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1';
-- Target: < 50ms
```

3. **Transaction History Query (Paginated)**
```sql
EXPLAIN ANALYZE
SELECT id, wallet_id, token, amount, type, status, created_at
FROM transactions
WHERE wallet_id = 'wallet_id_123'
ORDER BY created_at DESC
LIMIT 20 OFFSET 0;
-- Target: < 100ms
```

4. **Swap History Query**
```sql
EXPLAIN ANALYZE
SELECT s.*, t.status, t.created_at
FROM swaps s
JOIN transactions t ON s.transaction_id = t.id
WHERE s.user_id = 'user_id_123'
ORDER BY t.created_at DESC
LIMIT 20;
-- Target: < 100ms
```

5. **Investment Positions Query**
```sql
EXPLAIN ANALYZE
SELECT i.*, p.name, p.apy
FROM investments i
JOIN investment_plans p ON i.plan_id = p.id
WHERE i.user_id = 'user_id_123'
AND i.status = 'active';
-- Target: < 50ms
```

### 6.2 Index Optimization

**Required Indexes**:
```sql
-- Users table
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_created_at ON users(created_at);

-- Wallets table
CREATE INDEX idx_wallets_address ON wallets(address);
CREATE INDEX idx_wallets_user_id ON wallets(user_id);

-- Transactions table
CREATE INDEX idx_transactions_wallet_id ON transactions(wallet_id);
CREATE INDEX idx_transactions_created_at ON transactions(created_at);
CREATE INDEX idx_transactions_status ON transactions(status);
CREATE INDEX idx_transactions_type ON transactions(type);
CREATE INDEX idx_transactions_wallet_created ON transactions(wallet_id, created_at DESC);

-- Swaps table
CREATE INDEX idx_swaps_user_id ON swaps(user_id);
CREATE INDEX idx_swaps_transaction_id ON swaps(transaction_id);
CREATE INDEX idx_swaps_created_at ON swaps(created_at);

-- Investments table
CREATE INDEX idx_investments_user_id ON investments(user_id);
CREATE INDEX idx_investments_status ON investments(status);
CREATE INDEX idx_investments_plan_id ON investments(plan_id);
```

### 6.3 Connection Pool Monitoring

**Metrics to Track**:
- Active connections
- Idle connections
- Waiting connections
- Connection acquisition time
- Connection lifetime

**Target Configuration**:
```javascript
{
  max: 20,           // Maximum pool size
  min: 5,            // Minimum pool size
  idle: 10000,       // Idle timeout: 10 seconds
  acquire: 30000,    // Acquire timeout: 30 seconds
  evict: 60000       // Eviction interval: 60 seconds
}
```

---

## 7. Execution Schedule

### 7.1 Pre-Production Testing Schedule

| Test Type | Frequency | Duration | When to Run |
|-----------|-----------|----------|-------------|
| Baseline Test | Once | 5 min | Before each sprint |
| Normal Load Test | Daily | 10 min | During development |
| Peak Load Test | Weekly | 15 min | End of week |
| Stress Test | Before release | 20 min | Pre-production |
| Soak Test | Before major release | 30-120 min | Pre-production |
| Spike Test | Before release | 10 min | Pre-production |
| Lighthouse Audit | Daily | 15 min | Automated CI/CD |
| Database Profiling | Weekly | 30 min | Off-peak hours |

### 7.2 Production Monitoring

**Continuous Monitoring Metrics**:
- API response times (P50, P95, P99)
- Error rates (4xx, 5xx)
- Request throughput
- Database query performance
- Cache hit rates
- Memory usage
- CPU usage
- Disk I/O

**Alerting Thresholds**:
- P95 response time > 3s: Warning
- P99 response time > 8s: Critical
- Error rate > 1%: Warning
- Error rate > 5%: Critical
- Database connection pool > 90%: Warning
- Memory usage > 90%: Critical

---

## 8. Results Analysis

### 8.1 Performance Report Template

```markdown
# Performance Test Results
Date: YYYY-MM-DD
Test Type: [Baseline / Normal Load / Peak Load / Stress / Soak / Spike]
Environment: [Staging / Production]

## Summary
- Test Duration: XX minutes
- Virtual Users: XX
- Total Requests: XXXX
- Error Rate: X.XX%
- Test Status: PASS / FAIL

## Key Metrics
| Metric | Target | Actual | Status |
|--------|--------|--------|--------|
| P50 Response Time | < 500ms | XXX ms | ✅/❌ |
| P95 Response Time | < 2000ms | XXX ms | ✅/❌ |
| P99 Response Time | < 5000ms | XXX ms | ✅/❌ |
| Error Rate | < 0.1% | X.XX% | ✅/❌ |
| Throughput | > 100 req/s | XXX req/s | ✅/❌ |

## Endpoint Performance
| Endpoint | P95 | P99 | Error Rate | Status |
|----------|-----|-----|------------|--------|
| /api/auth/login | XXX ms | XXX ms | X.XX% | ✅/❌ |
| /api/wallet/balance | XXX ms | XXX ms | X.XX% | ✅/❌ |
| /api/transactions/send | XXX ms | XXX ms | X.XX% | ✅/❌ |

## Issues Identified
1. [Issue description]
   - Severity: High/Medium/Low
   - Affected endpoints: [list]
   - Recommended action: [action]

## Recommendations
1. [Recommendation 1]
2. [Recommendation 2]

## Conclusion
[Overall assessment and production readiness recommendation]
```

### 8.2 Bottleneck Identification

**Common Performance Bottlenecks**:

1. **Slow Database Queries**
   - Symptoms: High P99 latency, slow response times
   - Detection: Query logs, EXPLAIN ANALYZE
   - Solution: Add indexes, optimize queries, implement caching

2. **External API Latency**
   - Symptoms: Circle/WhiteBit API calls taking > 3s
   - Detection: API response time monitoring
   - Solution: Implement timeout handling, caching, retry logic

3. **Memory Leaks**
   - Symptoms: Increasing memory usage during soak test
   - Detection: Memory profiling over time
   - Solution: Review event listeners, cache management, object pooling

4. **N+1 Query Problem**
   - Symptoms: Multiple sequential database queries
   - Detection: Database query logs
   - Solution: Use JOIN queries, eager loading, DataLoader pattern

5. **Large Payload Sizes**
   - Symptoms: Slow network transfer times
   - Detection: Network tab, payload size analysis
   - Solution: Implement pagination, compress responses, filter unnecessary data

6. **Inefficient Algorithms**
   - Symptoms: High CPU usage, slow processing
   - Detection: CPU profiling
   - Solution: Optimize algorithms, use more efficient data structures

### 8.3 Optimization Priorities

**Priority 1 (Critical Path)**:
- Authentication endpoints (login, register)
- Wallet balance retrieval
- Transaction send/receive
- Swap quote and execution

**Priority 2 (High Traffic)**:
- Transaction history
- Dashboard data aggregation
- Swap history

**Priority 3 (Lower Frequency)**:
- Investment operations
- Admin endpoints
- Report generation

---

## 9. K6 Execution Commands

### 9.1 Running Tests Locally

```bash
# Normal Load Test
k6 run Testing/Performance/k6-api-performance.js

# Stress Test
k6 run Testing/Performance/k6-stress-test.js

# Soak Test
k6 run Testing/Performance/k6-soak-test.js

# Spike Test
k6 run Testing/Performance/k6-spike-test.js

# With custom environment variables
k6 run -e BASE_URL=https://api.coinpay.app -e API_KEY=your_key Testing/Performance/k6-api-performance.js

# With results output to JSON
k6 run --out json=results.json Testing/Performance/k6-api-performance.js

# With cloud execution (K6 Cloud)
k6 cloud Testing/Performance/k6-api-performance.js
```

### 9.2 CI/CD Integration

**GitHub Actions Workflow**: `.github/workflows/performance-test.yml`

```yaml
name: Performance Tests

on:
  schedule:
    - cron: '0 2 * * *' # Daily at 2 AM
  workflow_dispatch:

jobs:
  performance-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Install K6
        run: |
          sudo gpg -k
          sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
          echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
          sudo apt-get update
          sudo apt-get install k6

      - name: Run Performance Tests
        env:
          BASE_URL: ${{ secrets.STAGING_API_URL }}
          API_KEY: ${{ secrets.API_KEY }}
        run: |
          k6 run --out json=performance-results.json Testing/Performance/k6-api-performance.js

      - name: Upload Results
        uses: actions/upload-artifact@v3
        with:
          name: performance-results
          path: |
            performance-results.json
            performance-report.html
            performance-summary.txt

      - name: Check Thresholds
        run: |
          # Parse results and fail job if thresholds not met
          if grep -q '"failed":true' performance-results.json; then
            echo "Performance tests failed!"
            exit 1
          fi
```

---

## 10. Production Readiness Checklist

### Performance Criteria
- [ ] All P95 response times < 2s
- [ ] All P99 response times < 5s
- [ ] Error rate < 0.1% under normal load
- [ ] Error rate < 1% under peak load
- [ ] System handles 50 concurrent users smoothly
- [ ] No memory leaks in 30-minute soak test
- [ ] Database queries optimized (< 500ms P95)
- [ ] All critical indexes created
- [ ] Frontend Lighthouse score > 90
- [ ] Bundle sizes within targets
- [ ] CDN configured for static assets
- [ ] Compression enabled (gzip/brotli)
- [ ] Rate limiting implemented
- [ ] Monitoring and alerting configured
- [ ] Load balancing configured (if applicable)
- [ ] Auto-scaling configured (if applicable)
- [ ] Stress test identifies breaking point
- [ ] System recovers gracefully from overload

---

**Document End**

*This performance testing plan provides comprehensive coverage of CoinPay's performance requirements. Execute tests systematically, monitor results, optimize identified bottlenecks, and ensure performance targets are met before production deployment.*
