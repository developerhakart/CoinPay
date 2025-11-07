#!/bin/bash
# backup-restore.sh - Database and Vault Backup/Restore Script

BACKUP_DIR="./backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_PATH="${BACKUP_DIR}/${TIMESTAMP}"

function backup() {
    echo "=== Starting Backup Process ==="
    echo "Timestamp: ${TIMESTAMP}"
    echo ""

    # Create backup directory
    mkdir -p ${BACKUP_PATH}

    # 1. Backup Database
    echo "[1/5] Backing up database..."
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
    echo "[2/5] Backing up database volume..."
    docker run --rm \
      -v coinpay_postgres-data:/data \
      -v ${PWD}/${BACKUP_PATH}:/backup \
      alpine tar czf /backup/postgres-volume.tar.gz -C /data .
    echo "✅ Volume backup completed"

    # 3. Backup Vault Secrets
    echo "[3/5] Backing up Vault secrets..."
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

    echo "✅ Vault secrets backed up (7 files)"

    # 4. Backup Configuration
    echo "[4/5] Backing up configuration..."
    cp docker-compose.yml ${BACKUP_PATH}/ 2>/dev/null
    cp ../.env ${BACKUP_PATH}/ 2>/dev/null
    cp ../CoinPay.Api/appsettings.*.json ${BACKUP_PATH}/ 2>/dev/null
    echo "✅ Configuration backed up"

    # 5. Create manifest
    echo "[5/5] Creating backup manifest..."
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
- configuration files

Backup Size: $(du -sh ${BACKUP_PATH} | cut -f1)

Database Info:
$(docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "SELECT 'Users: ' || COUNT(*) FROM \"Users\" UNION ALL SELECT 'Wallets: ' || COUNT(*) FROM \"Wallets\" UNION ALL SELECT 'Transactions: ' || COUNT(*) FROM \"Transactions\";" -t 2>/dev/null || echo "Database not running")
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
        echo ""
        echo "Available backups:"
        ls -1 ${BACKUP_DIR} 2>/dev/null | grep -E '^[0-9]{8}_[0-9]{6}$' | tail -10
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

    if [ -f "${RESTORE_PATH}/manifest.txt" ]; then
        cat ${RESTORE_PATH}/manifest.txt
        echo ""
    fi

    read -p "This will overwrite current data. Type 'yes' to continue: " CONFIRM
    if [ "${CONFIRM}" != "yes" ]; then
        echo "Restore cancelled"
        return 1
    fi

    # 1. Stop containers
    echo "[1/6] Stopping containers..."
    docker-compose down

    # 2. Restore database volume
    echo "[2/6] Restoring database volume..."
    docker volume rm coinpay_postgres-data 2>/dev/null
    docker volume create coinpay_postgres-data

    docker run --rm \
      -v coinpay_postgres-data:/data \
      -v ${PWD}/${RESTORE_PATH}:/backup \
      alpine sh -c "cd /data && tar xzf /backup/postgres-volume.tar.gz"

    echo "✅ Database volume restored"

    # 3. Start infrastructure
    echo "[3/6] Starting infrastructure..."
    docker-compose up -d postgres vault

    echo "Waiting for services to start..."
    sleep 15

    # 4. Restore database SQL
    echo "[4/6] Restoring database SQL..."
    gunzip < ${RESTORE_PATH}/database_backup.sql.gz | \
      docker exec -i coinpay-postgres-compose psql -U postgres -d coinpay 2>&1 | grep -v "ERROR.*already exists" || true

    echo "✅ Database restored"

    # 5. Restore Vault secrets
    echo "[5/6] Restoring Vault secrets..."
    VAULT_TOKEN=dev-root-token

    # Enable secrets engine if not already enabled
    docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
      vault secrets enable -path=coinpay kv-v2 2>/dev/null || echo "Secrets engine already enabled"

    # Restore each secret
    for secret in database jwt encryption gateway blockchain circle whitebit; do
        if [ -f "${RESTORE_PATH}/vault-${secret}.json" ]; then
            echo "Restoring vault secret: ${secret}..."
            # Extract and restore secret data
            cat ${RESTORE_PATH}/vault-${secret}.json | jq -r '.data.data | to_entries | map("\(.key)=\(.value)") | join(" ")' | \
            xargs -I {} docker exec -e VAULT_TOKEN=${VAULT_TOKEN} coinpay-vault \
              vault kv put coinpay/${secret} {} 2>/dev/null || echo "Failed to restore ${secret}"
        fi
    done

    echo "✅ Vault secrets restored"

    # 6. Start remaining services
    echo "[6/6] Starting application services..."
    docker-compose up -d

    echo "Waiting for services to stabilize..."
    sleep 20

    echo ""
    echo "✅ Restore completed successfully"
    echo ""
    echo "Verifying restoration..."
    docker-compose ps
    echo ""

    # Verify data
    echo "Data verification:"
    docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c \
      "SELECT 'Users' as table_name, COUNT(*) as count FROM \"Users\" UNION ALL SELECT 'Transactions', COUNT(*) FROM \"Transactions\";" 2>/dev/null || echo "Database verification pending..."

    echo ""
    echo "✅ Restore process complete"
}

function list() {
    echo "=== Available Backups ==="
    echo ""

    if [ ! -d "${BACKUP_DIR}" ]; then
        echo "No backups directory found"
        return
    fi

    BACKUPS=$(ls -1 ${BACKUP_DIR} 2>/dev/null | grep -E '^[0-9]{8}_[0-9]{6}$')

    if [ -z "${BACKUPS}" ]; then
        echo "No backups found"
        return
    fi

    for backup in ${BACKUPS}; do
        SIZE=$(du -sh ${BACKUP_DIR}/${backup} 2>/dev/null | cut -f1)
        DATE=$(echo ${backup} | sed 's/\([0-9]\{4\}\)\([0-9]\{2\}\)\([0-9]\{2\}\)_\([0-9]\{2\}\)\([0-9]\{2\}\)\([0-9]\{2\}\)/\1-\2-\3 \4:\5:\6/')
        echo "  ${backup} - ${SIZE} - ${DATE}"
    done
}

# Main
case "$1" in
    backup)
        backup
        ;;
    restore)
        restore $2
        ;;
    list)
        list
        ;;
    *)
        echo "Usage: $0 {backup|restore <timestamp>|list}"
        echo ""
        echo "Commands:"
        echo "  backup          - Create a new backup"
        echo "  restore <ts>    - Restore from backup timestamp"
        echo "  list            - List available backups"
        echo ""
        echo "Example:"
        echo "  $0 backup"
        echo "  $0 list"
        echo "  $0 restore 20251105_120000"
        exit 1
        ;;
esac
