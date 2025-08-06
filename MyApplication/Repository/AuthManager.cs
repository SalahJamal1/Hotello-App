using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyApplication.Contracts;
using MyApplication.Data;
using MyApplication.Models.Users;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace MyApplication.Repository;

public class AuthManager : IAuthManager
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;

    public AuthManager(IConfiguration configuration, UserManager<ApiUser> userManager,
        IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _userManager = userManager;

        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext _httpContext => _httpContextAccessor.HttpContext;

    public async Task<AuthResponse> Login(AuthLogin authLogin)
    {
        var user = await _userManager.FindByEmailAsync(authLogin.Email);
        var isValid = await _userManager.CheckPasswordAsync(user, authLogin.Password);
        if (!isValid) return null;
        var token = await GenerateToken(user);
        setCookie(token);
        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task Logout()
    {
        _httpContext.Response.Cookies.Delete("jwt");
    }

    public async Task<UserDto> GetUser()
    {
        var userId = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
        var user = await _userManager.FindByIdAsync(userId);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto apiUserDto)
    {
        var user = _mapper.Map<ApiUser>(apiUserDto);
        user.UserName = apiUserDto.Email;
        var result = await _userManager.CreateAsync(user, apiUserDto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            var token = await GenerateToken(user);
            setCookie(token);
        }


        return result.Errors;
    }

    public async Task<string> GenerateToken(ApiUser user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var roles = await _userManager.GetRolesAsync(user);
        var rolesClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
        var userClaims = await _userManager.GetClaimsAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("uid", user.Id)
        }.Union(rolesClaims).Union(userClaims);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(90),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void setCookie(string token)
    {
        var cookieOption = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(90),
            Secure = true,
            SameSite = SameSiteMode.None
        };
        _httpContext.Response.Cookies.Append("jwt", token, cookieOption);
    }
}