using DepartmentalSystemAPI.Models;

namespace DepartmentalSystemAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
        bool ValidateToken(string token);
    }
}