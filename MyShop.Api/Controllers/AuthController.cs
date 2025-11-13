using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Helpers;
using MyShop.Core.Services;

namespace MyShop.Api.Controllers
{
    /// <summary>
    /// Authentication controller for user login and registration
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IUserService userService,
            IPasswordHelper passwordHelper,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _userService = userService;
            _passwordHelper = passwordHelper;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates user and returns JWT token
        /// </summary>
        /// <param name="dto">Login credentials (username and password)</param>
        /// <returns>JWT token and user information on successful authentication</returns>
        /// <response code="200">Returns the JWT token and user information</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If username/password is invalid or user is inactive</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userEntity = await _userService.GetUserEntityByUsernameAsync(dto.Username);
            if (userEntity == null || !_passwordHelper.VerifyPassword(dto.Password, userEntity.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password" });

            if (!userEntity.IsActive)
                return Unauthorized(new { message = "User account is inactive" });

            var token = _jwtService.GenerateToken(
                userEntity,
                _configuration["Jwt:Secret"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60")
            );

            var userDto = await _userService.GetUserByUsernameAsync(dto.Username);

            return Ok(new
            {
                message = "Login successful",
                token = token,
                user = userDto
            });
        }

        /// <summary>
        /// Registers a new user account
        /// </summary>
        /// <param name="dto">User registration data</param>
        /// <returns>Created user information</returns>
        /// <response code="201">User registered successfully</response>
        /// <response code="400">If username already exists or data is invalid</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userService.GetUserByUsernameAsync(dto.Username);
            if (existingUser != null)
                return BadRequest(new { message = "Username already exists" });

            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new
            {
                message = "User registered successfully",
                user = user
            });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

