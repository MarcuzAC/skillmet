using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Services
{
    public interface IAdService
    {
        Task<AdUserInfo?> AuthenticateAsync(string username, string password);
        Task<AdUserInfo?> GetUserInfoAsync(string username);
        Task<List<AdUserInfo>> GetAllUsersAsync();
    }

    public class AdUserInfo
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string[] Groups { get; set; } = Array.Empty<string>();
        public bool IsActive { get; set; } = true;
    }
}