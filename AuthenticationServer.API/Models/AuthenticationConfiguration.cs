namespace AuthenticationServer.API.Models
{
    public class AuthenticationConfiguration
    {
        public string? AccessTokenSecret { get; set; }
        public string? RefreshTokenSecret { get; set; }
        public double AccessTokenExpirationMinutes { get; set; }
        public double RefreshTokenExpirationMinutes { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        
    }
}
