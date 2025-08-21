using System.ComponentModel.DataAnnotations;

namespace MovieAppsProject.Models.DataModels;

public class MovieGenre
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}

public class MovieCast
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    [StringLength(120)]
    public string? CharacterName { get; set; }

    /// <summary>Billing order (lower = top)</summary>
    public int Order { get; set; }
}

public class MovieCrew
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    [StringLength(80)]
    public string Department { get; set; } = "Directing";

    [StringLength(80)]
    public string Job { get; set; } = "Director";
}