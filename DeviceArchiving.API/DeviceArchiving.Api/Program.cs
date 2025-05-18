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
services.AddScoped<IAccountService, AccountService>(); // Optional, in case you missed it

// 🌐 CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 🗄️ Database
services.AddDbContext<DeviceArchivingContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


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


var app = builder.Build();

// ⚙️ Middleware
app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
