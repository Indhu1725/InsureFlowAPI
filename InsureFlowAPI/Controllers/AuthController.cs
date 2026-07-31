using InsureFlowAPI.DTOs.Authentication;
using InsureFlowAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Constructor
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Customer Registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterCustomerAsync(requestDto);

            return Created("", result);
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(requestDto);

            return Ok(result);
        }
    }
}