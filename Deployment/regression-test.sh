#!/bin/bash
# regression-test.sh - Post-Deployment Regression Test Suite

echo "=============================================="
echo "  CoinPay Regression Test Suite"
echo "=============================================="
echo ""
echo "Date: $(date)"
echo "Environment: ${ENVIRONMENT:-development}"
echo ""

TESTS_PASSED=0
TESTS_FAILED=0
TESTS_SKIPPED=0

# Color codes
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

function test_result() {
    local test_name="$1"
    local result="$2"
    local message="$3"

    if [ "${result}" == "PASS" ]; then
        echo -e "  ${GREEN}✅ PASS${NC}: ${test_name}"
        ((TESTS_PASSED++))
    elif [ "${result}" == "FAIL" ]; then
        echo -e "  ${RED}❌ FAIL${NC}: ${test_name}"
        [ ! -z "${message}" ] && echo "      Error: ${message}"
        ((TESTS_FAILED++))
    else
        echo -e "  ${YELLOW}⊘ SKIP${NC}: ${test_name} - ${message}"
        ((TESTS_SKIPPED++))
    fi
}

echo "=============================================="
echo "Phase 1: Infrastructure Tests"
echo "=============================================="
echo ""

# Test 1: Docker Health
echo "Test 1.1: Docker Service"
if docker info > /dev/null 2>&1; then
    test_result "Docker Service" "PASS"
else
    test_result "Docker Service" "FAIL" "Docker daemon not running"
fi

# Test 2: Container Status
echo "Test 1.2: Container Status"
EXPECTED_CONTAINERS=6
RUNNING_CONTAINERS=$(docker ps --filter "name=coinpay" --format "{{.Names}}" | wc -l)

if [ ${RUNNING_CONTAINERS} -eq ${EXPECTED_CONTAINERS} ]; then
    test_result "All Containers Running" "PASS"
else
    test_result "All Containers Running" "FAIL" "Expected ${EXPECTED_CONTAINERS}, found ${RUNNING_CONTAINERS}"
    echo "      Running containers:"
    docker ps --filter "name=coinpay" --format "      - {{.Names}} ({{.Status}})"
fi

# Test 3: Database Connectivity
echo "Test 1.3: Database Connectivity"
if docker exec coinpay-postgres-compose pg_isready -U postgres 2>&1 | grep -q "accepting connections"; then
    test_result "PostgreSQL Connection" "PASS"
else
    test_result "PostgreSQL Connection" "FAIL" "Database not accepting connections"
fi

# Test 4: Vault Status
echo "Test 1.4: Vault Status"
if docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault status 2>&1 | grep -q "Initialized.*true"; then
    test_result "Vault Initialization" "PASS"
else
    test_result "Vault Initialization" "FAIL" "Vault not initialized"
fi

echo ""
echo "=============================================="
echo "Phase 2: API Health Tests"
echo "=============================================="
echo ""

# Test 5: API Health Endpoint
echo "Test 2.1: API Health Endpoint"
HEALTH=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:7777/health)
if [ "${HEALTH}" == "200" ]; then
    test_result "API Health Endpoint" "PASS"
else
    test_result "API Health Endpoint" "FAIL" "HTTP ${HEALTH}"
fi

# Test 6: Swagger Documentation
echo "Test 2.2: Swagger Documentation"
SWAGGER=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:7777/swagger/index.html)
if [ "${SWAGGER}" == "200" ]; then
    test_result "Swagger UI" "PASS"
else
    test_result "Swagger UI" "FAIL" "HTTP ${SWAGGER}"
fi

echo ""
echo "=============================================="
echo "Phase 3: Authentication Tests"
echo "=============================================="
echo ""

# Test 7: Username Check Endpoint
echo "Test 3.1: Username Check Endpoint"
RESPONSE=$(curl -s -X POST http://localhost:7777/api/auth/check-username \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser"}')

if echo ${RESPONSE} | grep -q "exists"; then
    test_result "Username Check Endpoint" "PASS"
else
    test_result "Username Check Endpoint" "FAIL" "Invalid response"
fi

echo ""
echo "=============================================="
echo "Phase 4: Database Integrity Tests"
echo "=============================================="
echo ""

# Test 8: Database Tables
echo "Test 4.1: Database Tables"
TABLE_COUNT=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c \
  "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';")

EXPECTED_TABLES=14
if [ ${TABLE_COUNT} -eq ${EXPECTED_TABLES} ]; then
    test_result "Database Tables Count" "PASS"
else
    test_result "Database Tables Count" "FAIL" "Expected ${EXPECTED_TABLES}, found ${TABLE_COUNT}"
fi

# Test 9: Users Table
echo "Test 4.2: Users Table"
USER_COUNT=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c "SELECT COUNT(*) FROM \"Users\";" 2>/dev/null | tr -d ' ')

if [ ! -z "${USER_COUNT}" ] && [ ${USER_COUNT} -ge 0 ]; then
    test_result "Users Table Access" "PASS"
    echo "      User count: ${USER_COUNT}"
else
    test_result "Users Table Access" "FAIL"
fi

# Test 10: Transactions Table
echo "Test 4.3: Transactions Table"
if docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "SELECT COUNT(*) FROM \"Transactions\";" > /dev/null 2>&1; then
    test_result "Transactions Table Access" "PASS"
else
    test_result "Transactions Table Access" "FAIL"
fi

# Test 11: SwapTransactions Table (Phase 5)
echo "Test 4.4: SwapTransactions Table"
if docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "SELECT COUNT(*) FROM \"SwapTransactions\";" > /dev/null 2>&1; then
    test_result "SwapTransactions Table Access" "PASS"
else
    test_result "SwapTransactions Table Access" "FAIL"
fi

echo ""
echo "=============================================="
echo "Phase 5: Vault Secrets Tests"
echo "=============================================="
echo ""

# Test 12: Vault Secrets Accessibility
echo "Test 5.1: Vault Secrets"
SECRETS=("database" "jwt" "encryption" "gateway" "blockchain" "circle" "whitebit")
SECRETS_OK=0

for secret in "${SECRETS[@]}"; do
    if docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
      vault kv get coinpay/${secret} > /dev/null 2>&1; then
        ((SECRETS_OK++))
    fi
done

if [ ${SECRETS_OK} -eq ${#SECRETS[@]} ]; then
    test_result "Vault Secrets Accessible" "PASS"
    echo "      Secrets found: ${SECRETS_OK}/${#SECRETS[@]}"
else
    test_result "Vault Secrets Accessible" "FAIL" "Found ${SECRETS_OK}/${#SECRETS[@]} secrets"
fi

echo ""
echo "=============================================="
echo "Phase 6: API Endpoint Tests"
echo "=============================================="
echo ""

# Test 13: Swap Quote Endpoint
echo "Test 6.1: Swap Quote Endpoint"
QUOTE=$(curl -s "http://localhost:7777/api/swap/quote?fromToken=0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582&toToken=0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9&amount=100&slippage=1")

if echo ${QUOTE} | grep -q "fromTokenSymbol"; then
    test_result "Swap Quote Endpoint" "PASS"
    RATE=$(echo ${QUOTE} | jq -r '.exchangeRate' 2>/dev/null || echo "N/A")
    echo "      Exchange rate: ${RATE}"
else
    test_result "Swap Quote Endpoint" "FAIL" "Invalid response"
fi

echo ""
echo "=============================================="
echo "Phase 7: Frontend Tests"
echo "=============================================="
echo ""

# Test 14: Frontend Accessibility
echo "Test 7.1: Frontend Web Application"
FRONTEND=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:3000)
if [ "${FRONTEND}" == "200" ]; then
    test_result "Frontend Accessibility" "PASS"
else
    test_result "Frontend Accessibility" "FAIL" "HTTP ${FRONTEND}"
fi

# Test 15: Documentation Site
echo "Test 7.2: Documentation Site"
DOCS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8080)
if [ "${DOCS}" == "200" ]; then
    test_result "Documentation Site" "PASS"
else
    test_result "Documentation Site" "FAIL" "HTTP ${DOCS}"
fi

echo ""
echo "=============================================="
echo "Phase 8: Performance Tests"
echo "=============================================="
echo ""

# Test 16: API Response Time
echo "Test 8.1: API Response Time"
RESPONSE_TIME=$(curl -s -o /dev/null -w "%{time_total}" http://localhost:7777/health)
RESPONSE_TIME_MS=$(echo "${RESPONSE_TIME} * 1000" | bc | cut -d'.' -f1)

if [ ${RESPONSE_TIME_MS} -lt 1000 ]; then
    test_result "API Response Time" "PASS"
    echo "      Response time: ${RESPONSE_TIME_MS}ms"
else
    test_result "API Response Time" "FAIL" "Response time: ${RESPONSE_TIME_MS}ms (threshold: 1000ms)"
fi

# Test 17: Database Connection Pool
echo "Test 8.2: Database Connection Pool"
DB_CONNECTIONS=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c \
  "SELECT count(*) FROM pg_stat_activity WHERE datname='coinpay';" 2>/dev/null | tr -d ' ')

if [ ! -z "${DB_CONNECTIONS}" ] && [ ${DB_CONNECTIONS} -lt 50 ]; then
    test_result "Database Connection Pool" "PASS"
    echo "      Active connections: ${DB_CONNECTIONS}"
else
    test_result "Database Connection Pool" "FAIL" "Connections: ${DB_CONNECTIONS} (threshold: 50)"
fi

echo ""
echo "=============================================="
echo "Phase 9: Security Tests"
echo "=============================================="
echo ""

# Test 18: Protected Endpoints (should require auth)
echo "Test 9.1: Protected Endpoints"
PROTECTED=$(curl -s -o /dev/null -w "%{http_code}" -X POST http://localhost:7777/api/swap/execute \
  -H "Content-Type: application/json" \
  -d '{"fromToken":"0x123","toToken":"0x456","fromAmount":100,"slippageTolerance":1}')

if [ "${PROTECTED}" == "401" ] || [ "${PROTECTED}" == "403" ]; then
    test_result "Protected Endpoints" "PASS"
    echo "      Unauthorized access properly rejected"
else
    test_result "Protected Endpoints" "FAIL" "HTTP ${PROTECTED} (expected 401 or 403)"
fi

echo ""
echo "=============================================="
echo "  Test Summary"
echo "=============================================="
echo ""
echo "Total Tests:   $((TESTS_PASSED + TESTS_FAILED + TESTS_SKIPPED))"
echo -e "${GREEN}Tests Passed:  ${TESTS_PASSED}${NC}"
echo -e "${RED}Tests Failed:  ${TESTS_FAILED}${NC}"
echo -e "${YELLOW}Tests Skipped: ${TESTS_SKIPPED}${NC}"
echo ""

# Calculate success rate
if [ $((TESTS_PASSED + TESTS_FAILED)) -gt 0 ]; then
    SUCCESS_RATE=$((TESTS_PASSED * 100 / (TESTS_PASSED + TESTS_FAILED)))
    echo "Success Rate:  ${SUCCESS_RATE}%"
    echo ""
fi

# Exit code
if [ ${TESTS_FAILED} -gt 0 ]; then
    echo -e "${RED}❌ REGRESSION TESTS FAILED${NC}"
    echo ""
    echo "Action Required:"
    echo "  1. Review failed tests above"
    echo "  2. Check container logs: docker-compose logs"
    echo "  3. Consider rollback if critical failures"
    echo ""
    exit 1
else
    echo -e "${GREEN}✅ ALL REGRESSION TESTS PASSED${NC}"
    echo ""
    echo "Deployment validation successful!"
    echo "System is ready for production traffic."
    echo ""
    exit 0
fi
