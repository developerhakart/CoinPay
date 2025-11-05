-- Create Test User for CoinPay API Testing
-- Run this in PostgreSQL to create a test user

-- Insert test user
INSERT INTO "Users" (
    "Username",
    "CircleUserId",
    "CredentialId",
    "CreatedAt",
    "LastLoginAt",
    "WalletAddress",
    "CircleWalletId"
) VALUES (
    'testuser',
    'test-circle-user-id-12345',
    'test-credential-id-67890',
    NOW(),
    NOW(),
    '0x1234567890123456789012345678901234567890',
    'test-wallet-id-abc123'
);

-- Verify user was created
SELECT "Id", "Username", "CircleUserId", "WalletAddress", "CreatedAt"
FROM "Users"
WHERE "Username" = 'testuser';

-- Get the user ID (you'll need this for JWT token generation)
SELECT "Id" FROM "Users" WHERE "Username" = 'testuser';
