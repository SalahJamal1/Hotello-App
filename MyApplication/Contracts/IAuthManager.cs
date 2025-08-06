using Microsoft.AspNetCore.Identity;
using MyApplication.Models.Users;

namespace MyApplication.Contracts;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(ApiUserDto user);
    Task<AuthResponse> Login(AuthLogin user);
    Task Logout();
    Task<UserDto> GetUser();
}