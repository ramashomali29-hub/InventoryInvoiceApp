using InventoryInvoiceApp.Data;
using InventoryInvoiceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryInvoiceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public AdminController(
            UserManager<AppUser> userManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users =
                await _userManager.Users
                    .AsNoTracking()
                    .ToListAsync();

            var summaries =
                new List<UserSummaryViewModel>();

            foreach (var user in users)
            {
                var roles =
                    await _userManager.GetRolesAsync(user);

                summaries.Add(
                    new UserSummaryViewModel
                    {
                        Username =
                            user.UserName ?? "",

                        Email =
                            user.Email ?? "",

                        Role =
                            roles.FirstOrDefault() ??
                            "No Role"
                    });
            }

            var model =
                new AdminDashboardViewModel
                {
                    Users = summaries
                };

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(
            CreateUserViewModel model)
        {
            string[] allowedRoles =
            {
                "Warehouse",
                "Cashier"
            };

            if (!allowedRoles.Contains(model.Role))
            {
                ModelState.AddModelError(
                    nameof(model.Role),
                    "Select Warehouse or Cashier.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(
                    nameof(model.ConfirmPassword),
                    "Passwords do not match.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email =
                model.Email.Trim().ToLower();

            var existingEmail =
                await _userManager.FindByEmailAsync(email);

            if (existingEmail != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "This email is already registered.");

                return View(model);
            }

            var existingUsername =
                await _userManager.FindByNameAsync(
                    model.Username.Trim());

            if (existingUsername != null)
            {
                ModelState.AddModelError(
                    nameof(model.Username),
                    "This username is already used.");

                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Username.Trim(),
                Email = email,
                EmailConfirmed = true
            };

            var result =
                await _userManager.CreateAsync(
                    user,
                    model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description);
                }

                return View(model);
            }

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    model.Role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                ModelState.AddModelError(
                    "",
                    "Could not assign the selected role.");

                return View(model);
            }

            TempData["Success"] =
                "User created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> StockMovements()
        {
            var movements =
                await _context.StockMovements
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

            return View(movements);
        }
    }
}