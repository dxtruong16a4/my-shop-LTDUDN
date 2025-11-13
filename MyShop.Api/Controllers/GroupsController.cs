using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Api.Controllers
{
    /// <summary>
    /// Product groups management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Gets a group by ID
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <returns>Group details</returns>
        /// <response code="200">Returns the group</response>
        /// <response code="404">If group not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GroupDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound();

            return Ok(group);
        }

        /// <summary>
        /// Gets all groups with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10, max 100)</param>
        /// <returns>Paginated list of groups</returns>
        /// <response code="200">Returns paginated groups</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _groupService.GetGroupsPaginatedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Gets groups by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of groups in the category</returns>
        /// <response code="200">Returns groups</response>
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<GroupDto>), 200)]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var groups = await _groupService.GetGroupsByCategoryAsync(categoryId);
            return Ok(groups);
        }

        /// <summary>
        /// Creates a new group (Admin only)
        /// </summary>
        /// <param name="dto">Group data</param>
        /// <returns>Created group</returns>
        /// <response code="201">Group created successfully</response>
        /// <response code="400">If data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GroupDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create([FromBody] CreateGroupDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var group = await _groupService.CreateGroupAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }

        /// <summary>
        /// Updates a group (Admin only)
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="dto">Updated group data</param>
        /// <returns>Updated group</returns>
        /// <response code="200">Group updated successfully</response>
        /// <response code="400">If data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If group not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GroupDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGroupDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var group = await _groupService.UpdateGroupAsync(id, dto);
            if (group == null)
                return NotFound();

            return Ok(group);
        }

        /// <summary>
        /// Deletes a group (Admin only)
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Group deleted successfully</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If group not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _groupService.DeleteGroupAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
