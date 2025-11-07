# Archive Folder

**Date**: November 5, 2025
**Purpose**: Historical and reference documentation moved from deployment/

---

## Why These Files Were Archived

The deployment folder was cleaned to keep only essential files for CI/CD and Docker Compose deployment. All historical documentation, test results, and redundant scripts were moved to this Archive folder for reference.

---

## Archived Files

### Historical Documentation (11 files)
- `DATABASE_TEST_RESULTS.md` - Test results from initial setup
- `DEPLOYMENT_COMPLETE.md` - Historical completion summary
- `DEPLOYMENT-SETUP-SUMMARY.md` - Executive summary of deployment setup
- `DEPLOYMENT-STRUCTURE-COMPLETE.md` - Structure overview documentation
- `INDEX.md` - File organization index (outdated after cleanup)
- `MANUAL_REVIEW_SUMMARY.md` - Manual review checklist
- `PORT_CHANGE_COMPLETE.md` - Port change documentation (5433→5432)
- `TEST_USER_SETUP_COMPLETE.md` - Test user setup documentation
- `DOCKER_DEPLOYMENT_GUIDE.md` - Older deployment guide (replaced by DOCKER-COMPOSE-DEPLOYMENT.md)
- `DOCKER.md` - Basic Docker reference
- `TESTNET-GUIDE.md` - Testnet deployment guide

### Helper Scripts (5 files)
- `quick-test.sh` - Quick validation script (covered by regression-test.sh)
- `start-coinpay.sh` - Manual startup script (covered by docker-compose commands)
- `start-coinpay.ps1` - PowerShell startup script (covered by docker-compose commands)
- `test-api-with-user.ps1` - API testing script (covered by regression-test.sh)
- `create-test-user.sql` - SQL script for test user creation

**Total Archived**: 16 files (~150KB)

---

## Essential Files Kept in deployment/

### Core Deployment Files (3 files)
- `docker-compose.yml` - Container orchestration configuration
- `backup-restore.sh` - Backup/restore automation
- `regression-test.sh` - Full regression test suite (18 tests)

### Documentation (2 files)
- `DOCKER-COMPOSE-DEPLOYMENT.md` - Comprehensive deployment guide (31KB)
- `README.md` - Quick start guide (9KB)

### CI/CD Pipeline (1 file)
- `.github-workflows-docker-deploy.yml` - GitHub Actions workflow

**Total Essential**: 6 files (~62KB)

---

## File Access

All archived files are still accessible in this Archive folder for reference or if needed in the future. The essential deployment files remain in the parent `deployment/` folder for active use.

---

**Cleanup Date**: November 5, 2025
**Reason**: Streamline deployment folder for CI/CD and production use
