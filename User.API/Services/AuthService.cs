using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserApi.Models;
using UserApi.Services.Interfaces;

namespace UserApi.Services;

public sealed class AuthService(IConfiguration configuration) : IAuthService
{
    public LoginResponse? Login(string suppliedUsername, string suppliedPassword)
    {
        var username = configuration["Authentication:Username"];
        var password = configuration["Authentication:Password"];
        var signingKey = configuration["Authentication:SigningKey"];
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32)
            throw new InvalidOperationException("Authentication is not configured correctly.");

        if (!string.Equals(suppliedUsername, username, StringComparison.Ordinal) || suppliedPassword != password)
            return null;

        var role = configuration["Authentication:Role"];
        var tokenLifetimeMinutes = configuration.GetValue<int?>("Authentication:TokenLifetimeMinutes");
        if (string.IsNullOrWhiteSpace(role) || tokenLifetimeMinutes is null or <= 0)
            throw new InvalidOperationException("Authentication role and token lifetime are not configured correctly.");

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes.Value);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            configuration["Authentication:Issuer"],
            configuration["Authentication:Audience"],
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
