using InsureFlowAPI.DTOs.Authentication;
using InsureFlowAPI.DTOs.User;
using InsureFlowAPI.Models;
using InsureFlowAPI.Models.Enums;
using InsureFlowAPI.Repositories.Interfaces;
using InsureFlowAPI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InsureFlowAPI.Exceptions;

namespace InsureFlowAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ICloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<UserResponseDto> RegisterCustomerAsync(RegisterRequestDto requestDto)
        {
            var email = requestDto.Email.Trim().ToLower();

            if (await _userRepository.EmailExistsAsync(email))
                throw new ConflictException("User with this email already exists.");

            // Upload image to Cloudinary
            string? imageUrl = null;

            if (requestDto.ProfileImage != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(requestDto.ProfileImage);
            }

            var user = new User
            {
                FullName = requestDto.FullName.Trim(),
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(requestDto.Password),
                MobileNumber = requestDto.MobileNumber.Trim(),
                Role = Role.Customer,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,

                // Save Cloudinary URL
                ProfileImageUrl = imageUrl
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto requestDto)
        {
            var email = requestDto.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new NotFoundException("Invalid email or password.");

            if (!user.IsActive)
                throw new BadRequestException("This user account is inactive.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(requestDto.Password, user.Password);

            if (!isPasswordValid)
                throw new NotFoundException("Invalid email or password.");

            return GenerateToken(user);
        }

        private LoginResponseDto GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("JWT");

            var secretKey = jwtSection["SecretKey"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiryMinutes = jwtSection["ExpiryMinutes"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new BadRequestException("JWT SecretKey is missing in configuration.");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new BadRequestException("JWT Issuer is missing in configuration.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new BadRequestException("JWT Audience is missing in configuration.");

            if (!int.TryParse(expiryMinutes, out int tokenExpiryMinutes))
                tokenExpiryMinutes = 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);

            var claims = new List<System.Security.Claims.Claim>
           {
             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.FullName),
             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString())
           };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                JWTToken = jwtToken,
                TokenType = "Bearer",
                UserEmail = user.Email,
                UserRole = user.Role.ToString(),
                TokenExpiry= expiresAt
            };
        }
    }
}