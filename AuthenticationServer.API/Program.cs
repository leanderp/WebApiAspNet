using AuthenticationServer.API.Models;
using AuthenticationServer.API.Services.Authenticator;
using AuthenticationServer.API.Services.RefreshTokenRepository;
using AuthenticationServer.API.Services.TokenGenerator;
using AuthenticationServer.API.Services.TokenValidator;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddIdentityCore<User>(o =>
{

    o.User.RequireUniqueEmail = true;

    o.Password.RequireDigit = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.Password.RequiredLength = 0;
}).AddEntityFrameworkStores<AutheticationDbContext>();

AuthenticationConfiguration authenticationConfiguration = new AuthenticationConfiguration();
builder.Configuration.Bind("Authentication", authenticationConfiguration);
builder.Services.AddSingleton(authenticationConfiguration);

SecretClient keyVaultClient = new SecretClient(
    new Uri(builder.Configuration.GetValue<string>("KeyVaultUri")),
    new DefaultAzureCredential());
authenticationConfiguration.AccessTokenSecret = keyVaultClient.GetSecret("access-token-secret").Value.Value;
authenticationConfiguration.RefreshTokenSecret = keyVaultClient.GetSecret("refresh-token-secret").Value.Value;

string connectionString = builder.Configuration.GetConnectionString("postgresql");
builder.Services.AddDbContext<AutheticationDbContext>(options =>
            options.UseNpgsql(connectionString));

builder.Services.AddScoped<IRefreshTokenRepository, DataBaseRefreshTokenRepository>();
builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddScoped<Authenticator>();
builder.Services.AddSingleton<AccessTokenGenerator>();
builder.Services.AddSingleton<RefreshTokenGenerator>();
builder.Services.AddSingleton<RefreshTokenValidator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = authenticationConfiguration.Issuer,
        ValidAudience = authenticationConfiguration.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Auth Server Asp .Net 6",
                    Version = "v1",
                    Description = "Server authentication JWT",
                    TermsOfService = new Uri("https://github.com/leanderp/"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Leander Perez",
                        Email = "leanderperez15@gmail.com",
                        Url = new Uri("https://github.com/leanderp/")
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Add migration db
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
using var context = scope.ServiceProvider.GetRequiredService<AutheticationDbContext>();
context.Database.Migrate();

app.MapControllers();

app.Run();
