using InsureFlowAPI.DTOs.Authentication;
using InsureFlowAPI.DTOs.User;

namespace InsureFlowAPI.Services.Interfaces
{
    public interface IAuthService
    {
            Task<UserResponseDto> RegisterCustomerAsync(RegisterRequestDto dto);
            Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        
    }

}