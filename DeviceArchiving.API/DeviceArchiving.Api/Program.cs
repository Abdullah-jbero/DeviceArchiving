using DeviceArchiving.Data;
using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Dto;
using DeviceArchiving.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// 🔧 Configuration
var configuration = builder.Configuration;
var services = builder.Services;
// 🔌 Add Services
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
// 🧩 Dependency Injection
services.AddScoped<IDeviceService, DeviceService>();
services.AddScoped<IOperationService, OperationService>();
services.AddScoped<IOperationTypeService, OperationTypeService>();
services.AddScoped<IAccountService, AccountService>();
// 🗄️ Database (SQLite or SQL Server based on configuration)
services.AddHttpContextAccessor();
services.AddSingleton<UserIdInterceptor>();

services.AddDbContextFactory<DeviceArchivingContext>((serviceProvider, options) =>
{
    var userIdInterceptor = serviceProvider.GetRequiredService<UserIdInterceptor>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    options.UseSqlServer(connectionString);
    options.AddInterceptors(userIdInterceptor);
});


// Bind JwtSettings
services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JWTSettings"));
// JWT Setup
var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JwtSettings>();
// 🔐 JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
// Read allowed origins from configuration
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("x-pagination", "Authorization");
    });
});
var app = builder.Build();

app.UseMiddleware<DbExceptionMiddleware>();

app.UseCors("CorsPolicy");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
