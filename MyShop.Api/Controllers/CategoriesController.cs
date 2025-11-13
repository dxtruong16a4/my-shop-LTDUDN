using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Api.Controllers
{
    /// <summary>
    /// Categories management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        /// <response code="200">Returns the category</response>
        /// <response code="404">If category not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CategoryDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Gets all categories with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10, max 100)</param>
        /// <returns>Paginated list of categories</returns>
        /// <response code="200">Returns paginated categories</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _categoryService.GetCategoriesPaginatedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Gets all root categories (no parent)
        /// </summary>
        /// <returns>List of root categories</returns>
        /// <response code="200">Returns root categories</response>
        [HttpGet("roots")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<CategoryDto>), 200)]
        public async Task<IActionResult> GetRoots()
        {
            var categories = await _categoryService.GetRootCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Gets subcategories of a parent category
        /// </summary>
        /// <param name="parentId">Parent category ID</param>
        /// <returns>List of subcategories</returns>
        /// <response code="200">Returns subcategories</response>
        [HttpGet("{parentId}/subcategories")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<CategoryDto>), 200)]
        public async Task<IActionResult> GetSubcategories(int parentId)
        {
            var categories = await _categoryService.GetSubCategoriesAsync(parentId);
            return Ok(categories);
        }

        /// <summary>
        /// Creates a new category (Admin only)
        /// </summary>
        /// <param name="dto">Category data</param>
        /// <returns>Created category</returns>
        /// <response code="201">Category created successfully</response>
        /// <response code="400">If data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CategoryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Updates a category (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="dto">Updated category data</param>
        /// <returns>Updated category</returns>
        /// <response code="200">Category updated successfully</response>
        /// <response code="400">If data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If category not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CategoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Deletes a category (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Category deleted successfully</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If category not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
