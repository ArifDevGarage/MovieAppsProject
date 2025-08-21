using System.ComponentModel.DataAnnotations;

namespace MovieAppsProject.Models.DataModels;

public class Person
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public ICollection<MovieCast> CastCredits { get; set; } = new List<MovieCast>();
    public ICollection<MovieCrew> CrewCredits { get; set; } = new List<MovieCrew>();
}