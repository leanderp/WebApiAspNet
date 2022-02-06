using AuthenticationServer.API.Models;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Services.RefreshTokenRepository;
using AuthenticationServer.API.Services.TokenGenerator;


namespace AuthenticationServer.API.Services.Authenticator
{
    public class Authenticator
    {

        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AccessTokenGenerator _accessTokenGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;

        public Authenticator(IRefreshTokenRepository refreshTokenRepository, AccessTokenGenerator accessTokenGenerator , RefreshTokenGenerator refreshTokenGenerator)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<AuthenticatedUserResponse> Autenticate(User user)
        {
            string accesToken = _accessTokenGenerator.GenerateToken(user);
            string refreshToken = _refreshTokenGenerator.GenerateToken();

            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };

            await _refreshTokenRepository.Create(refreshTokenDTO);

            return new AuthenticatedUserResponse()
            {
                AccessToken = accesToken,
                RefreshToken = refreshToken
            };
        }
    }
}
