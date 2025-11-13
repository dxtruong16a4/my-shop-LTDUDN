using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Core.DTOs;
using MyShop.Core.Services;

namespace MyShop.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            try
            {
                var result = await _userService.GetUsersPaginatedAsync(pageNumber, pageSize);
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
                _logger.LogError(ex, "Error fetching users");
                TempData["Error"] = "Error fetching users";
                return View(new List<UserDto>());
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.CreateUserAsync(model);
                TempData["Success"] = "User created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                ModelState.AddModelError("", "Error creating user");
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                var updateDto = new UpdateUserDto
                {
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
                ViewData["UserId"] = id;
                ViewData["Username"] = user.Username;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user for edit");
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.UpdateUserAsync(id, model);
                if (user == null)
                    return NotFound();

                TempData["Success"] = "User updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                ModelState.AddModelError("", "Error updating user");
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user for delete");
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "User deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                TempData["Error"] = "Error deleting user";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
