using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace OlxFilterWatcher.Services.Services;

public class TokenGeneratorService : ITokenGeneratorService
{
    private readonly string _tokenKey;
    private readonly int tokenHoursDuration;

    public TokenGeneratorService(IConfiguration configuration)
    {
        this._tokenKey = configuration.GetSection("JwtBearerKey").Value;
        this.tokenHoursDuration = int.Parse(configuration.GetSection("TokenHoursDuration").Value);
    }

    public JwtToken GenerateToken(UserAuth user = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenKey);
        var expiresAt = DateTime.UtcNow.AddHours(tokenHoursDuration);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        if (user != null)
            tokenDescriptor.Subject = new ClaimsIdentity(GetClaims(user.Roles));

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);

        return new JwtToken
        {
            Token = jwt,
            ExpiresAt = expiresAt
        };
    }

    private IEnumerable<Claim> GetClaims(IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            yield return new Claim(ClaimTypes.Role, role);
        }
    }
}