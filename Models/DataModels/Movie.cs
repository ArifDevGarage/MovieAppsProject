using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAppsProject.Models.DataModels;

public class Movie
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Overview { get; set; }

    public DateOnly? ReleaseDate { get; set; }
    public int? RuntimeMinutes { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Budget { get; set; }

    [StringLength(400)]
    public string? PosterUrl { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public ICollection<MovieCast> Cast { get; set; } = new List<MovieCast>();
    public ICollection<MovieCrew> Crew { get; set; } = new List<MovieCrew>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}