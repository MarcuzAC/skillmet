using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> WindowsLoginAsync();
        Task<UserDto> GetCurrentUserAsync();
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}