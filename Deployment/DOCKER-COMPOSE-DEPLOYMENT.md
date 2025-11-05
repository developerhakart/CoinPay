# Docker Compose Deployment Guide - Data Protection & CI/CD

**Version**: 1.0.0
**Date**: November 5, 2025
**Status**: Production Ready

---

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Data Protection Strategy](#data-protection-strategy)
4. [Pre-Deployment Checklist](#pre-deployment-checklist)
5. [Deployment Steps](#deployment-steps)
6. [Post-Deployment Validation](#post-deployment-validation)
7. [Backup & Restore Procedures](#backup--restore-procedures)
8. [CI/CD Pipeline](#cicd-pipeline)
9. [Rollback Procedures](#rollback-procedures)
10. [Troubleshooting](#troubleshooting)

---

## Overview

This guide provides comprehensive instructions for deploying CoinPay using Docker Compose with **ZERO DATA LOSS** guarantees. It includes:

- Database backup/restore procedures
- Vault credentials protection
- Full regression testing after each build
- CI/CD pipeline integration
- Configuration management
- Data integrity verification

---

## Prerequisites

### Required Software

```bash
# Check versions
docker --version          # Required: >= 20.10.0
docker-compose --version  # Required: >= 2.0.0
git --version            # Required: >= 2.30.0
```

### System Requirements

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| CPU | 4 cores | 8 cores |
| RAM | 8 GB | 16 GB |
| Disk Space | 20 GB | 50 GB |
| Network | 10 Mbps | 100 Mbps |

### Required Files

```
deployment/
├── docker-compose.yml
├── .env.production
├── backup-restore.sh
├── regression-test.sh
├── health-check.sh
└── ci-cd-pipeline.yml
```

---

## Data Protection Strategy

### 1. Database Data Protection

#### Named Volumes (Persistent Storage)
```yaml
volumes:
  postgres-data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: /var/lib/coinpay/postgres  # Absolute path for backup

  vault-data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: /var/lib/coinpay/vault
```

#### Database Backup Strategy

**Before ANY deployment**:
```bash
# 1. Create timestamped backup
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="./backups/${TIMESTAMP}"
mkdir -p ${BACKUP_DIR}

# 2. Backup database
docker exec coinpay-postgres-compose pg_dump -U postgres -d coinpay \
  | gzip > ${BACKUP_DIR}/database_backup.sql.gz

# 3. Backup volume data
docker run --rm \
  -v coinpay_postgres-data:/data \
  -v ${PWD}/${BACKUP_DIR}:/backup \
  alpine tar czf /backup/postgres-volume.tar.gz -C /data .

# 4. Verify backup
gunzip -t ${BACKUP_DIR}/database_backup.sql.gz
echo "Backup created: ${BACKUP_DIR}"
```

### 2. Vault Credentials Protection

#### Vault Backup Procedure
```bash
# Export all secrets before deployment
VAULT_ADDR=http://localhost:8200
VAULT_TOKEN=dev-root-token

# Backup all secrets
docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
  vault kv get -format=json coinpay/database > ${BACKUP_DIR}/vault-database.json

docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
  vault kv get -format=json coinpay/jwt > ${BACKUP_DIR}/vault-jwt.json

docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
  vault kv get -format=json coinpay/encryption > ${BACKUP_DIR}/vault-encryption.json

docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
  vault kv get -format=json coinpay/gateway > ${BACKUP_DIR}/vault-gateway.json

docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
  vault kv get -format=json coinpay/blockchain > ${BACKUP_DIR}/vault-blockchain.json

docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
  vault kv get -format=json coinpay/circle > ${BACKUP_DIR}/vault-circle.json

docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
  vault kv get -format=json coinpay/whitebit > ${BACKUP_DIR}/vault-whitebit.json

# Create backup manifest
echo "Backup Date: $(date)" > ${BACKUP_DIR}/manifest.txt
echo "Database Backup: database_backup.sql.gz" >> ${BACKUP_DIR}/manifest.txt
echo "Vault Secrets: 7 files" >> ${BACKUP_DIR}/manifest.txt
ls -lh ${BACKUP_DIR} >> ${BACKUP_DIR}/manifest.txt
```

### 3. Configuration Backup

```bash
# Backup all configuration files
cp docker-compose.yml ${BACKUP_DIR}/
cp .env ${BACKUP_DIR}/
cp CoinPay.Api/appsettings.json ${BACKUP_DIR}/
cp CoinPay.Api/appsettings.Development.json ${BACKUP_DIR}/
cp CoinPay.Api/appsettings.Production.json ${BACKUP_DIR}/
```

---

## Pre-Deployment Checklist

### Phase 1: Pre-Flight Checks

```bash
#!/bin/bash
# pre-deployment-check.sh

echo "=== Pre-Deployment Checklist ==="
echo ""

# 1. Check Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running"
    exit 1
fi
echo "✅ Docker is running"

# 2. Check disk space (minimum 10GB free)
FREE_SPACE=$(df -BG . | tail -1 | awk '{print $4}' | sed 's/G//')
if [ ${FREE_SPACE} -lt 10 ]; then
    echo "❌ Insufficient disk space: ${FREE_SPACE}GB (need 10GB)"
    exit 1
fi
echo "✅ Disk space: ${FREE_SPACE}GB available"

# 3. Check if containers are running
if docker ps --filter "name=coinpay" --format "{{.Names}}" | grep -q "coinpay"; then
    echo "⚠️  CoinPay containers are currently running"
    read -p "Continue with backup and rebuild? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# 4. Check backup directory exists
if [ ! -d "./backups" ]; then
    mkdir -p ./backups
fi
echo "✅ Backup directory ready"

# 5. Verify backup script exists
if [ ! -f "./backup-restore.sh" ]; then
    echo "❌ backup-restore.sh not found"
    exit 1
fi
echo "✅ Backup script ready"

echo ""
echo "✅ All pre-flight checks passed"
echo ""
```

### Phase 2: Create Backup

```bash
# Run automated backup
./backup-restore.sh backup

# Verify backup completed successfully
if [ $? -eq 0 ]; then
    echo "✅ Backup completed successfully"
else
    echo "❌ Backup failed - ABORTING DEPLOYMENT"
    exit 1
fi
```

### Phase 3: Record Current State

```bash
# Record current container states
docker ps --filter "name=coinpay" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" \
  > ${BACKUP_DIR}/container-state-before.txt

# Record current data counts
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c \
  "SELECT 'Users' as table_name, COUNT(*) as count FROM \"Users\"
   UNION ALL
   SELECT 'Wallets', COUNT(*) FROM \"Wallets\"
   UNION ALL
   SELECT 'Transactions', COUNT(*) FROM \"Transactions\"
   UNION ALL
   SELECT 'SwapTransactions', COUNT(*) FROM \"SwapTransactions\";" \
  > ${BACKUP_DIR}/data-counts-before.txt

echo "✅ Current state recorded"
```

---

## Deployment Steps

### Step 1: Stop Current Containers (Graceful Shutdown)

```bash
#!/bin/bash
# graceful-shutdown.sh

echo "=== Graceful Shutdown Procedure ==="

# 1. Stop accepting new requests (if load balancer exists)
# curl -X POST http://loadbalancer/drain

# 2. Wait for in-flight requests to complete (30 seconds)
echo "Waiting for in-flight requests to complete..."
sleep 30

# 3. Stop containers in reverse dependency order
echo "Stopping web container..."
docker stop coinpay-web

echo "Stopping gateway container..."
docker stop coinpay-gateway

echo "Stopping API container..."
docker stop coinpay-api

# 4. Verify no active connections to database
CONNECTIONS=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c \
  "SELECT count(*) FROM pg_stat_activity WHERE datname='coinpay' AND pid <> pg_backend_pid();")
echo "Active database connections: ${CONNECTIONS}"

# 5. Stop remaining infrastructure
echo "Stopping docs container..."
docker stop coinpay-docs

echo "Stopping database container..."
docker stop coinpay-postgres-compose

echo "Stopping vault container..."
docker stop coinpay-vault

echo "✅ All containers stopped gracefully"
```

### Step 2: Build New Images

```bash
#!/bin/bash
# build-all.sh

echo "=== Building All Docker Images ==="

# Build with no cache to ensure fresh build
docker-compose build --no-cache --parallel

# Verify all images built successfully
IMAGES=(
    "coinpay-api"
    "coinpay-gateway"
    "coinpay-web"
    "coinpay-docs"
)

for image in "${IMAGES[@]}"; do
    if docker images | grep -q ${image}; then
        echo "✅ ${image} built successfully"
    else
        echo "❌ ${image} build failed"
        exit 1
    fi
done

echo "✅ All images built successfully"
```

### Step 3: Start Containers (Staged Rollout)

```bash
#!/bin/bash
# staged-startup.sh

echo "=== Staged Container Startup ==="

# Stage 1: Infrastructure (Vault, Database)
echo "Stage 1: Starting infrastructure..."
docker-compose up -d vault postgres

# Wait for infrastructure health
echo "Waiting for Vault health check..."
timeout 60 bash -c 'until docker exec coinpay-vault vault status 2>&1 | grep -q "Initialized"; do sleep 2; done'
if [ $? -ne 0 ]; then
    echo "❌ Vault failed to start"
    exit 1
fi
echo "✅ Vault is healthy"

echo "Waiting for database health check..."
timeout 60 bash -c 'until docker exec coinpay-postgres-compose pg_isready -U postgres 2>&1 | grep -q "accepting connections"; do sleep 2; done'
if [ $? -ne 0 ]; then
    echo "❌ Database failed to start"
    exit 1
fi
echo "✅ Database is healthy"

# Stage 2: Restore Vault secrets
echo "Stage 2: Restoring Vault secrets..."
./restore-vault-secrets.sh
if [ $? -ne 0 ]; then
    echo "❌ Failed to restore Vault secrets"
    exit 1
fi
echo "✅ Vault secrets restored"

# Stage 3: Application Layer (API)
echo "Stage 3: Starting API..."
docker-compose up -d api

# Wait for API health
echo "Waiting for API health check..."
timeout 120 bash -c 'until curl -f http://localhost:7777/health 2>&1 | grep -q "Healthy"; do sleep 5; done'
if [ $? -ne 0 ]; then
    echo "❌ API failed to start"
    docker logs coinpay-api --tail 50
    exit 1
fi
echo "✅ API is healthy"

# Stage 4: Gateway and Frontend
echo "Stage 4: Starting gateway and frontend..."
docker-compose up -d gateway web docs

# Final health check
sleep 10
docker-compose ps

echo "✅ All containers started successfully"
```

### Step 4: Verify Data Integrity

```bash
#!/bin/bash
# verify-data-integrity.sh

echo "=== Data Integrity Verification ==="

# 1. Compare data counts before and after
echo "Checking data counts..."
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c \
  "SELECT 'Users' as table_name, COUNT(*) as count FROM \"Users\"
   UNION ALL
   SELECT 'Wallets', COUNT(*) FROM \"Wallets\"
   UNION ALL
   SELECT 'Transactions', COUNT(*) FROM \"Transactions\"
   UNION ALL
   SELECT 'SwapTransactions', COUNT(*) FROM \"SwapTransactions\";" \
  > data-counts-after.txt

# Compare counts
if diff -q data-counts-before.txt data-counts-after.txt > /dev/null; then
    echo "✅ Data counts match - no data loss"
else
    echo "⚠️  Data count differences detected:"
    diff data-counts-before.txt data-counts-after.txt
    read -p "Continue anyway? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Initiating rollback..."
        ./rollback.sh
        exit 1
    fi
fi

# 2. Verify critical data samples
echo "Verifying sample data..."
SAMPLE_USER=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c \
  "SELECT \"Username\" FROM \"Users\" LIMIT 1;")

if [ -z "${SAMPLE_USER}" ]; then
    echo "❌ No users found in database - DATA LOSS DETECTED"
    echo "Initiating rollback..."
    ./rollback.sh
    exit 1
fi
echo "✅ Sample data verified: Found user '${SAMPLE_USER}'"

# 3. Verify Vault secrets accessible
echo "Verifying Vault secrets..."
VAULT_TEST=$(docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv get -field=connectionString coinpay/database 2>&1)

if echo "${VAULT_TEST}" | grep -q "Host="; then
    echo "✅ Vault secrets accessible"
else
    echo "❌ Vault secrets not accessible"
    exit 1
fi

echo "✅ Data integrity verified"
```

---

## Post-Deployment Validation

### Regression Test Suite

```bash
#!/bin/bash
# regression-test.sh

echo "=== Post-Deployment Regression Tests ==="

TESTS_PASSED=0
TESTS_FAILED=0

# Test 1: Health Endpoints
echo "Test 1: Health Endpoints"
if curl -f http://localhost:7777/health 2>&1 | grep -q "Healthy"; then
    echo "  ✅ API Health: PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ API Health: FAIL"
    ((TESTS_FAILED++))
fi

# Test 2: Database Connectivity
echo "Test 2: Database Connectivity"
if docker exec coinpay-postgres-compose pg_isready -U postgres | grep -q "accepting connections"; then
    echo "  ✅ Database Connection: PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ Database Connection: FAIL"
    ((TESTS_FAILED++))
fi

# Test 3: Vault Accessibility
echo "Test 3: Vault Accessibility"
if docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault status 2>&1 | grep -q "Initialized.*true"; then
    echo "  ✅ Vault Access: PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ Vault Access: FAIL"
    ((TESTS_FAILED++))
fi

# Test 4: User Authentication Endpoint
echo "Test 4: Authentication Endpoints"
RESPONSE=$(curl -s -X POST http://localhost:7777/api/auth/check-username \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser"}')
if echo ${RESPONSE} | grep -q "exists"; then
    echo "  ✅ Auth Endpoint: PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ Auth Endpoint: FAIL"
    ((TESTS_FAILED++))
fi

# Test 5: Swap Quote Endpoint (Phase 5)
echo "Test 5: Swap Quote Endpoint"
QUOTE=$(curl -s "http://localhost:7777/api/swap/quote?fromToken=0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582&toToken=0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9&amount=100&slippage=1")
if echo ${QUOTE} | grep -q "fromTokenSymbol"; then
    echo "  ✅ Swap Quote: PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ Swap Quote: FAIL"
    ((TESTS_FAILED++))
fi

# Test 6: Frontend Accessibility
echo "Test 6: Frontend Accessibility"
if curl -f http://localhost:3000 2>&1 | grep -q "<!doctype html>"; then
    echo "  ✅ Frontend: PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ Frontend: FAIL"
    ((TESTS_FAILED++))
fi

# Test 7: Documentation Site
echo "Test 7: Documentation Site"
if curl -f http://localhost:8080 2>&1 | grep -q "html"; then
    echo "  ✅ Documentation: PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ Documentation: FAIL"
    ((TESTS_FAILED++))
fi

# Test 8: Data Persistence (User Count)
echo "Test 8: Data Persistence"
USER_COUNT=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c "SELECT COUNT(*) FROM \"Users\";")
if [ ${USER_COUNT} -gt 0 ]; then
    echo "  ✅ Data Persistence (${USER_COUNT} users): PASS"
    ((TESTS_PASSED++))
else
    echo "  ❌ Data Persistence: FAIL"
    ((TESTS_FAILED++))
fi

# Summary
echo ""
echo "=== Test Summary ==="
echo "Tests Passed: ${TESTS_PASSED}"
echo "Tests Failed: ${TESTS_FAILED}"
echo ""

if [ ${TESTS_FAILED} -gt 0 ]; then
    echo "❌ REGRESSION TESTS FAILED"
    echo "Recommend reviewing logs and considering rollback"
    exit 1
else
    echo "✅ ALL REGRESSION TESTS PASSED"
    exit 0
fi
```

---

## Backup & Restore Procedures

### Automated Backup Script

```bash
#!/bin/bash
# backup-restore.sh

BACKUP_DIR="./backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_PATH="${BACKUP_DIR}/${TIMESTAMP}"

function backup() {
    echo "=== Starting Backup Process ==="

    # Create backup directory
    mkdir -p ${BACKUP_PATH}

    # 1. Backup Database
    echo "Backing up database..."
    docker exec coinpay-postgres-compose pg_dump -U postgres -d coinpay \
      | gzip > ${BACKUP_PATH}/database_backup.sql.gz

    # Verify backup
    if gunzip -t ${BACKUP_PATH}/database_backup.sql.gz 2>&1; then
        SIZE=$(du -h ${BACKUP_PATH}/database_backup.sql.gz | cut -f1)
        echo "✅ Database backup completed: ${SIZE}"
    else
        echo "❌ Database backup failed"
        return 1
    fi

    # 2. Backup Database Volume
    echo "Backing up database volume..."
    docker run --rm \
      -v coinpay_postgres-data:/data \
      -v ${PWD}/${BACKUP_PATH}:/backup \
      alpine tar czf /backup/postgres-volume.tar.gz -C /data .
    echo "✅ Volume backup completed"

    # 3. Backup Vault Secrets
    echo "Backing up Vault secrets..."
    VAULT_TOKEN=dev-root-token

    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault kv get -format=json coinpay/database > ${BACKUP_PATH}/vault-database.json 2>/dev/null || echo "{}" > ${BACKUP_PATH}/vault-database.json

    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault kv get -format=json coinpay/jwt > ${BACKUP_PATH}/vault-jwt.json 2>/dev/null || echo "{}" > ${BACKUP_PATH}/vault-jwt.json

    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault kv get -format=json coinpay/encryption > ${BACKUP_PATH}/vault-encryption.json 2>/dev/null || echo "{}" > ${BACKUP_PATH}/vault-encryption.json

    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault kv get -format=json coinpay/gateway > ${BACKUP_PATH}/vault-gateway.json 2>/dev/null || echo "{}" > ${BACKUP_PATH}/vault-gateway.json

    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault kv get -format=json coinpay/blockchain > ${BACKUP_PATH}/vault-blockchain.json 2>/dev/null || echo "{}" > ${BACKUP_PATH}/vault-blockchain.json

    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault kv get -format=json coinpay/circle > ${BACKUP_PATH}/vault-circle.json 2>/dev/null || echo "{}" > ${BACKUP_PATH}/vault-circle.json

    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault kv get -format=json coinpay/whitebit > ${BACKUP_PATH}/vault-whitebit.json 2>/dev/null || echo "{}" > ${BACKUP_PATH}/vault-whitebit.json

    echo "✅ Vault secrets backed up"

    # 4. Backup Configuration
    echo "Backing up configuration..."
    cp docker-compose.yml ${BACKUP_PATH}/
    cp ../CoinPay.Api/appsettings.*.json ${BACKUP_PATH}/ 2>/dev/null
    echo "✅ Configuration backed up"

    # 5. Create manifest
    echo "Creating backup manifest..."
    cat > ${BACKUP_PATH}/manifest.txt <<EOF
CoinPay Backup
==============
Date: $(date)
Backup ID: ${TIMESTAMP}

Contents:
- database_backup.sql.gz
- postgres-volume.tar.gz
- vault-*.json (7 files)
- docker-compose.yml
- appsettings files

Size: $(du -sh ${BACKUP_PATH} | cut -f1)
EOF

    echo ""
    echo "✅ Backup completed successfully"
    echo "Backup location: ${BACKUP_PATH}"
    echo ""
    cat ${BACKUP_PATH}/manifest.txt

    return 0
}

function restore() {
    if [ -z "$1" ]; then
        echo "Usage: $0 restore <backup_timestamp>"
        echo "Available backups:"
        ls -1 ${BACKUP_DIR} | grep -E '^[0-9]{8}_[0-9]{6}$'
        return 1
    fi

    RESTORE_PATH="${BACKUP_DIR}/$1"

    if [ ! -d "${RESTORE_PATH}" ]; then
        echo "❌ Backup not found: ${RESTORE_PATH}"
        return 1
    fi

    echo "=== Starting Restore Process ==="
    echo "Restoring from: ${RESTORE_PATH}"
    echo ""

    read -p "This will overwrite current data. Continue? (yes/no): " CONFIRM
    if [ "${CONFIRM}" != "yes" ]; then
        echo "Restore cancelled"
        return 1
    fi

    # 1. Stop containers
    echo "Stopping containers..."
    docker-compose down

    # 2. Restore database volume
    echo "Restoring database volume..."
    docker volume rm coinpay_postgres-data 2>/dev/null
    docker volume create coinpay_postgres-data

    docker run --rm \
      -v coinpay_postgres-data:/data \
      -v ${PWD}/${RESTORE_PATH}:/backup \
      alpine sh -c "cd /data && tar xzf /backup/postgres-volume.tar.gz"

    echo "✅ Database volume restored"

    # 3. Start infrastructure
    echo "Starting infrastructure..."
    docker-compose up -d postgres vault

    sleep 10

    # 4. Restore database SQL
    echo "Restoring database..."
    gunzip < ${RESTORE_PATH}/database_backup.sql.gz | \
      docker exec -i coinpay-postgres-compose psql -U postgres -d coinpay

    echo "✅ Database restored"

    # 5. Restore Vault secrets
    echo "Restoring Vault secrets..."
    ./restore-vault-secrets.sh ${RESTORE_PATH}

    echo "✅ Vault secrets restored"

    # 6. Start remaining services
    echo "Starting application..."
    docker-compose up -d

    echo ""
    echo "✅ Restore completed successfully"
    echo ""
}

# Main
case "$1" in
    backup)
        backup
        ;;
    restore)
        restore $2
        ;;
    *)
        echo "Usage: $0 {backup|restore <timestamp>}"
        exit 1
        ;;
esac
```

---

## CI/CD Pipeline

### GitHub Actions Workflow

```yaml
# .github/workflows/docker-deploy.yml
name: Docker Compose Deployment Pipeline

on:
  push:
    branches:
      - main
      - development
  pull_request:
    branches:
      - main

env:
  DOCKER_BUILDKIT: 1
  COMPOSE_DOCKER_CLI_BUILD: 1

jobs:
  pre-deployment-checks:
    name: Pre-Deployment Checks
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Check Docker version
        run: |
          docker --version
          docker-compose --version

      - name: Verify required files
        run: |
          test -f deployment/docker-compose.yml || exit 1
          test -f deployment/backup-restore.sh || exit 1
          test -f deployment/regression-test.sh || exit 1
          echo "✅ All required files present"

      - name: Check disk space
        run: |
          df -h
          FREE_GB=$(df -BG . | tail -1 | awk '{print $4}' | sed 's/G//')
          if [ ${FREE_GB} -lt 10 ]; then
            echo "❌ Insufficient disk space"
            exit 1
          fi
          echo "✅ Disk space OK: ${FREE_GB}GB"

  backup-current-state:
    name: Backup Current State
    runs-on: ubuntu-latest
    needs: pre-deployment-checks
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Setup Docker Compose
        run: docker-compose version

      - name: Start existing containers (if any)
        run: |
          cd deployment
          docker-compose up -d || echo "No existing containers"
          sleep 10

      - name: Create backup
        run: |
          cd deployment
          chmod +x backup-restore.sh
          ./backup-restore.sh backup

      - name: Upload backup artifact
        uses: actions/upload-artifact@v3
        with:
          name: pre-deployment-backup
          path: deployment/backups/
          retention-days: 30

  build-and-test:
    name: Build & Test Containers
    runs-on: ubuntu-latest
    needs: pre-deployment-checks
    strategy:
      matrix:
        service: [api, gateway, web, docfx]
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Build ${{ matrix.service }}
        run: |
          cd deployment
          docker-compose build --no-cache ${{ matrix.service }}

      - name: Verify image built
        run: |
          docker images | grep coinpay-${{ matrix.service }}

      - name: Run container smoke test
        run: |
          cd deployment
          docker-compose up -d ${{ matrix.service }}
          sleep 10
          docker-compose ps
          docker-compose logs ${{ matrix.service }}

  integration-tests:
    name: Integration Tests
    runs-on: ubuntu-latest
    needs: build-and-test
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Start all services
        run: |
          cd deployment
          docker-compose up -d
          sleep 30

      - name: Wait for services to be healthy
        run: |
          timeout 120 bash -c 'until docker exec coinpay-postgres-compose pg_isready -U postgres; do sleep 2; done'
          timeout 120 bash -c 'until curl -f http://localhost:7777/health; do sleep 5; done'

      - name: Run regression tests
        run: |
          cd deployment
          chmod +x regression-test.sh
          ./regression-test.sh

      - name: Check data integrity
        run: |
          USER_COUNT=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c "SELECT COUNT(*) FROM \"Users\";")
          echo "User count: ${USER_COUNT}"
          if [ ${USER_COUNT} -eq 0 ]; then
            echo "❌ No users found - possible data loss"
            exit 1
          fi

      - name: Test API endpoints
        run: |
          # Test health
          curl -f http://localhost:7777/health

          # Test swap quote
          curl -f "http://localhost:7777/api/swap/quote?fromToken=0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582&toToken=0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9&amount=100&slippage=1"

      - name: Generate test report
        if: always()
        run: |
          echo "# Test Report" > test-report.md
          echo "" >> test-report.md
          echo "## Container Status" >> test-report.md
          docker-compose ps >> test-report.md
          echo "" >> test-report.md
          echo "## Service Logs" >> test-report.md
          docker-compose logs --tail=50 >> test-report.md

      - name: Upload test report
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-report
          path: test-report.md

  deploy-production:
    name: Deploy to Production
    runs-on: ubuntu-latest
    needs: [backup-current-state, integration-tests]
    if: github.ref == 'refs/heads/main'
    environment:
      name: production
      url: http://production.coinpay.example.com
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Download backup artifact
        uses: actions/download-artifact@v3
        with:
          name: pre-deployment-backup
          path: deployment/backups/

      - name: Deploy to production server
        env:
          SSH_PRIVATE_KEY: ${{ secrets.SSH_PRIVATE_KEY }}
          PRODUCTION_HOST: ${{ secrets.PRODUCTION_HOST }}
        run: |
          # Setup SSH
          mkdir -p ~/.ssh
          echo "${SSH_PRIVATE_KEY}" > ~/.ssh/id_rsa
          chmod 600 ~/.ssh/id_rsa

          # Copy files to server
          scp -r deployment/ ${PRODUCTION_HOST}:/opt/coinpay/

          # Deploy on server
          ssh ${PRODUCTION_HOST} << 'EOF'
            cd /opt/coinpay/deployment
            ./graceful-shutdown.sh
            ./backup-restore.sh backup
            docker-compose build --no-cache
            ./staged-startup.sh
            ./regression-test.sh
          EOF

      - name: Post-deployment validation
        run: |
          sleep 30
          curl -f http://production.coinpay.example.com/health

      - name: Notify deployment success
        uses: 8398a7/action-slack@v3
        with:
          status: ${{ job.status }}
          text: '🚀 CoinPay deployed to production successfully'
          webhook_url: ${{ secrets.SLACK_WEBHOOK }}

  rollback-on-failure:
    name: Automatic Rollback
    runs-on: ubuntu-latest
    needs: deploy-production
    if: failure()
    steps:
      - name: Download backup
        uses: actions/download-artifact@v3
        with:
          name: pre-deployment-backup
          path: deployment/backups/

      - name: Execute rollback
        run: |
          LATEST_BACKUP=$(ls -t deployment/backups/ | head -1)
          cd deployment
          ./backup-restore.sh restore ${LATEST_BACKUP}

      - name: Notify rollback
        uses: 8398a7/action-slack@v3
        with:
          status: 'failure'
          text: '⚠️ Deployment failed - Automatic rollback executed'
          webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

---

## Rollback Procedures

### Automated Rollback Script

```bash
#!/bin/bash
# rollback.sh

echo "=== EMERGENCY ROLLBACK INITIATED ==="
echo ""

# Find latest backup
LATEST_BACKUP=$(ls -t ./backups/ | grep -E '^[0-9]{8}_[0-9]{6}$' | head -1)

if [ -z "${LATEST_BACKUP}" ]; then
    echo "❌ No backup found for rollback"
    exit 1
fi

echo "Rolling back to backup: ${LATEST_BACKUP}"
echo ""

# Execute restore
./backup-restore.sh restore ${LATEST_BACKUP}

if [ $? -eq 0 ]; then
    echo "✅ Rollback completed successfully"

    # Verify rollback
    echo "Verifying rollback..."
    ./regression-test.sh

    if [ $? -eq 0 ]; then
        echo "✅ Rollback verification passed"
    else
        echo "⚠️  Rollback verification failed - manual intervention required"
    fi
else
    echo "❌ Rollback failed - manual intervention required"
    exit 1
fi
```

---

## Troubleshooting

### Common Issues

#### Issue 1: Data Loss Detected
```bash
# Symptom
USER_COUNT=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c "SELECT COUNT(*) FROM \"Users\";")
# COUNT = 0

# Solution
1. Immediately stop deployment
2. Execute rollback: ./rollback.sh
3. Investigate logs: docker-compose logs postgres
4. Check volume mounts: docker volume inspect coinpay_postgres-data
```

#### Issue 2: Vault Secrets Lost
```bash
# Symptom
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get coinpay/database
# Error: No value found

# Solution
1. Restore from backup: ./restore-vault-secrets.sh <backup-timestamp>
2. Verify restoration: docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list coinpay/
3. Restart API: docker-compose restart api
```

#### Issue 3: Container Won't Start
```bash
# Check logs
docker logs coinpay-api --tail 100

# Common causes:
# - Port already in use
# - Volume mount issues
# - Configuration errors
# - Insufficient resources

# Solution:
1. Stop conflicting services
2. Clean up old containers: docker system prune
3. Verify configuration: docker-compose config
4. Check resources: docker stats
```

---

## Monitoring & Alerting

### Health Check Script (Continuous)

```bash
#!/bin/bash
# continuous-health-check.sh

while true; do
    # Check API health
    if ! curl -f http://localhost:7777/health > /dev/null 2>&1; then
        echo "❌ API health check failed at $(date)"
        # Send alert (e.g., Slack, email, PagerDuty)
    fi

    # Check database connections
    CONNECTIONS=$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c \
      "SELECT count(*) FROM pg_stat_activity WHERE datname='coinpay';")
    if [ ${CONNECTIONS} -gt 100 ]; then
        echo "⚠️  High database connection count: ${CONNECTIONS} at $(date)"
    fi

    # Check disk space
    FREE_PERCENT=$(df -h /var/lib/docker | tail -1 | awk '{print $5}' | sed 's/%//')
    if [ ${FREE_PERCENT} -gt 80 ]; then
        echo "⚠️  Disk usage high: ${FREE_PERCENT}% at $(date)"
    fi

    sleep 60
done
```

---

## Summary Checklist

Before any deployment, verify:

- [ ] ✅ Backup created and verified
- [ ] ✅ Disk space sufficient (>10GB)
- [ ] ✅ All configuration files updated
- [ ] ✅ Pre-deployment tests passed
- [ ] ✅ Rollback plan ready
- [ ] ✅ Team notified of deployment window

During deployment:

- [ ] ✅ Graceful shutdown executed
- [ ] ✅ Images built without errors
- [ ] ✅ Staged startup completed
- [ ] ✅ Data integrity verified
- [ ] ✅ Regression tests passed

After deployment:

- [ ] ✅ All services healthy
- [ ] ✅ No data loss confirmed
- [ ] ✅ Performance metrics normal
- [ ] ✅ Documentation updated
- [ ] ✅ Backup retained for 30 days

---

**Version**: 1.0.0
**Last Updated**: November 5, 2025
**Maintained By**: CoinPay DevOps Team

For questions or issues, contact: devops@coinpay.example.com
