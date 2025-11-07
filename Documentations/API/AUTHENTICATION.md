# Authentication API

Complete reference for CoinPay Authentication API - JWT-based authentication system.

## Overview

CoinPay uses JWT (JSON Web Tokens) for API authentication. All protected endpoints require a valid JWT token in the Authorization header.

## Endpoints

### Login

Authenticate user and receive JWT token.

**Endpoint:** `POST /api/auth/login`

**Authentication:** None required

**Request Body:**
```json
{
  "username": "testuser",
  "password": "your-password"
}
```

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 1440,
  "refreshToken": "refresh_abc123xyz",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "testuser",
    "email": "user@example.com",
    "walletAddress": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
    "circleWalletId": "fef70777-cb2d-5096-a0ea-15dba5662ce6"
  }
}
```

### Register

Create a new user account.

**Endpoint:** `POST /api/auth/register`

**Request Body:**
```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "SecurePassword123!"
}
```

**Response:** `201 Created`
```json
{
  "message": "User registered successfully",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "username": "newuser"
}
```

### Refresh Token

Refresh expired JWT token.

**Endpoint:** `POST /api/auth/refresh`

**Request Body:**
```json
{
  "refreshToken": "refresh_abc123xyz"
}
```

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 1440
}
```

### Logout

Invalidate current session.

**Endpoint:** `POST /api/auth/logout`

**Authentication:** Required (JWT)

**Response:** `200 OK`
```json
{
  "message": "Logged out successfully"
}
```

## Using JWT Tokens

Include token in Authorization header:

```
Authorization: Bearer YOUR_JWT_TOKEN
```

**Example:**
```bash
curl -X GET http://localhost:7777/api/wallet \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## Token Expiration

- **Access Token:** 24 hours (1440 minutes)
- **Refresh Token:** 7 days

## Code Examples

### Node.js
```javascript
async function login(username, password) {
  const response = await axios.post(
    'http://localhost:7777/api/auth/login',
    { username, password }
  );

  const { token, user } = response.data;

  // Store token securely
  localStorage.setItem('authToken', token);

  return { token, user };
}
```

### Python
```python
def login(username, password):
    response = requests.post(
        'http://localhost:7777/api/auth/login',
        json={'username': username, 'password': password}
    )

    data = response.json()
    return data['token'], data['user']
```

## Best Practices

1. **Store tokens securely** (httpOnly cookies for web apps)
2. **Never expose tokens** in URLs or logs
3. **Implement token refresh** before expiration
4. **Use HTTPS** in production
5. **Validate token** on every protected request

## Rate Limits

| Endpoint | Rate Limit |
|----------|------------|
| POST /api/auth/login | 5 requests/minute |
| POST /api/auth/register | 3 requests/minute |
| POST /api/auth/refresh | 10 requests/minute |

---

**Last Updated:** November 7, 2025
