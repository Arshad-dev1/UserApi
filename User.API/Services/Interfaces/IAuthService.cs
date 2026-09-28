using UserApi.Models;

namespace UserApi.Services.Interfaces;

public interface IAuthService
{
    LoginResponse? Login(string username, string password);
}
