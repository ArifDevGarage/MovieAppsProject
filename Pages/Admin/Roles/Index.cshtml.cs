using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieAppsProject.Areas.Data;          // ApplicationDbContext
using MovieAppsProject.Models;             // ApplicationUser

namespace MovieAppsProject.Pages.Admin.Roles;

// [Authorize(Policy = "RequireAdmin")] // or [Authorize(Roles="Admin")]
[Authorize]
public class IndexModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public record RoleRow(string Id, string Name, int UserCount);
    public List<RoleRow> RoleRows { get; set; } = new();

    [BindProperty]
    public string NewRoleName { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var roles = _roleManager.Roles.ToList();
        RoleRows = new List<RoleRow>(roles.Count);

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            RoleRows.Add(new RoleRow(role.Id, role.Name!, usersInRole.Count));
        }
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (string.IsNullOrWhiteSpace(NewRoleName))
        {
            ModelState.AddModelError(string.Empty, "Role name is required.");
            await OnGetAsync();
            return Page();
        }

        if (await _roleManager.RoleExistsAsync(NewRoleName))
        {
            ModelState.AddModelError(string.Empty, "Role already exists.");
            await OnGetAsync();
            return Page();
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(NewRoleName.Trim()));
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            ModelState.AddModelError(string.Empty, "Role not found.");
            await OnGetAsync();
            return Page();
        }

        // prevent delete if users still in this role (safety)
        var users = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (users.Count > 0)
        {
            ModelState.AddModelError(string.Empty, $"Cannot delete '{role.Name}' because {users.Count} user(s) are assigned.");
            await OnGetAsync();
            return Page();
        }

        var res = await _roleManager.DeleteAsync(role);
        if (!res.Succeeded)
        {
            foreach (var e in res.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            await OnGetAsync();
            return Page();
        }

        return RedirectToPage();
    }
}