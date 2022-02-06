using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.API.Models
{
    public class AutheticationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {

        public AutheticationDbContext(DbContextOptions<AutheticationDbContext> options) : base(options)
        {

        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
