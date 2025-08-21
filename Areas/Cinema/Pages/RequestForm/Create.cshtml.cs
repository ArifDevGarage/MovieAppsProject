using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
// using MovieAppsProject.Data;
using MovieAppsProject.Models.DataModels;
using System.ComponentModel.DataAnnotations;

namespace MovieAppsProject.Areas.Cinema.Pages.RequestForm;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputDto Input { get; set; } = new();

    public IList<Genre> Genres { get; set; } = new List<Genre>();

    public class InputDto
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = null!;
        [StringLength(2000)]
        public string? Overview { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public int? RuntimeMinutes { get; set; }
        public decimal? Budget { get; set; }
        public string? PosterUrl { get; set; }

        public List<int> SelectedGenres { get; set; } = new();
    }

    public async Task OnGet()
    {
        Genres = await _db.Genres.OrderBy(g => g.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Genres = await _db.Genres.OrderBy(g => g.Name).ToListAsync();
            return Page();
        }

        var movie = new Movie
        {
            Title = Input.Title,
            Overview = Input.Overview,
            ReleaseDate = Input.ReleaseDate,
            RuntimeMinutes = Input.RuntimeMinutes,
            Budget = Input.Budget,
            PosterUrl = Input.PosterUrl
        };

        foreach (var gid in Input.SelectedGenres.Distinct())
        {
            movie.MovieGenres.Add(new MovieGenre { GenreId = gid });
        }

        await _db.Movies.AddAsync(movie);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}