using Microsoft.AspNetCore.Mvc;
using DepartmentalSystemAPI.Services;
using DepartmentalSystemAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DepartmentalSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Change from AdLoginAsync to LoginAsync
                var authResponse = await _authService.LoginAsync(loginDto);
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Login failed. Please try again." });
            }
        }

        [HttpGet("windows-login")]
        public async Task<ActionResult<AuthResponseDto>> WindowsLogin()
        {
            try
            {
                var authResponse = await _authService.WindowsLoginAsync();
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Windows login failed. Please try again." });
            }
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync();
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}