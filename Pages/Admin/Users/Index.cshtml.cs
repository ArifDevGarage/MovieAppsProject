using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieAppsProject.Areas.Data;          // ApplicationDbContext
using MovieAppsProject.Models;             // ApplicationUser

namespace MovieAppsProject.Pages.Admin.Users;

// [Authorize(Policy = "RequireAdmins")] // or: [Authorize(Roles = "Admin")]
[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public sealed record UserListItemVM(
        string Id,
        string? UserName,
        string? Email,
        List<string> Roles
    );

    public List<UserListItemVM> Users { get; private set; } = new();

    // Simple filters/paging
    [BindProperty(SupportsGet = true)]
    [Display(Name = "Search")]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 20;

    public int TotalCount { get; private set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(1, PageSize));

    public async Task<IActionResult> OnGetAsync()
    {
        PageNumber = Math.Max(1, PageNumber);
        PageSize = Math.Clamp(PageSize, 5, 100);

        // Base query of users (no tracking for faster read)
        var usersQuery = _db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var s = Search.Trim();
            usersQuery = usersQuery.Where(u =>
                (u.UserName ?? "").Contains(s) ||
                (u.Email ?? "").Contains(s));
        }

        // Count distinct users (before paging)
        TotalCount = await usersQuery.CountAsync();

        // Fetch the page of users (id, username, email)
        var pageUsers = await usersQuery
            .OrderBy(u => u.UserName)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(u => new { u.Id, u.UserName, u.Email })
            .ToListAsync();

        var userIds = pageUsers.Select(u => u.Id).ToList();

        // Fetch roles for ONLY the users on this page
        var rolesPerUser = await (from ur in _db.UserRoles.AsNoTracking()
                                  join r in _db.Roles.AsNoTracking() on ur.RoleId equals r.Id
                                  where userIds.Contains(ur.UserId)
                                  group r.Name by ur.UserId into g
                                  select new
                                  {
                                      UserId = g.Key,
                                      Roles = g.ToList()
                                  })
                                  .ToListAsync();

        var roleDict = rolesPerUser.ToDictionary(x => x.UserId, x => x.Roles);

        Users = pageUsers.Select(u =>
            new UserListItemVM(
                u.Id,
                u.UserName,
                u.Email,
                roleDict.TryGetValue(u.Id, out var rr) ? rr : new List<string>()
            )
        ).ToList();

        return Page();
    }
}