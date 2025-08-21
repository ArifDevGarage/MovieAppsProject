using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieAppsProject.Models.DataModels;

namespace MovieAppsProject.Areas.Cinema.Pages.RequestForm;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public Movie? Movie { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Movie = await _db.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Movie == null)
        {
            return NotFound();
        } 
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var m = await _db.Movies.FirstOrDefaultAsync(x => x.Id == id);
        if (m == null)
        {
            return NotFound();
        } 

        _db.Movies.Remove(m);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}