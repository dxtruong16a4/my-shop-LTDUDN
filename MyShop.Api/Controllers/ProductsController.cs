using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Api.Controllers
{
    /// <summary>
    /// Products management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Gets a product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        /// <response code="200">Returns the product</response>
        /// <response code="404">If product not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Gets all products with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10, max 100)</param>
        /// <returns>Paginated list of products</returns>
        /// <response code="200">Returns paginated products</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _productService.GetProductsPaginatedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Gets products by group
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <returns>List of products in the group</returns>
        /// <response code="200">Returns products</response>
        [HttpGet("group/{groupId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ProductDto>), 200)]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            var products = await _productService.GetProductsByGroupAsync(groupId);
            return Ok(products);
        }

        /// <summary>
        /// Searches products by keyword with pagination
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10, max 100)</param>
        /// <returns>Paginated search results</returns>
        /// <response code="200">Returns search results</response>
        /// <response code="400">If keyword is empty</response>
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required" });

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var products = await _productService.SearchProductsAsync(keyword, pageNumber, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Creates a new product (Admin only)
        /// </summary>
        /// <param name="dto">Product data</param>
        /// <returns>Created product</returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">If data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Updates a product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="dto">Updated product data</param>
        /// <returns>Updated product</returns>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">If data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If product not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _productService.UpdateProductAsync(id, dto);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Deletes a product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Product deleted successfully</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not admin</response>
        /// <response code="404">If product not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
