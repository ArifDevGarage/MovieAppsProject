using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieAppsProject.Areas.Data;          // ApplicationDbContext
using MovieAppsProject.Models;             // ApplicationUser


namespace MovieAppsProject.Pages.Admin.Users;

// [Authorize(Policy = "RequireAdmin")] // or [Authorize(Roles="Admin")]
[Authorize] // or [Authorize(Roles="Admin")]
public class EditRolesModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EditRolesModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public string? UserId { get; set; }
    public string? Email { get; set; }

    public class RoleSelection
    {
        public string RoleName { get; set; } = default!;
        public bool IsSelected { get; set; }
    }

    [BindProperty]
    public List<RoleSelection> Roles { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        UserId = user.Id;
        Email = user.Email;

        var allRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
        var userRoles = await _userManager.GetRolesAsync(user);

        Roles = allRoles
            .OrderBy(r => r)
            .Select(r => new RoleSelection { RoleName = r, IsSelected = userRoles.Contains(r) })
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // normalize posted roles
        var selected = Roles.Where(x => x.IsSelected).Select(x => x.RoleName).ToList();
        var current = await _userManager.GetRolesAsync(user);

        var toAdd = selected.Except(current).ToList();
        var toRemove = current.Except(selected).ToList();

        if (toAdd.Count > 0)
        {
            var addRes = await _userManager.AddToRolesAsync(user, toAdd);
            if (!addRes.Succeeded)
            {
                foreach (var e in addRes.Errors) ModelState.AddModelError(string.Empty, e.Description);
            }
        }

        if (toRemove.Count > 0)
        {
            var remRes = await _userManager.RemoveFromRolesAsync(user, toRemove);
            if (!remRes.Succeeded)
            {
                foreach (var e in remRes.Errors) ModelState.AddModelError(string.Empty, e.Description);
            }
        }

        if (!ModelState.IsValid)
        {
            // reload lists if errors
            return await OnGetAsync(id);
        }

        return RedirectToPage("/Admin/Users/Index"); // your users list page
    }
}