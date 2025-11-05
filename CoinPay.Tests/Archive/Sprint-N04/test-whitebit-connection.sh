#!/bin/bash

echo "Testing WhiteBit Connection with Mock Mode"
echo "==========================================="
echo ""

# Step 1: Login
echo "Step 1: Getting authentication token..."
LOGIN_RESPONSE=$(curl -s -X POST "http://localhost:7777/api/auth/login/dev" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser"}')

TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
    echo "❌ Failed to get authentication token"
    echo "Response: $LOGIN_RESPONSE"
    exit 1
fi

echo "✅ Token obtained: ${TOKEN:0:30}..."
echo ""

# Step 2: Connect WhiteBit
echo "Step 2: Connecting WhiteBit with test credentials..."
CONNECT_RESPONSE=$(curl -s -X POST "http://localhost:7777/api/exchange/whitebit/connect" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"apiKey":"test_api_key_123","apiSecret":"test_api_secret_456"}')

echo "Response:"
echo "$CONNECT_RESPONSE" | python -m json.tool 2>/dev/null || echo "$CONNECT_RESPONSE"
echo ""

# Check if successful
if echo "$CONNECT_RESPONSE" | grep -q "connectionId"; then
    echo "✅ SUCCESS: WhiteBit connection created!"
    CONNECTION_ID=$(echo $CONNECT_RESPONSE | grep -o '"connectionId":"[^"]*' | cut -d'"' -f4)
    echo "Connection ID: $CONNECTION_ID"
elif echo "$CONNECT_RESPONSE" | grep -q "already connected"; then
    echo "⚠️  WhiteBit account already connected (need to disconnect first)"
elif echo "$CONNECT_RESPONSE" | grep -q "Invalid API credentials"; then
    echo "❌ FAILED: Invalid API credentials (mock mode not working)"
    echo ""
    echo "Check API logs:"
    echo "docker logs coinpay-api --tail 20"
else
    echo "❌ Unexpected response"
fi

echo ""
echo "Step 3: Checking connection status..."
STATUS_RESPONSE=$(curl -s -X GET "http://localhost:7777/api/exchange/whitebit/status" \
  -H "Authorization: Bearer $TOKEN")

echo "Status Response:"
echo "$STATUS_RESPONSE" | python -m json.tool 2>/dev/null || echo "$STATUS_RESPONSE"
