using System.ComponentModel.DataAnnotations;

namespace MovieAppsProject.Models.DataModels;

public class Rating
{
    public long Id { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    [Range(1, 10)]
    public int Score { get; set; }

    [StringLength(120)]
    public string? UserName { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}