# Circle Webhook Setup Guide

## Overview
Circle webhooks automatically notify your application when transaction status changes, eliminating the need for polling the API.

## What Was Implemented

### 1. Webhook Models (`CircleWebhookModels.cs`)
- `CircleWebhookNotification` - Main webhook payload structure
- `CircleWebhookData` - Transaction data within the notification

### 2. Webhook Handler (`CircleWebhookHandler.cs`)
- Processes incoming webhook notifications
- Updates transaction status in database automatically
- Maps Circle states to application statuses:
  - `CONFIRMED` / `COMPLETE` → `Completed`
  - `FAILED` / `CANCELLED` / `DENIED` → `Failed`
  - Others → `Pending`

### 3. Webhook Endpoint (`Program.cs`)
- **URL**: `POST /api/webhooks/circle`
- Receives notifications from Circle
- No authentication required (Circle doesn't use JWT)
- Returns `200 OK` on success

## Setup Instructions

### Step 1: Expose Your API Publicly

Circle needs to reach your webhook endpoint. You have two options:

#### Option A: Using ngrok (Development/Testing)
```bash
# Install ngrok: https://ngrok.com/download

# Start ngrok tunnel to your API
ngrok http http://localhost:5100

# You'll get a public URL like: https://abc123.ngrok.io
```

####Option B: Deploy to Production
Deploy your API to a cloud provider (AWS, Azure, Google Cloud, etc.) with a public domain.

### Step 2: Configure Webhook in Circle Dashboard

1. **Log in to Circle Console**:
   - Go to: https://console.circle.com/

2. **Navigate to Webhooks**:
   - Sidebar → Developer Settings → Webhooks

3. **Create New Webhook**:
   - Click "Add Subscription"

4. **Configure Webhook**:
   ```
   Endpoint URL: https://your-domain.com/api/webhooks/circle
   OR (if using ngrok): https://abc123.ngrok.io/api/webhooks/circle

   Event Types: Select "transactions.updated"

   Description: CoinPay transaction status updates
   ```

5. **Save Webhook**:
   - Click "Create Subscription"
   - Circle will send a test notification to verify the endpoint

### Step 3: Verify Webhook is Working

1. **Create a Test Transaction**:
   - Use your app to create a POL transfer
   - Check logs: `docker logs coinpay-api`

2. **Look for Webhook Logs**:
   ```
   [INF] Received Circle webhook notification: Type=transactions.updated, NotificationId=...
   [INF] Transaction {Id} status updated via webhook: Pending → Completed, CircleTransactionId: ...
   ```

3. **Check Database**:
   ```sql
   SELECT "Id", "TransactionId", "Status", "CompletedAt"
   FROM "Transactions"
   WHERE "Currency" = 'POL'
   ORDER BY "CreatedAt" DESC;
   ```

## Transaction Status Flow

```
1. User Creates Transfer
   ↓
2. API Calls Circle (ExecuteDeveloperTransferAsync)
   ↓
3. Transaction Stored with Status="Pending"
   ↓
4. Circle Processes Transfer (5-30 seconds)
   ↓
5. Circle Sends Webhook: "state": "COMPLETE"
   ↓
6. Webhook Handler Updates DB: Status="Completed"
   ↓
7. User Sees Updated Status in UI
```

## Webhook Payload Example

```json
{
  "notificationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "subscriptionId": "sub_123",
  "notificationType": "transactions.updated",
  "timestamp": "2025-11-04T10:30:00.000Z",
  "notification": {
    "id": "e2cde5d2-edf9-5343-b13b-7172e1949501",
    "state": "COMPLETE",
    "blockchain": "MATIC-AMOY",
    "txHash": "0x2cf7216853dbf6884bb6d9b810ba16774fe99bdfdca077e221f9318ffc649d5f",
    "sourceAddress": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
    "destinationAddress": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
    "amounts": ["0.0012"],
    "walletId": "fef70777-cb2d-5096-a0ea-15dba5662ce6",
    "createDate": "2025-11-04T05:14:25.916Z",
    "updateDate": "2025-11-04T05:15:30.000Z"
  }
}
```

## Troubleshooting

### Webhook Not Receiving Notifications

1. **Check Endpoint Accessibility**:
   ```bash
   curl -X POST https://your-domain.com/api/webhooks/circle \
     -H "Content-Type: application/json" \
     -d '{"notificationId":"test","notificationType":"transactions.updated","timestamp":"2025-11-04T10:00:00Z","notification":{"id":"test-123","state":"COMPLETE"}}'
   ```

2. **Check Circle Console**:
   - Go to Webhooks → Your Subscription
   - View "Recent Deliveries"
   - Check for failed attempts

3. **Check API Logs**:
   ```bash
   docker logs coinpay-api | grep webhook
   ```

4. **Verify Ngrok is Running**:
   ```bash
   # Check ngrok status
   curl http://localhost:4040/api/tunnels
   ```

### Transaction Not Updating

1. **Check Webhook Handler Logs**:
   ```bash
   docker logs coinpay-api | grep "Processing Circle webhook"
   ```

2. **Verify Transaction ID Exists**:
   ```sql
   SELECT * FROM "Transactions" WHERE "TransactionId" = 'your-circle-tx-id';
   ```

3. **Check for Errors**:
   ```bash
   docker logs coinpay-api | grep "Error processing Circle webhook"
   ```

## Security Considerations

### Current Implementation
- ✅ Webhook endpoint is publicly accessible
- ❌ **No signature verification** (to be added in future)

### Recommended Improvements (TODO)

1. **Add Signature Verification**:
   ```csharp
   // Circle signs webhooks with a secret key
   // Verify X-Circle-Signature header
   ```

2. **Rate Limiting**:
   ```csharp
   // Limit webhook requests to prevent abuse
   ```

3. **IP Whitelisting**:
   ```csharp
   // Only accept requests from Circle's IP ranges
   ```

## Testing Webhooks Locally

### Using Webhook.site (Simple)
1. Go to https://webhook.site
2. Copy the unique URL
3. Configure in Circle Console
4. View incoming requests in real-time

### Using Ngrok (Full Integration)
```bash
# Terminal 1: Start API
docker-compose up -d

# Terminal 2: Start ngrok
ngrok http http://localhost:5100

# Use the ngrok URL in Circle Console
# Example: https://abc123.ngrok.io/api/webhooks/circle
```

## Monitoring Webhooks

### View Webhook Logs
```bash
# All webhook activity
docker logs coinpay-api | grep "Circle webhook"

# Successful updates
docker logs coinpay-api | grep "status updated via webhook"

# Errors
docker logs coinpay-api | grep "Error processing Circle webhook"
```

### Check Webhook Statistics in Circle
1. Go to Circle Console → Webhooks
2. Click on your subscription
3. View:
   - Total deliveries
   - Success rate
   - Recent delivery attempts
   - Failed deliveries with retry status

## Next Steps

1. ✅ **Webhooks Implemented** - Automatic transaction updates
2. 🔄 **Disable Polling** - Background monitoring service is now redundant
3. 🔒 **Add Signature Verification** - For production security
4. 📊 **Add Webhook Analytics** - Track webhook performance
5. 🔔 **Add User Notifications** - Notify users when transactions complete

## Support

- **Circle Documentation**: https://developers.circle.com/w3s/docs/web3-services-webhooks
- **Circle Console**: https://console.circle.com/
- **CoinPay API Logs**: `docker logs coinpay-api`

---

**Status**: ✅ Webhooks fully implemented and ready to use!
**Last Updated**: November 4, 2025
