using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieAppsProject.Models.DataModels;

namespace MovieAppsProject.Areas.Cinema.Pages.RequestForm;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public DetailsModel(AppDbContext db)
    {
        _db = db;
    }

    public Movie? Movie { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public IList<MovieCast> Cast { get; set; } = new List<MovieCast>();
    public IList<MovieCrew> Crew { get; set; } = new List<MovieCrew>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Movie = await _db.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Movie == null) return NotFound();

        // Rating summary
        var rAgg = await _db.Ratings
            .Where(r => r.MovieId == id)
            .GroupBy(r => r.MovieId)
            .Select(g => new { Avg = g.Average(x => (double)x.Score), Cnt = g.Count() })
            .FirstOrDefaultAsync();

        AverageRating = rAgg?.Avg ?? 0.0;
        RatingsCount = rAgg?.Cnt ?? 0;

        // Cast (ordered billing)
        Cast = await _db.MovieCast
            .Where(c => c.MovieId == id)
            .Include(c => c.Person)
            .AsNoTracking()
            .OrderBy(c => c.Order)
            .ToListAsync();

        // Crew (group in view by Department)
        Crew = await _db.MovieCrew
            .Where(c => c.MovieId == id)
            .Include(c => c.Person)
            .AsNoTracking()
            .OrderBy(c => c.Department).ThenBy(c => c.Job).ThenBy(c => c.Person.FullName)
            .ToListAsync();

        return Page();
    }
}