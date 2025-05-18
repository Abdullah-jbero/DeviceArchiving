using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeviceArchiving.Service
{
    public class AccountService : IAccountService
    {
        private readonly DeviceArchivingContext _context;
        private readonly JwtSettings _jwtSettings;

        public AccountService(DeviceArchivingContext context, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<BaseResponse<string>> AuthenticateAsync(AuthenticationRequest request)
        {
            try
            {
                var email = request.Email.Trim().ToLower();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);

                if (user == null)
                    return BaseResponse<string>.Failure($"User with email '{request.Email}' not found.");

                if (!VerifyPassword(request.Password, user.Password))
                    return BaseResponse<string>.Failure("Invalid credentials.");

                var token = GenerateJwtToken(user);
                return BaseResponse<string>.SuccessResult(new JwtSecurityTokenHandler().WriteToken(token), "Authenticated successfully.");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.Failure($"Authentication error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<string>> AddUserAsync(AuthenticationRequest request)
        {
            try
            {
                var email = request.Email.Trim().ToLower();
                var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email);

                if (exists)
                    return BaseResponse<string>.Failure($"Email '{request.Email}' is already in use.");

                var newUser = new User
                {
                    Email = email,
                    Password = HashPassword(request.Password)
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return BaseResponse<string>.SuccessResult("User created successfully.", "Registration completed.");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.Failure($"Registration error: {ex.Message}");
            }
        }

        private JwtSecurityToken GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        private bool VerifyPassword(string input, string hashed) => BCrypt.Net.BCrypt.Verify(input, hashed);

    
    }
}
