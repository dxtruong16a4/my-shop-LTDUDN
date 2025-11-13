using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Web.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            try
            {
                var result = await _categoryService.GetCategoriesPaginatedAsync(pageNumber, pageSize);
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
                _logger.LogError(ex, "Error fetching categories");
                TempData["Error"] = "Error fetching categories";
                return View(new List<CategoryDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var roots = await _categoryService.GetRootCategoriesAsync();
            ViewData["ParentCategories"] = roots;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                var roots = await _categoryService.GetRootCategoriesAsync();
                ViewData["ParentCategories"] = roots;
                return View(model);
            }

            try
            {
                var category = await _categoryService.CreateCategoryAsync(model);
                TempData["Success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                ModelState.AddModelError("", "Error creating category");
                var roots = await _categoryService.GetRootCategoriesAsync();
                ViewData["ParentCategories"] = roots;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound();

                var roots = await _categoryService.GetRootCategoriesAsync();
                ViewData["ParentCategories"] = roots;
                ViewData["CategoryId"] = id;

                var updateDto = new UpdateCategoryDto
                {
                    Name = category.Name,
                    ParentId = category.ParentId,
                    OrderIndex = category.OrderIndex
                };
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category for edit");
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                var roots = await _categoryService.GetRootCategoriesAsync();
                ViewData["ParentCategories"] = roots;
                return View(model);
            }

            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, model);
                if (category == null)
                    return NotFound();

                TempData["Success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                ModelState.AddModelError("", "Error updating category");
                var roots = await _categoryService.GetRootCategoriesAsync();
                ViewData["ParentCategories"] = roots;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound();

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category for delete");
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
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Category deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                TempData["Error"] = "Error deleting category";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
