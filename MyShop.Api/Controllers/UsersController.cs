using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Api.Controllers
{
    /// <summary>
    /// Users management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">User ID (GUID)</param>
        /// <returns>User details</returns>
        /// <response code="200">Returns the user</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="404">If user not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Gets all users with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10, max 100)</param>
        /// <returns>Paginated list of users</returns>
        /// <response code="200">Returns paginated users</response>
        /// <response code="401">If user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _userService.GetUsersPaginatedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Updates a user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="dto">Updated user data</param>
        /// <returns>Updated user</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">If data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If user not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.UpdateUserAsync(id, dto);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Deletes a user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">User deleted successfully</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If user not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
