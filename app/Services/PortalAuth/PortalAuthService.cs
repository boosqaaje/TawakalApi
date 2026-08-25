using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TawakalApi.app.Data;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Models.Entities;
using TawakalApi.app.Services.Util;
using TawakalApi.app.Utils.Constants;
using TawakalApi.app.Extensions;

namespace TawakalApi.app.Services.PortalAuth;

public class PortalAuthService(
    AppDbContext dbContext,
    IConfiguration configuration,
    IHttpContextAccessor contextAccessor
) : IPortalAuthService
{

    private readonly AppDbContext _dbContext = dbContext;
    private readonly IConfiguration _configuration = configuration;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    private static readonly PasswordHasher<string> _passwordHasher = new();


    // Get the partner username from the HTTP context claims
    private string? GetUserEmail()
    {
        return _contextAccessor.HttpContext?.User?.GetUserEmail();
    }

    public async Task<CommonRes> RegisterAsync(RegisterRequestDto? dto)
    {

        var validationResult = await ValidateRegistrationFields(dto);

        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Success = false,
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }

        var activeUser = await _dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == GetUserEmail());

        // Map all fields including the new name properties
        var user = new PortalUserEntity
        {
            FirstName = dto!.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = dto.Role,
            CreatedByUserId = activeUser!.Id
        };

        // Securely hash the password
        user.PasswordHash = _passwordHasher.HashPassword(user.Email, dto.Password);

        _dbContext.PortalUsers.Add(user);
        await _dbContext.SaveChangesAsync();

        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = SystemMessages.Auth.UserRegistered
        };
    }

    private async Task<DataResult<bool>> ValidateRegistrationFields(RegisterRequestDto? dto)
    {

        if (dto == null)
        {
            return DataResult<bool>.Error(
                SystemCodes.BodyNull,
                SystemMessages.BodyNull
                );
        }

        // Check if username already exists
        var existingUser = await _dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
        {
            return DataResult<bool>.Error(
                SystemCodes.Auth.EmailExist,
                SystemMessages.Auth.EmailExist
            );
        }

        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.FirstNameRequired
            );
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.LastNameRequired
            );
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.EmailRequired
            );
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.PasswordRequired
            );
        }

        if (string.IsNullOrWhiteSpace(dto.Role))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.RoleRequired
            );
        }


        return DataResult<bool>.Ok(true);
    }



    public async Task<CommonRes> LoginAsync(LoginRequestDto? dto)
    {
        // Validation
        var validationResult = await ValidateLoginFields(dto);
        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }


        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = SystemMessages.Auth.LoginSuccess,
            PortalAuthResponse = validationResult.Data
        };
    }

    // Validate Login
    private async Task<DataResult<PortalAuthResponseDto>> ValidateLoginFields(LoginRequestDto? dto)
    {
        if (dto is null) return DataResult<PortalAuthResponseDto>.Error(
            SystemCodes.BodyNull,
            SystemMessages.BodyNull
        );

        var user = await _dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null)
        {
            return DataResult<PortalAuthResponseDto>.Error(
                SystemCodes.Auth.InvalidCredentials,
                SystemMessages.Auth.InvalidCredentials
            );
        }

        // Verify password
        var result = _passwordHasher.VerifyHashedPassword(dto.Email, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return DataResult<PortalAuthResponseDto>.Error(
                SystemCodes.Auth.InvalidCredentials,
                SystemMessages.Auth.InvalidCredentials
            );
        }


        var token = GenerateJwtTokenForPortal(user);

        var authTokenRes = new PortalAuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };

        return DataResult<PortalAuthResponseDto>.Ok(authTokenRes);
    }

    private String GenerateJwtTokenForPortal(PortalUserEntity user)
    {
        var secretKey = Encoding.UTF8.GetBytes(_configuration["PortalJwt:Key"]!);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new("user_email", user.Email),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["PortalJwt:Issuer"],
            Audience = _configuration["PortalJwt:Audience"],
            SigningCredentials = credentials
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}