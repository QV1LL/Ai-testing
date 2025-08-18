using AiTesting.Application.Users.Dto.Auth;
using AiTesting.Application.Users.Utils;
using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Domain.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AiTesting.Application.Users.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserService userService, 
                       IConfiguration config, 
                       IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
        _userService = userService;
        _config = config;
    }

    public async Task<Result<LoginResultDto>> Register(RegisterUserDto dto)
    {
        var passwordHash = _passwordHasher.HashPassword(dto.Password);

        var userResult = User.CreateDefault(email: dto.Email,
                                      passwordHash: passwordHash,
                                      displayName: dto.Name);

        if (userResult.IsFailure) return Result<LoginResultDto>.Failure(userResult.Error);

        var user = userResult.Value;
        var addingResult = await _userService.AddAsync(user);

        if (addingResult.IsFailure) return Result<LoginResultDto>.Failure(addingResult.Error);

        var token = GenerateJwtToken(user);
        var resultDto = new LoginResultDto(
            Token: token,
            UserId: user.Id,
            DisplayName: user.DisplayName,
            Email: user.Email
        );

        return Result<LoginResultDto>.Success(resultDto);
    }

    public async Task<Result<LoginResultDto>> Login(LoginDto dto)
    {
        var userResult = await _userService.GetByEmailAsync(dto.Email);

        if (!userResult.IsSuccess)
            return Result<LoginResultDto>.Failure("User not found");

        var user = userResult.Value;

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, dto.Password))
            return Result<LoginResultDto>.Failure("Invalid credentials");

        var token = GenerateJwtToken(user);

        var resultDto = new LoginResultDto(
            Token: token,
            UserId: user.Id,
            DisplayName: user.DisplayName,
            Email: user.Email
        );

        return Result<LoginResultDto>.Success(resultDto);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["JwtSettings:ExpiresInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
