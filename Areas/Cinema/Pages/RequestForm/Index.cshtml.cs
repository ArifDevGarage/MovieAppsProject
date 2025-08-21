using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieAppsProject.Models.DataModels;

namespace MovieAppsProject.Areas.Cinema.Pages.RequestForm;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public sealed record MovieRow(Movie Movie, double AvgRating);
    public IList<MovieRow> Items { get; set; } = new List<MovieRow>();
    public IList<Genre> Genres { get; set; } = new List<Genre>();

    public string? Q { get; set; }
    public int? GenreId { get; set; }

    public async Task OnGetAsync(string? q, int? genreId)
    {
        Q = q;
        GenreId = genreId;

        var query = _db.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(m => m.Title.Contains(q));

        if (genreId is { } gid)
            query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == gid));

        Items = await query
            .OrderBy(m => m.Title)
            .Select(m => new MovieRow(
                m,
                _db.Ratings.Where(r => r.MovieId == m.Id)
                           .Select(r => (double?)r.Score)
                           .Average() ?? 0.0
            ))
            .Take(200)
            .ToListAsync();

        Genres = await _db.Genres.AsNoTracking().OrderBy(g => g.Name).ToListAsync();
    }
}