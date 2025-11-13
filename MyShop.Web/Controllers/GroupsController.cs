using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Web.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IGroupService groupService, ICategoryService categoryService, ILogger<GroupsController> logger)
        {
            _groupService = groupService;
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
                var result = await _groupService.GetGroupsPaginatedAsync(pageNumber, pageSize);
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
                _logger.LogError(ex, "Error fetching groups");
                TempData["Error"] = "Error fetching groups";
                return View(new List<GroupDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["Categories"] = categories;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGroupDto model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewData["Categories"] = categories;
                return View(model);
            }

            try
            {
                var group = await _groupService.CreateGroupAsync(model);
                TempData["Success"] = "Group created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group");
                ModelState.AddModelError("", "Error creating group");
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewData["Categories"] = categories;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var group = await _groupService.GetGroupByIdAsync(id);
                if (group == null)
                    return NotFound();

                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewData["Categories"] = categories;
                ViewData["GroupId"] = id;

                var updateDto = new UpdateGroupDto
                {
                    Name = group.Name,
                    Description = group.Description,
                    CategoryId = group.CategoryId
                };
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching group for edit");
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateGroupDto model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewData["Categories"] = categories;
                return View(model);
            }

            try
            {
                var group = await _groupService.UpdateGroupAsync(id, model);
                if (group == null)
                    return NotFound();

                TempData["Success"] = "Group updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group");
                ModelState.AddModelError("", "Error updating group");
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewData["Categories"] = categories;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var group = await _groupService.GetGroupByIdAsync(id);
                if (group == null)
                    return NotFound();

                return View(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching group for delete");
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
                var result = await _groupService.DeleteGroupAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Group deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group");
                TempData["Error"] = "Error deleting group";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
