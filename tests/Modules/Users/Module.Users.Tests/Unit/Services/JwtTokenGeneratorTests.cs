using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces;
using Module.Users.Infrastructure.Authentication;
using Xunit;

namespace Module.Users.Tests.Unit.Services;

public class JwtTokenGeneratorTests
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;

    public JwtTokenGeneratorTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "SuperSecretKey123456789012345678901234567890" }, // 32+ chars
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _jwtTokenGenerator = new JwtTokenGenerator(_configuration);
    }

    [Fact]
    public void GenerateToken_Should_Throw_Exception_When_JwtKey_Is_Missing()
    {
        var invalidConfig = new ConfigurationBuilder().Build(); // Sem chave JWT
        var jwtGenerator = new JwtTokenGenerator(invalidConfig);
        var user = FakeUser.CreateFakeUser();

        Assert.Throws<ArgumentNullException>(() => jwtGenerator.GenerateToken(user));
    }

    [Fact]
    public void GenerateToken_Should_Return_Valid_Token()
    {
        var user = FakeUser.CreateFakeUser();

        var response = _jwtTokenGenerator.GenerateToken(user);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Token));
        Assert.True(response.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_Should_Contain_Correct_User_Claims()
    {
        var user = FakeUser.CreateFakeUser();

        var response = _jwtTokenGenerator.GenerateToken(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        var principal = tokenHandler.ValidateToken(response.Token, validationParameters, out var validatedToken);

        Assert.NotNull(validatedToken);
        Assert.NotNull(principal);
        Assert.Equal(user.Name.Firstname, principal.FindFirst(ClaimTypes.Name)?.Value);
        Assert.Equal(user.Role.ToString(), principal.FindFirst(ClaimTypes.Role)?.Value);
    }
}