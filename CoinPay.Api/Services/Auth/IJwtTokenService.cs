using System.Security.Claims;
using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Auth;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}
