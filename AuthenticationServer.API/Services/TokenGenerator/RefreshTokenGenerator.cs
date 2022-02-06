using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.TokenGenerator
{
    public class RefreshTokenGenerator
    {

        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGenerator _tokenGenerator;

        public RefreshTokenGenerator(AuthenticationConfiguration configuration, TokenGenerator tokenGenerator)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
        }

        public string GenerateToken()
        {
            return _tokenGenerator.GenerateToken(
                secretKey: _configuration.RefreshTokenSecret,
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                expirationMinutes: _configuration.RefreshTokenExpirationMinutes,
                claims: null);
        }
    }
}
