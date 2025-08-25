using AiTesting.Application.Users.Dto.Auth;
using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Domain.Services.User;
using AiTesting.Application.Users.Services.RefreshToken;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AiTesting.Application.Users.Utils;

namespace AiTesting.Application.Users.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserService userService,
        IConfiguration config,
        IPasswordHasher passwordHasher,
        IRefreshTokenService refreshTokenService)
    {
        _userService = userService;
        _config = config;
        _passwordHasher = passwordHasher;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<AuthResultDto>> Register(RegisterUserDto dto)
    {
        var passwordHash = _passwordHasher.HashPassword(dto.Password);
        var userResult = User.CreateDefault(dto.Email, passwordHash, dto.Name);
        if (userResult.IsFailure) return Result<AuthResultDto>.Failure(userResult.Error);

        var user = userResult.Value;
        var addingResult = await _userService.AddAsync(user);
        if (addingResult.IsFailure) return Result<AuthResultDto>.Failure(addingResult.Error);

        return await GenerateTokens(user);
    }

    public async Task<Result<AuthResultDto>> Login(LoginDto dto)
    {
        var userResult = await _userService.GetByEmailAsync(dto.Email);
        if (!userResult.IsSuccess) return Result<AuthResultDto>.Failure("User not found");

        var user = userResult.Value;
        if (!_passwordHasher.VerifyPassword(user.PasswordHash, dto.Password))
            return Result<AuthResultDto>.Failure("Invalid credentials");

        var tokenResult = await _refreshTokenService.GetByUserIdAsync(user.Id);

        return await GenerateTokens(user, tokenResult.IsSuccess ? tokenResult.Value : null);
    }

    public async Task<Result<AuthResultDto>> Refresh(RefreshTokenDto dto)
    {
        var tokenResult = await _refreshTokenService.GetByTokenAsync(dto.RefreshToken);
        if (!tokenResult.IsSuccess) return Result<AuthResultDto>.Failure("Invalid or expired refresh token");

        var storedToken = tokenResult.Value;
        if (storedToken.ExpiresAt < DateTime.UtcNow)
            return Result<AuthResultDto>.Failure("Refresh token expired");

        var userResult = await _userService.GetByIdAsync(storedToken.UserId);
        if (!userResult.IsSuccess) return Result<AuthResultDto>.Failure("User not found");

        var user = userResult.Value;
        return await GenerateTokens(user, storedToken);
    }

    private async Task<Result<AuthResultDto>> GenerateTokens(User user, Infrastructure.Persistence.Entities.RefreshToken? existingRefreshToken = null)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = existingRefreshToken ?? Infrastructure.Persistence.Entities.RefreshToken.Create(user);

        if (refreshToken.IsExpired()) refreshToken.Refresh();

        await _refreshTokenService.SaveAsync(refreshToken);

        var resultDto = new AuthResultDto(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            UserId: user.Id,
            DisplayName: user.DisplayName,
            Email: user.Email
        );

        return Result<AuthResultDto>.Success(resultDto);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (JwtRegisteredClaimNames.Name, user.DisplayName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiresInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
