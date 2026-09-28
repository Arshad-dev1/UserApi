using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UserApi.Controllers;
using UserApi.Models;
using UserApi.Services;
using UserApi.Services.Interfaces;

namespace UserApiTests;

[TestClass]
public sealed class AuthControllerTests
{
    private static AuthController CreateController() => new(new AuthService(new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Authentication:Username"] = "admin",
            ["Authentication:Password"] = "correct horse battery staple",
            ["Authentication:SigningKey"] = "test-signing-key-must-be-at-least-32-bytes-long",
            ["Authentication:Issuer"] = "test-issuer",
            ["Authentication:Audience"] = "test-audience",
            ["Authentication:Role"] = "Administrator",
            ["Authentication:TokenLifetimeMinutes"] = "60"
        }).Build()));

    [TestMethod]
    public void Login_ValidCredentials_ReturnsSignedToken()
    {
        var result = CreateController().Login(new LoginRequest
        {
            Username = "admin",
            Password = "correct horse battery staple"
        });

        var response = result.Result as OkObjectResult;
        Assert.IsNotNull(response);
        var token = response.Value as LoginResponse;
        Assert.IsNotNull(token);
        Assert.IsGreaterThan(20, token.AccessToken.Length);
        Assert.IsGreaterThan(DateTime.UtcNow, token.ExpiresAtUtc);
    }

    [TestMethod]
    public void Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var result = CreateController().Login(new LoginRequest
        {
            Username = "admin",
            Password = "wrong"
        });

        Assert.IsInstanceOfType<UnauthorizedResult>(result.Result);
    }
}
