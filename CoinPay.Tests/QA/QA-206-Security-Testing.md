# QA-206: Security Testing (OWASP)

**Test Owner**: Security Engineer / QA Engineer 2
**Effort**: 2.00 days
**Status**: Ready for Execution
**Date Created**: 2025-10-29
**Priority**: CRITICAL

---

## Test Objectives

Comprehensive security assessment of CoinPay application:
- OWASP Top 10 vulnerability testing
- Authentication security (Passkey/WebAuthn)
- API security testing
- Smart contract security review
- Blockchain-specific security
- Data protection and privacy
- Infrastructure security

---

## Test Environment

**Frontend**: http://localhost:3000
**Backend API**: http://localhost:5000
**Network**: Polygon Amoy Testnet
**Tools**: OWASP ZAP, Burp Suite, npm audit, Slither, MythX

---

## OWASP Top 10 (2021) Security Testing

### 1. A01:2021 - Broken Access Control

#### SEC-001: Horizontal Privilege Escalation
**Priority**: CRITICAL
**Risk**: High

**Attack Vector**:
1. Login as User A
2. Capture User A's transaction ID or wallet ID
3. Login as User B
4. Attempt to access User A's resources using User B's session

**Test Cases**:
- [ ] Try accessing another user's wallet balance: `GET /api/wallet/balance?userId=<other-user-id>`
- [ ] Try viewing another user's transactions: `GET /api/transactions?userId=<other-user-id>`
- [ ] Try initiating transfer from another user's wallet
- [ ] Try accessing another merchant's dashboard

**Expected Result**:
- ✅ All requests return 403 Forbidden or 401 Unauthorized
- ✅ No data from other users accessible
- ✅ Server-side authorization checks in place

**Status**: ⏳ Pending

---

#### SEC-002: Vertical Privilege Escalation
**Priority**: CRITICAL
**Risk**: High

**Attack Vector**:
1. Login as regular customer
2. Attempt to access admin/merchant endpoints

**Test Cases**:
- [ ] Try accessing admin panel: `GET /admin/dashboard`
- [ ] Try accessing merchant endpoints: `GET /merchant/analytics`
- [ ] Try modifying admin settings
- [ ] Try escalating user role via API

**Expected Result**:
- ✅ All unauthorized requests blocked
- ✅ 403 Forbidden responses
- ✅ Role-based access control enforced

**Status**: ⏳ Pending

---

#### SEC-003: Insecure Direct Object References (IDOR)
**Priority**: CRITICAL
**Risk**: High

**Attack Vector**:
1. Enumerate transaction IDs, user IDs, wallet addresses
2. Try accessing objects by manipulating IDs

**Test Cases**:
- [ ] `GET /api/transactions/1`, `/api/transactions/2`, etc.
- [ ] `GET /api/users/1`, `/api/users/2`, etc.
- [ ] `GET /api/wallet/details/{address}` with other addresses
- [ ] Sequential ID guessing attacks

**Expected Result**:
- ✅ UUIDs or non-sequential IDs used
- ✅ Authorization checks on every object access
- ✅ No information disclosure via error messages

**Status**: ⏳ Pending

---

### 2. A02:2021 - Cryptographic Failures

#### SEC-004: Data in Transit Encryption
**Priority**: CRITICAL
**Risk**: High

**Test Cases**:
- [ ] Verify HTTPS enforced on all pages
- [ ] Check for HTTP Strict Transport Security (HSTS) header
- [ ] Verify TLS 1.2+ used (not TLS 1.0/1.1)
- [ ] Check certificate validity
- [ ] Verify no mixed content warnings

**Expected Result**:
- ✅ All traffic over HTTPS
- ✅ HSTS header present: `Strict-Transport-Security: max-age=31536000; includeSubDomains`
- ✅ Valid SSL/TLS certificate
- ✅ Strong cipher suites only

**Status**: ⏳ Pending

---

#### SEC-005: Sensitive Data Storage
**Priority**: CRITICAL
**Risk**: High

**Test Cases**:
- [ ] Verify passwords NOT stored (passkey-only auth)
- [ ] Check if private keys stored securely (should be in smart wallet)
- [ ] Verify session tokens encrypted
- [ ] Check localStorage/sessionStorage for sensitive data
- [ ] Verify database encryption at rest

**Expected Result**:
- ✅ No passwords stored (passkey authentication)
- ✅ Private keys in smart contract wallet (not in database)
- ✅ Session tokens hashed/encrypted
- ✅ No sensitive data in client-side storage
- ✅ Database fields encrypted

**Status**: ⏳ Pending

---

#### SEC-006: Cryptographic Key Management
**Priority**: CRITICAL
**Risk**: High

**Test Cases**:
- [ ] Verify API keys not hardcoded in source
- [ ] Check .env files not committed to git
- [ ] Verify Paymaster private key stored securely
- [ ] Check smart contract deployment keys
- [ ] Verify Chainlink oracle keys

**Expected Result**:
- ✅ All secrets in environment variables
- ✅ .env files in .gitignore
- ✅ Key rotation policy in place
- ✅ No keys in source code or logs

**Status**: ⏳ Pending

---

### 3. A03:2021 - Injection

#### SEC-007: SQL Injection
**Priority**: CRITICAL
**Risk**: High

**Attack Vectors**:
1. Username field: `' OR '1'='1`
2. Amount field: `'; DROP TABLE transactions--`
3. Search fields: `%'; DELETE FROM users WHERE '1'='1`

**Test Cases**:
- [ ] Test all input fields with SQL injection payloads
- [ ] Test search/filter parameters
- [ ] Test JSON API bodies
- [ ] Use automated tool: sqlmap

**Expected Result**:
- ✅ Parameterized queries used (Entity Framework protects)
- ✅ No SQL errors in responses
- ✅ Input validation and sanitization

**Status**: ⏳ Pending

---

#### SEC-008: Cross-Site Scripting (XSS)
**Priority**: CRITICAL
**Risk**: High

**Attack Vectors**:
1. Reflected XSS: `<script>alert('XSS')</script>`
2. Stored XSS in transaction notes/descriptions
3. DOM-based XSS via URL parameters

**Test Cases**:
- [ ] Inject `<script>alert(document.cookie)</script>` in all text fields
- [ ] Try `<img src=x onerror=alert(1)>`
- [ ] Test transaction notes/descriptions
- [ ] Test URL parameters: `/transfer?note=<script>...`
- [ ] Check for Content Security Policy (CSP) header

**Expected Result**:
- ✅ All input sanitized/escaped
- ✅ React's built-in XSS protection active
- ✅ CSP header present
- ✅ No script execution from user input

**Status**: ⏳ Pending

---

#### SEC-009: Command Injection
**Priority**: HIGH
**Risk**: Medium

**Attack Vectors**:
1. File upload fields
2. Export functionality
3. Any field that triggers system commands

**Test Cases**:
- [ ] Try uploading malicious files
- [ ] Test CSV export with injection: `=cmd|'/c calc'!A1`
- [ ] Test PDF generation with payloads

**Expected Result**:
- ✅ File upload validation strict
- ✅ CSV formula injection prevented
- ✅ No system command execution from user input

**Status**: ⏳ Pending

---

### 4. A04:2021 - Insecure Design

#### SEC-010: Business Logic Vulnerabilities
**Priority**: CRITICAL
**Risk**: High

**Test Cases**:
- [ ] **Negative amount transfer**: Try sending -100 USDC to gain money
- [ ] **Race condition**: Send same funds twice simultaneously
- [ ] **Integer overflow**: Try transferring amount > max uint256
- [ ] **Refund abuse**: Request multiple refunds for same transaction
- [ ] **Recurring payment manipulation**: Cancel and re-create to reset count

**Expected Result**:
- ✅ Amount validation (positive, within limits)
- ✅ Idempotency tokens for transactions
- ✅ Safe math libraries used
- ✅ Refund once per transaction
- ✅ State machine for recurring payments

**Status**: ⏳ Pending

---

#### SEC-011: Rate Limiting and DoS Protection
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
- [ ] Send 100 requests/second to login endpoint
- [ ] Send 1000 requests/second to transfer endpoint
- [ ] Attempt to exhaust database connections
- [ ] Large payload attacks (10MB JSON body)

**Expected Result**:
- ✅ Rate limiting: 10 requests/minute for login
- ✅ Rate limiting: 20 requests/minute for transfers
- ✅ 429 Too Many Requests response
- ✅ Request size limits enforced
- ✅ Connection pooling configured

**Status**: ⏳ Pending

---

### 5. A05:2021 - Security Misconfiguration

#### SEC-012: Security Headers
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
- [ ] Check X-Frame-Options header (prevent clickjacking)
- [ ] Check X-Content-Type-Options header
- [ ] Check Content-Security-Policy header
- [ ] Check Referrer-Policy header
- [ ] Check Permissions-Policy header

**Expected Result**:
```
X-Frame-Options: DENY
X-Content-Type-Options: nosniff
Content-Security-Policy: default-src 'self'; script-src 'self'; object-src 'none'
Referrer-Policy: no-referrer
Permissions-Policy: geolocation=(), microphone=(), camera=()
```

**Status**: ⏳ Pending

---

#### SEC-013: Error Handling and Information Disclosure
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
- [ ] Trigger 404 errors, check for stack traces
- [ ] Trigger 500 errors, check for sensitive info
- [ ] Check API error responses
- [ ] Verify detailed errors not in production

**Expected Result**:
- ✅ Generic error messages to users
- ✅ No stack traces in responses
- ✅ Detailed logs server-side only
- ✅ Debug mode disabled in production

**Status**: ⏳ Pending

---

#### SEC-014: Default Credentials and Configuration
**Priority**: CRITICAL
**Risk**: High

**Test Cases**:
- [ ] Check for default admin credentials
- [ ] Verify database default passwords changed
- [ ] Check swagger/API docs authentication
- [ ] Verify CORS policy restrictive

**Expected Result**:
- ✅ No default credentials
- ✅ Strong passwords enforced
- ✅ API docs require authentication
- ✅ CORS: specific origins only

**Status**: ⏳ Pending

---

### 6. A06:2021 - Vulnerable and Outdated Components

#### SEC-015: Dependency Vulnerability Scanning
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
```bash
# Frontend
cd CoinPay.Web
npm audit
npm audit fix

# Backend
cd CoinPay.Api
dotnet list package --vulnerable

# Check for outdated packages
npm outdated
dotnet outdated
```

**Expected Result**:
- ✅ 0 critical vulnerabilities
- ✅ 0 high vulnerabilities
- ✅ All packages up to date
- ✅ Dependabot enabled

**Status**: ⏳ Pending

---

### 7. A07:2021 - Identification and Authentication Failures

#### SEC-016: Passkey/WebAuthn Security
**Priority**: CRITICAL
**Risk**: High

**Test Cases**:
- [ ] Verify passkey creation requires user presence
- [ ] Check passkey challenge randomness (128+ bits)
- [ ] Test passkey replay attack prevention
- [ ] Verify attestation validation
- [ ] Check user verification requirement

**Expected Result**:
- ✅ User presence required (physical interaction)
- ✅ Challenge is cryptographically random
- ✅ Challenges expire after 5 minutes
- ✅ Signature verification on server
- ✅ Attestation format validated

**Status**: ⏳ Pending

---

#### SEC-017: Session Management
**Priority**: CRITICAL
**Risk**: High

**Test Cases**:
- [ ] Check session timeout (idle 30 minutes)
- [ ] Verify session token randomness (256+ bits)
- [ ] Test session fixation attack
- [ ] Verify logout invalidates token
- [ ] Check concurrent session limits

**Expected Result**:
- ✅ Sessions expire after 30 minutes idle
- ✅ Tokens cryptographically secure
- ✅ New token after authentication
- ✅ Token revoked on logout
- ✅ Max 5 concurrent sessions per user

**Status**: ⏳ Pending

---

#### SEC-018: Brute Force Protection
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
- [ ] Attempt 100 login failures
- [ ] Try account enumeration via login
- [ ] Test CAPTCHA after N failures
- [ ] Verify temporary lockout

**Expected Result**:
- ✅ Account locked after 10 failed attempts
- ✅ Generic error: "Invalid credentials" (no enumeration)
- ✅ CAPTCHA shown after 3 failures
- ✅ Lockout for 15 minutes

**Status**: ⏳ Pending

---

### 8. A08:2021 - Software and Data Integrity Failures

#### SEC-019: Smart Contract Integrity
**Priority**: CRITICAL
**Risk**: HIGH

**Test Cases**:
- [ ] Verify smart contracts verified on Polygonscan
- [ ] Check for proxy pattern vulnerabilities
- [ ] Test upgrade mechanism security
- [ ] Verify contract ownership transfer protection

**Expected Result**:
- ✅ Contracts verified with source code
- ✅ UUPS proxy pattern implemented correctly
- ✅ Only admin can upgrade contracts
- ✅ Timelock on ownership transfer

**Status**: ⏳ Pending

---

#### SEC-020: Code Signing and Integrity
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
- [ ] Verify frontend build integrity (SRI)
- [ ] Check NPM package integrity
- [ ] Verify Docker image signatures
- [ ] Check CI/CD pipeline security

**Expected Result**:
- ✅ Subresource Integrity (SRI) for CDN assets
- ✅ Package-lock.json committed
- ✅ Signed container images
- ✅ Secrets in CI/CD encrypted

**Status**: ⏳ Pending

---

### 9. A09:2021 - Security Logging and Monitoring Failures

#### SEC-021: Logging and Monitoring
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
- [ ] Verify failed login attempts logged
- [ ] Check transaction events logged
- [ ] Verify security events logged (access denied)
- [ ] Test log tampering protection
- [ ] Check alerting for anomalies

**Expected Result**:
- ✅ All authentication events logged
- ✅ All financial transactions logged
- ✅ Logs immutable/append-only
- ✅ Alerts for 10+ failed logins
- ✅ Alerts for large transfers (>$10k)

**Status**: ⏳ Pending

---

### 10. A10:2021 - Server-Side Request Forgery (SSRF)

#### SEC-022: SSRF Protection
**Priority**: HIGH
**Risk**: Medium

**Test Cases**:
- [ ] Test webhook URL validation
- [ ] Try internal IP addresses: `http://127.0.0.1:5000`
- [ ] Try AWS metadata: `http://169.254.169.254/latest/meta-data/`
- [ ] Test DNS rebinding attacks

**Expected Result**:
- ✅ Webhook URLs validated (allowlist)
- ✅ Internal IPs blocked
- ✅ Cloud metadata endpoints blocked
- ✅ URL scheme restricted (https only)

**Status**: ⏳ Pending

---

## Blockchain-Specific Security Testing

### SEC-023: Smart Contract Reentrancy
**Priority**: CRITICAL
**Risk**: HIGH

**Test Cases**:
- [ ] Test reentrancy attack on transfer function
- [ ] Check ReentrancyGuard modifier usage
- [ ] Verify Checks-Effects-Interactions pattern

**Expected Result**:
- ✅ ReentrancyGuard on all payable functions
- ✅ State updated before external calls
- ✅ No reentrancy vulnerabilities found

**Status**: ⏳ Pending

---

### SEC-024: Smart Contract Access Control
**Priority**: CRITICAL
**Risk**: HIGH

**Test Cases**:
- [ ] Try calling admin functions as non-admin
- [ ] Test ownership transfer
- [ ] Check role-based access control (RBAC)

**Expected Result**:
- ✅ OnlyOwner modifier on sensitive functions
- ✅ RBAC implemented correctly
- ✅ Ownership transfer requires 2-step process

**Status**: ⏳ Pending

---

### SEC-025: Integer Overflow/Underflow
**Priority**: HIGH
**Risk**: MEDIUM

**Test Cases**:
- [ ] Try arithmetic overflow in transfer amounts
- [ ] Test underflow in balance subtraction
- [ ] Verify SafeMath usage (Solidity 0.8+ has built-in)

**Expected Result**:
- ✅ Solidity 0.8+ (automatic overflow checks)
- ✅ SafeMath library used if <0.8
- ✅ No overflow/underflow possible

**Status**: ⏳ Pending

---

### SEC-026: Front-Running Protection
**Priority**: HIGH
**Risk**: MEDIUM

**Test Cases**:
- [ ] Test transaction ordering manipulation
- [ ] Verify MEV (Miner Extractable Value) protection
- [ ] Check for commit-reveal schemes where needed

**Expected Result**:
- ✅ Gasless transactions via paymaster (less front-running risk)
- ✅ Time-based or nonce-based replay protection
- ✅ No obvious front-running vectors

**Status**: ⏳ Pending

---

### SEC-027: Oracle Manipulation
**Priority**: HIGH
**Risk**: MEDIUM

**Test Cases**:
- [ ] Verify Chainlink price feed usage
- [ ] Test stale price data handling
- [ ] Check for oracle fallback mechanisms

**Expected Result**:
- ✅ Chainlink decentralized oracles used
- ✅ Stale data rejected (check timestamp)
- ✅ Multiple oracle sources for critical data
- ✅ Circuit breaker for price deviations

**Status**: ⏳ Pending

---

## API Security Testing

### SEC-028: API Authentication
**Priority**: CRITICAL
**Risk**: HIGH

**Test Cases**:
- [ ] Try accessing protected endpoints without token
- [ ] Test with expired token
- [ ] Test with manipulated token (change user ID in JWT)
- [ ] Verify token signature validation

**Expected Result**:
- ✅ 401 Unauthorized without token
- ✅ 401 with expired token
- ✅ Token manipulation detected
- ✅ JWT signature verified

**Status**: ⏳ Pending

---

### SEC-029: API Rate Limiting
**Priority**: HIGH
**Risk**: MEDIUM

**Test Cases**:
- [ ] Send 100 requests/second to API
- [ ] Verify rate limit headers present
- [ ] Test different endpoints

**Expected Result**:
- ✅ Rate limit: 100 requests/minute per IP
- ✅ Headers: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`
- ✅ 429 status when exceeded

**Status**: ⏳ Pending

---

### SEC-030: API Input Validation
**Priority**: CRITICAL
**Risk**: HIGH

**Test Cases**:
- [ ] Send invalid JSON to API
- [ ] Send extra fields in request
- [ ] Send wrong data types (string instead of number)
- [ ] Test boundary values (max int, max string length)

**Expected Result**:
- ✅ 400 Bad Request for invalid JSON
- ✅ Extra fields ignored or rejected
- ✅ Type validation enforced
- ✅ Boundary validation present

**Status**: ⏳ Pending

---

## Data Protection & Privacy

### SEC-031: GDPR Compliance
**Priority**: HIGH
**Risk**: MEDIUM

**Test Cases**:
- [ ] Verify data minimization (only collect necessary data)
- [ ] Test right to access (user can download their data)
- [ ] Test right to erasure (user can delete account)
- [ ] Check privacy policy and consent

**Expected Result**:
- ✅ Minimal data collection
- ✅ Data export feature available
- ✅ Account deletion removes all PII
- ✅ Privacy policy accessible
- ✅ Consent captured on registration

**Status**: ⏳ Pending

---

### SEC-032: PII Protection
**Priority**: CRITICAL
**Risk**: HIGH

**Test Cases**:
- [ ] Verify email addresses not in URLs or logs
- [ ] Check user names not in blockchain transactions
- [ ] Verify transaction notes encrypted if containing PII

**Expected Result**:
- ✅ No PII in URLs or logs
- ✅ Blockchain transactions pseudonymous (only addresses)
- ✅ PII encrypted in database

**Status**: ⏳ Pending

---

## Security Testing Tools

### Automated Scanning Tools

#### OWASP ZAP
```bash
docker run -v $(pwd):/zap/wrk/:rw -t owasp/zap2docker-stable zap-baseline.py \
  -t http://localhost:3000 -g gen.conf -r zap-report.html
```

#### npm audit (Frontend)
```bash
cd CoinPay.Web
npm audit --audit-level=moderate
```

#### dotnet security scan (Backend)
```bash
cd CoinPay.Api
dotnet list package --vulnerable --include-transitive
```

#### Slither (Smart Contracts)
```bash
slither contracts/
```

#### MythX (Smart Contracts)
```bash
mythx analyze contracts/WalletFactory.sol
```

---

## Test Execution Checklist

### Phase 1: Automated Scanning
- [ ] Run npm audit on frontend
- [ ] Run dotnet security scan on backend
- [ ] Run OWASP ZAP baseline scan
- [ ] Run Slither on smart contracts
- [ ] Review all findings

### Phase 2: Manual Testing
- [ ] Test all OWASP Top 10 vulnerabilities
- [ ] Test authentication and authorization
- [ ] Test business logic vulnerabilities
- [ ] Test API security
- [ ] Test blockchain-specific vulnerabilities

### Phase 3: Remediation
- [ ] Document all vulnerabilities found
- [ ] Assign severity ratings (Critical, High, Medium, Low)
- [ ] Create remediation tickets
- [ ] Verify fixes

### Phase 4: Final Scan
- [ ] Re-run all automated scans
- [ ] Verify all critical/high vulnerabilities resolved
- [ ] Generate security test report

---

## Severity Rating Guide

| Severity | Description | Example |
|----------|-------------|---------|
| **Critical** | Immediate exploitation possible, high impact | SQL injection, RCE, authentication bypass |
| **High** | Exploitation likely, significant impact | XSS, IDOR, privilege escalation |
| **Medium** | Exploitation requires specific conditions | CSRF, information disclosure |
| **Low** | Minimal impact or difficult to exploit | Missing security headers, verbose errors |

---

## Test Report Template

### Security Test Report

**Date**: _______________
**Tester**: _______________
**Environment**: _______________

#### Executive Summary
- Total vulnerabilities found: ___
- Critical: ___
- High: ___
- Medium: ___
- Low: ___

#### Critical Findings
1. **[Vulnerability Name]**
   - Severity: Critical
   - CVSS Score: 9.8
   - Description: ...
   - Impact: ...
   - Remediation: ...
   - Status: Open/Fixed

#### High Findings
1. ...

#### Recommendations
1. Implement Web Application Firewall (WAF)
2. Enable automated dependency scanning
3. Conduct annual penetration testing
4. Security training for development team

**Signed**: _______________ Date: _______________

---

## Notes

- Security testing is ongoing, not one-time
- Run automated scans weekly
- Manual testing quarterly
- Re-test after major releases
- Coordinate with penetration testing team
- Report all findings to security team immediately

