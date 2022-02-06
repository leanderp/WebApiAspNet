using AuthenticationServer.API.Models;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AuthenticationServer.API.Services.TokenGenerator
{
    public class AccessTokenGenerator
    {

        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGenerator _tokenGenerator;

        public AccessTokenGenerator(AuthenticationConfiguration configuration, TokenGenerator tokenGenerator)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            return _tokenGenerator.GenerateToken(
                secretKey: _configuration.AccessTokenSecret,
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                expirationMinutes: _configuration.AccessTokenExpirationMinutes,
                claims: claims);
        }
    }
}
