using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Web.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IGroupService _groupService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, IGroupService groupService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _groupService = groupService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            try
            {
                var result = await _productService.GetProductsPaginatedAsync(pageNumber, pageSize);
                ViewData["PageNumber"] = result.PageNumber;
                ViewData["PageSize"] = result.PageSize;
                ViewData["TotalItems"] = result.TotalItems;
                ViewData["TotalPages"] = result.TotalPages;
                ViewData["HasPreviousPage"] = result.HasPreviousPage;
                ViewData["HasNextPage"] = result.HasNextPage;
                return View(result.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                TempData["Error"] = "Error fetching products";
                return View(new List<ProductDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            ViewData["Groups"] = groups;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                var groups = await _groupService.GetAllGroupsAsync();
                ViewData["Groups"] = groups;
                return View(model);
            }

            try
            {
                var product = await _productService.CreateProductAsync(model);
                TempData["Success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError("", "Error creating product");
                var groups = await _groupService.GetAllGroupsAsync();
                ViewData["Groups"] = groups;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                var groups = await _groupService.GetAllGroupsAsync();
                ViewData["Groups"] = groups;
                ViewData["ProductId"] = id;

                var updateDto = new UpdateProductDto
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    GroupId = product.GroupId,
                    Stock = product.Stock,
                    IsActive = product.IsActive
                };
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product for edit");
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                var groups = await _groupService.GetAllGroupsAsync();
                ViewData["Groups"] = groups;
                return View(model);
            }

            try
            {
                var product = await _productService.UpdateProductAsync(id, model);
                if (product == null)
                    return NotFound();

                TempData["Success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                ModelState.AddModelError("", "Error updating product");
                var groups = await _groupService.GetAllGroupsAsync();
                ViewData["Groups"] = groups;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product for delete");
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Product deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                TempData["Error"] = "Error deleting product";
                return RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    keyword = "";

                var result = await _productService.SearchProductsAsync(keyword, pageNumber, pageSize);
                ViewData["Keyword"] = keyword;
                ViewData["PageNumber"] = result.PageNumber;
                ViewData["PageSize"] = result.PageSize;
                ViewData["TotalItems"] = result.TotalItems;
                ViewData["TotalPages"] = result.TotalPages;
                ViewData["HasPreviousPage"] = result.HasPreviousPage;
                ViewData["HasNextPage"] = result.HasNextPage;
                return View(result.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                TempData["Error"] = "Error searching products";
                return View(new List<ProductDto>());
            }
        }
    }
}
