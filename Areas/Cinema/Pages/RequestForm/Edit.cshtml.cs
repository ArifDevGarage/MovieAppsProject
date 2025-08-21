using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovieAppsProject.Models.DataModels;
using System.ComponentModel.DataAnnotations;

namespace MovieAppsProject.Areas.Cinema.Pages.RequestForm;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputDto Input { get; set; } = new();

    public IList<Genre> AllGenres { get; set; } = new List<Genre>();

    public class InputDto
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = null!;
        [StringLength(2000)]
        public string? Overview { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public int? RuntimeMinutes { get; set; }
        public decimal? Budget { get; set; }
        [StringLength(400)]
        public string? PosterUrl { get; set; }

        public List<int> SelectedGenres { get; set; } = new();
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var movie = await _db.Movies
            .Include(m => m.MovieGenres)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        } 

        Input = new InputDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Overview = movie.Overview,
            ReleaseDate = movie.ReleaseDate,
            RuntimeMinutes = movie.RuntimeMinutes,
            Budget = movie.Budget,
            PosterUrl = movie.PosterUrl,
            SelectedGenres = movie.MovieGenres.Select(mg => mg.GenreId).ToList()
        };

        AllGenres = await _db.Genres.AsNoTracking().OrderBy(g => g.Name).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            AllGenres = await _db.Genres.AsNoTracking().OrderBy(g => g.Name).ToListAsync();
            return Page();
        }

        var movie = await _db.Movies
            .Include(m => m.MovieGenres)
            .FirstOrDefaultAsync(m => m.Id == Input.Id);
        if (movie == null) return NotFound();

        // Update scalar fields
        movie.Title = Input.Title;
        movie.Overview = Input.Overview;
        movie.ReleaseDate = Input.ReleaseDate;
        movie.RuntimeMinutes = Input.RuntimeMinutes;
        movie.Budget = Input.Budget;
        movie.PosterUrl = Input.PosterUrl;

        // Update genre links (diff)
        var desired = Input.SelectedGenres.Distinct().ToHashSet();
        var existing = movie.MovieGenres.Select(mg => mg.GenreId).ToHashSet();

        // remove
        movie.MovieGenres.RemoveWhere(mg => !desired.Contains(mg.GenreId));
        // add
        foreach (var gid in desired.Except(existing))
            movie.MovieGenres.Add(new MovieGenre { GenreId = gid });

        await _db.SaveChangesAsync();
        return RedirectToPage("Details", new { id = movie.Id });
    }
}

// Small helper to allow RemoveWhere on ICollection<T> without converting to List
file static class CollectionExtensions
{
    public static void RemoveWhere<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var toRemove = source.Where(predicate).ToList();
        foreach (var item in toRemove) source.Remove(item);
    }
}