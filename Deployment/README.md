# CoinPay Docker Deployment

Production-ready Docker Compose deployment with **zero data loss** guarantees.

## ⚡ Quick Start

**For manual build, start, and stop operations, see:**
📂 **[Start Folder](./Start/README.md)** - All deployment scripts and guides

```bash
# Build Docker images
./Start/build-coinpay.sh

# Start all services (includes Vault setup)
./Start/start-coinpay.sh

# Stop all services
./Start/stop-coinpay.sh
```

## 📁 Project Structure

| Location | Purpose |
|----------|---------|
| **`Start/`** | **Manual deployment scripts & guides** |
| `docker-compose.yml` | Container orchestration configuration |
| `regression-test.sh` | Post-deployment regression testing |
| `.github-workflows-docker-deploy.yml` | CI/CD pipeline template |
| `Archive/` | Historical deployment documentation |

## 📂 Start Folder Contents

The `Start/` folder contains everything needed for manual deployment:

**Scripts:**
- `build-coinpay.ps1` / `.sh` - Build all Docker images
- `start-coinpay.ps1` / `.sh` - Start all services with Vault setup
- `stop-coinpay.ps1` / `.sh` - Stop all services
- `backup-restore.sh` - Database backup/restore utility

**Guides:**
- `README.md` - Complete deployment guide
- `VAULT_ADMIN_GUIDE.md` - Vault configuration management
- `DOCKER-COMPOSE-DEPLOYMENT.md` - Docker Compose reference
- `SAFE-DEPLOYMENT-STRATEGY.md` - Production best practices

## 🚀 Deployment Steps

### 1. Pre-Deployment

```bash
# Check prerequisites
docker --version          # Requires >= 20.10.0
docker-compose --version  # Requires >= 2.0.0

# Check disk space (minimum 10GB)
df -h

# Create backup
chmod +x backup-restore.sh
./backup-restore.sh backup
```

### 2. Deploy

```bash
# Stop current containers
docker-compose down

# Build all images
docker-compose build --no-cache

# Start containers
docker-compose up -d

# Wait for health checks
sleep 30
```

### 3. Validate

```bash
# Run regression tests
chmod +x regression-test.sh
./regression-test.sh

# Check container status
docker-compose ps

# Verify data integrity
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c \
  "SELECT 'Users' as table, COUNT(*) FROM \"Users\" UNION ALL SELECT 'Transactions', COUNT(*) FROM \"Transactions\";"
```

## 🛡️ Data Protection

### Automatic Backups

Backups include:
- ✅ Database SQL dump
- ✅ Database volume
- ✅ Vault secrets (7 files)
- ✅ Configuration files
- ✅ Manifest with metadata

```bash
# Create backup
./backup-restore.sh backup

# List backups
./backup-restore.sh list

# Restore from backup
./backup-restore.sh restore 20251105_120000
```

### Volume Persistence

Data is stored in Docker named volumes:
- `postgres-data` - Database files
- `vault-data` - Vault storage

These volumes persist even when containers are removed.

## 🧪 Testing

### Regression Test Suite

Tests 9 phases with 18+ checks:

1. **Infrastructure Tests** (4 tests)
   - Docker health
   - Container status
   - Database connectivity
   - Vault initialization

2. **API Health Tests** (2 tests)
   - Health endpoint
   - Swagger documentation

3. **Authentication Tests** (1 test)
   - Username check endpoint

4. **Database Integrity** (4 tests)
   - Table count verification
   - Data access tests
   - SwapTransactions table

5. **Vault Secrets** (1 test)
   - All 7 secrets accessible

6. **API Endpoints** (1 test)
   - Swap quote functionality

7. **Frontend Tests** (2 tests)
   - Web application
   - Documentation site

8. **Performance Tests** (2 tests)
   - Response time
   - Connection pool

9. **Security Tests** (1 test)
   - Protected endpoints

```bash
# Run full test suite
./regression-test.sh

# Expected output:
# ✅ ALL REGRESSION TESTS PASSED
# Success Rate: 100%
```

## 📦 CI/CD Pipeline

### GitHub Actions Workflow

Place `.github-workflows-docker-deploy.yml` in `.github/workflows/` directory.

**Workflow stages:**
1. Pre-deployment validation
2. Backup current state
3. Build all containers
4. Integration tests
5. Security scanning
6. Deploy to staging/production
7. Automatic rollback on failure

**Triggers:**
- Push to `main` → Deploy to production
- Push to `development` → Deploy to staging
- Pull request → Run tests only

## 🔄 Rollback Procedure

### Automatic Rollback

If deployment fails, automatic rollback executes:

```bash
# Emergency rollback to latest backup
./backup-restore.sh restore $(ls -t backups/ | head -1)
```

### Manual Rollback

```bash
# 1. List available backups
./backup-restore.sh list

# 2. Choose backup timestamp
./backup-restore.sh restore 20251105_120000

# 3. Verify restoration
./regression-test.sh
```

## 📊 Service Ports

| Service | Port | URL |
|---------|------|-----|
| Frontend | 3000 | http://localhost:3000 |
| Gateway | 5000 | http://localhost:5000 |
| API | 7777 | http://localhost:7777 |
| Documentation | 8080 | http://localhost:8080 |
| PostgreSQL | 5432 | localhost:5432 |
| Vault | 8200 | http://localhost:8200 |

## 🔐 Database Credentials

```
Host: localhost
Port: 5432
Database: coinpay
Username: postgres
Password: root
```

**Connection string:**
```
Host=localhost;Port=5432;Database=coinpay;Username=postgres;Password=root
```

## 🔧 Common Commands

### Container Management

```bash
# Start all containers
docker-compose up -d

# Stop all containers
docker-compose down

# Restart specific service
docker-compose restart api

# View logs
docker-compose logs -f api

# View all logs
docker-compose logs -f

# Check status
docker-compose ps
```

### Database Operations

```bash
# Connect to database
docker exec -it coinpay-postgres-compose psql -U postgres -d coinpay

# Backup database
docker exec coinpay-postgres-compose pg_dump -U postgres -d coinpay | gzip > backup.sql.gz

# Restore database
gunzip < backup.sql.gz | docker exec -i coinpay-postgres-compose psql -U postgres -d coinpay

# Check database size
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "SELECT pg_size_pretty(pg_database_size('coinpay'));"
```

### Vault Operations

```bash
# List secrets
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list coinpay/

# Get secret
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get coinpay/database

# Put secret
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put coinpay/test key=value

# Access Vault UI
# URL: http://localhost:8200/ui
# Token: dev-root-token
```

## 🐛 Troubleshooting

### Issue: Container won't start

```bash
# Check logs
docker logs coinpay-api --tail 100

# Check if port is in use
netstat -an | findstr :7777

# Clean up and restart
docker-compose down -v
docker-compose up -d
```

### Issue: Database connection failed

```bash
# Check if PostgreSQL is ready
docker exec coinpay-postgres-compose pg_isready -U postgres

# Check connection strings
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "SELECT 1;"

# Restart database
docker-compose restart postgres
```

### Issue: Data loss detected

```bash
# Immediately execute rollback
./backup-restore.sh restore $(ls -t backups/ | head -1)

# Verify restoration
./regression-test.sh
```

### Issue: Vault secrets missing

```bash
# Check Vault status
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault status

# Re-create secrets
# (See DOCKER-COMPOSE-DEPLOYMENT.md for full procedure)

# Restart API
docker-compose restart api
```

## 📚 Documentation

- **[DOCKER-COMPOSE-DEPLOYMENT.md](DOCKER-COMPOSE-DEPLOYMENT.md)** - Complete deployment guide (300+ lines)
  - Data protection strategy
  - Step-by-step deployment
  - Backup/restore procedures
  - CI/CD pipeline details
  - Rollback procedures
  - Troubleshooting guide

## 🔒 Security Considerations

### Production Deployment

Before deploying to production:

1. **Change default passwords**
   ```bash
   # Update database password in docker-compose.yml
   POSTGRES_PASSWORD: <strong-password>
   ```

2. **Update Vault token**
   ```bash
   # Use production token instead of dev-root-token
   VAULT_DEV_ROOT_TOKEN_ID: <production-token>
   ```

3. **Enable SSL/TLS**
   ```yaml
   # Add SSL configuration to nginx
   - ./ssl:/etc/nginx/ssl
   ```

4. **Restrict CORS**
   ```json
   // Update appsettings.Production.json
   "AllowedOrigins": ["https://your-production-domain.com"]
   ```

5. **Enable authentication**
   ```bash
   # Ensure all endpoints have [Authorize] attributes
   ```

## 📞 Support

### Getting Help

- **Documentation**: Read DOCKER-COMPOSE-DEPLOYMENT.md
- **Issues**: Check troubleshooting section
- **Logs**: `docker-compose logs -f`
- **Health Check**: `curl http://localhost:7777/health`

### Monitoring

```bash
# Continuous health monitoring
while true; do
  curl -f http://localhost:7777/health || echo "Health check failed at $(date)"
  sleep 60
done
```

## ✅ Success Criteria

Deployment is successful when:

- [ ] All 6 containers running
- [ ] Health endpoint returns "Healthy"
- [ ] All regression tests pass
- [ ] No data loss (counts match)
- [ ] Vault secrets accessible
- [ ] Frontend loads correctly
- [ ] API endpoints respond < 1s
- [ ] Database connections < 50

## 📈 Maintenance

### Daily Tasks

- Check container health: `docker-compose ps`
- Review logs: `docker-compose logs --tail=100`
- Monitor disk space: `df -h`

### Weekly Tasks

- Create backup: `./backup-restore.sh backup`
- Clean old backups: Keep last 30 days
- Review security logs
- Check for updates

### Monthly Tasks

- Update Docker images: `docker-compose pull`
- Run full regression tests: `./regression-test.sh`
- Review and rotate secrets
- Performance audit

## 📝 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2025-11-05 | Initial release with complete CI/CD |

---

**Status**: Production Ready ✅
**Maintained By**: CoinPay DevOps Team
**Last Updated**: November 5, 2025

For detailed deployment procedures, see [DOCKER-COMPOSE-DEPLOYMENT.md](DOCKER-COMPOSE-DEPLOYMENT.md)
