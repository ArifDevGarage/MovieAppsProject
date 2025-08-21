using System.ComponentModel.DataAnnotations;

namespace MovieAppsProject.Models.DataModels;

public class Genre
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = null!;

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}