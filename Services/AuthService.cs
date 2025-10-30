using Microsoft.EntityFrameworkCore;
using DepartmentalSystemAPI.Data;
using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Principal;

namespace DepartmentalSystemAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly DepartmentContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IAdService _adService;

        public AuthService(
            DepartmentContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger,
            ITokenService tokenService,
            IAdService adService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _adService = adService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("AD login attempt for user: {Username}", loginDto.Username);

                // Validate input
                if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    throw new UnauthorizedAccessException("Username and password are required.");
                }

                // Authenticate against Active Directory
                var adUser = await _adService.AuthenticateAsync(loginDto.Username, loginDto.Password);

                if (adUser == null || !adUser.IsActive)
                {
                    _logger.LogWarning("AD authentication failed or user inactive: {Username}", loginDto.Username);
                    throw new UnauthorizedAccessException("Invalid credentials or inactive account.");
                }

                // Find or create user in local database
                var user = await FindOrCreateUserFromAdAsync(adUser);

                // Update last login
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successful AD login for user {Username}", user.Username);

                // Generate JWT token
                var token = _tokenService.CreateToken(user);

                return new AuthResponseDto
                {
                    User = MapToUserDto(user),
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24) // Token expiry
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AD login for user: {Username}", loginDto.Username);
                throw;
            }
        }

        public async Task<AuthResponseDto> WindowsLoginAsync()
        {
            try
            {
                var windowsIdentity = GetWindowsIdentity();
                if (windowsIdentity == null)
                {
                    _logger.LogWarning("Windows authentication required but not provided.");
                    throw new UnauthorizedAccessException("Windows authentication required.");
                }

                var (domain, login) = ParseWindowsIdentity(windowsIdentity);
                _logger.LogInformation("Windows auto-login for {Domain}\\{Login}", domain, login);

                // Get user info from AD
                var adUser = await _adService.GetUserInfoAsync(login);
                if (adUser == null || !adUser.IsActive)
                {
                    throw new UnauthorizedAccessException("User not found in Active Directory or account inactive.");
                }

                // Find or create user
                var user = await FindOrCreateUserFromAdAsync(adUser);

                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successful Windows auto-login for user {Username}", user.Username);

                var token = _tokenService.CreateToken(user);

                return new AuthResponseDto
                {
                    User = MapToUserDto(user),
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Windows auto-login");
                throw;
            }
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId.Value && u.IsActive);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            return MapToUserDto(user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            _logger.LogInformation("Password change requested for user {UserId}", userId);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            // For AD users, password changes should be done in Active Directory
            _logger.LogWarning("Password change attempted for AD user {Username} - should be done in Active Directory", user.Username);
            throw new InvalidOperationException("Password changes must be performed in Active Directory.");
        }

        private async Task<User> FindOrCreateUserFromAdAsync(AdUserInfo adUser)
        {
            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username == adUser.Username);

            if (user == null)
            {
                user = await CreateUserFromAdAsync(adUser);
            }
            else
            {
                await UpdateUserFromAdAsync(user, adUser);
            }

            return user;
        }

        private async Task<User> CreateUserFromAdAsync(AdUserInfo adUser)
        {
            var user = new User
            {
                Username = adUser.Username,
                Email = !string.IsNullOrEmpty(adUser.Email) ? adUser.Email : $"{adUser.Username}@company.com",
                PasswordHash = "AD_AUTHENTICATED", // No local password storage for AD users
                Role = DetermineUserRole(adUser.Groups),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastLogin = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create employee record
            var employee = new Employee
            {
                FirstName = !string.IsNullOrEmpty(adUser.FirstName) ? adUser.FirstName : adUser.Username,
                LastName = !string.IsNullOrEmpty(adUser.LastName) ? adUser.LastName : "User",
                Email = user.Email,
                Position = !string.IsNullOrEmpty(adUser.Title) ? adUser.Title : "Employee",
                Department = !string.IsNullOrEmpty(adUser.Department) ? adUser.Department : "Not assigned",
                HireDate = DateTime.UtcNow.Date,
                Status = AvailabilityStatus.Available,
                UserId = user.Id
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return await _context.Users
                .Include(u => u.Employee)
                .FirstAsync(u => u.Id == user.Id);
        }

        private async Task UpdateUserFromAdAsync(User user, AdUserInfo adUser)
        {
            if (user.Employee != null)
            {
                user.Employee.FirstName = !string.IsNullOrEmpty(adUser.FirstName) ? adUser.FirstName : user.Employee.FirstName;
                user.Employee.LastName = !string.IsNullOrEmpty(adUser.LastName) ? adUser.LastName : user.Employee.LastName;
                user.Employee.Email = !string.IsNullOrEmpty(adUser.Email) ? adUser.Email : user.Employee.Email;
                user.Employee.Position = !string.IsNullOrEmpty(adUser.Title) ? adUser.Title : user.Employee.Position;
                user.Employee.Department = !string.IsNullOrEmpty(adUser.Department) ? adUser.Department : user.Employee.Department;

                // Update role if needed
                var newRole = DetermineUserRole(adUser.Groups);
                if (user.Role != newRole)
                {
                    user.Role = newRole;
                }
            }

            await _context.SaveChangesAsync();
        }

        private UserRole DetermineUserRole(string[] groups)
        {
            var adminGroups = new[] { "domain admins", "enterprise admins", "administrators", "it admin", "system admin" };
            var userGroups = groups.Select(g => g.ToLower()).ToArray();

            if (userGroups.Any(g => adminGroups.Any(ag => g.Contains(ag))))
            {
                return UserRole.Admin;
            }

            return UserRole.Employee;
        }

        private WindowsIdentity? GetWindowsIdentity()
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity as WindowsIdentity;
            return identity == null || !identity.IsAuthenticated ? null : identity;
        }

        private (string Domain, string Login) ParseWindowsIdentity(WindowsIdentity identity)
        {
            var nameParts = identity.Name.Split('\\');
            if (nameParts.Length == 2)
            {
                return (nameParts[0], nameParts[1]);
            }

            if (identity.Name.Contains('@'))
            {
                var atParts = identity.Name.Split('@');
                return (atParts[1], atParts[0]);
            }

            return ("UNKNOWN", identity.Name);
        }

        private int? GetCurrentUserIdFromToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst("userId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                var nameClaim = httpContext.User.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Username == nameClaim.Value);
                    if (user != null)
                    {
                        return user.Id;
                    }
                }
            }
            return null;
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                FirstName = user.Employee?.FirstName ?? "Unknown",
                LastName = user.Employee?.LastName ?? "User",
                Position = user.Employee?.Position ?? "Employee",
                Department = user.Employee?.Department ?? "Not assigned"
            };
        }
    }
}