namespace UserApi.Models;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
