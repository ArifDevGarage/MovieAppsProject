// Models/DataContext/AppDbContext.cs
#pragma warning disable
using Microsoft.EntityFrameworkCore;
// using MovieAppsProject.Models.DataModels;

namespace MovieAppsProject.Models.DataModels;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<ExtEmployeeFromSintum> ExtEmployeeFromSinta { get; set; }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<MovieCast> MovieCast => Set<MovieCast>();
    public DbSet<MovieCrew> MovieCrew => Set<MovieCrew>();
    public DbSet<Rating> Ratings => Set<Rating>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<Product>().HasData(
        //     new Product { Id = 1, Name = "Laptop", Price = 1200.00M },
        //     new Product { Id = 2, Name = "Phone", Price = 800.00M }
        // );


        // DateOnly mapping (EF Core 8+ supports DateOnly out-of-box for SQL Server)
        modelBuilder.Entity<Person>().Property(p => p.BirthDate);
        modelBuilder.Entity<Movie>().Property(m => m.ReleaseDate);

        // Movie
        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.Title);

        modelBuilder.Entity<Movie>()
            .Property(m => m.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Movie>()
            .Property(m => m.UpdatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        // Genre
        modelBuilder.Entity<Genre>()
            .HasIndex(g => g.Name)
            .IsUnique();

        // Person
        modelBuilder.Entity<Person>()
            .HasIndex(p => p.FullName);

        // Many-to-many Movie <-> Genre
        modelBuilder.Entity<MovieGenre>()
            .HasKey(x => new { x.MovieId, x.GenreId });

        modelBuilder.Entity<MovieGenre>()
            .HasOne(x => x.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieGenre>()
            .HasOne(x => x.Genre)
            .WithMany(g => g.MovieGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cast (Movie-Person)
        modelBuilder.Entity<MovieCast>()
            .HasKey(x => new { x.MovieId, x.PersonId });

        modelBuilder.Entity<MovieCast>()
            .HasIndex(x => new { x.MovieId, x.Order });

        // Crew (Movie-Person + Job)
        modelBuilder.Entity<MovieCrew>()
            .HasKey(x => new { x.MovieId, x.PersonId, x.Job });

        // Rating
        modelBuilder.Entity<Rating>()
            .Property(r => r.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Rating>()
            .HasIndex(r => new { r.MovieId, r.Score });

        // Check constraint (server-side) for Score range
        modelBuilder.Entity<Rating>()
            .ToTable(t => t.HasCheckConstraint("CK_Ratings_Score_Range", "[Score] BETWEEN 1 AND 10"));

        // Seed a couple of genres for convenience
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Action" },
            new Genre { Id = 2, Name = "Drama" }
        );

        modelBuilder.Entity<ExtEmployeeFromSintum>().HasData(
            new ExtEmployeeFromSintum
            {
                Id = -1,
                EmployeeId = "635342",
                PositionId = "645326"
            }
            // {
                // Id = 1, Name = "Laptop", Price = 1200.00M

                // 'EmployeeId = "12213213";



                // public string? EmployeeName;



                // public string PositionId;



                // public string? PositionName ;



                // public string? Area ;



                // public string? PlantArea ;



                // public string? Directorate ;



                // public string? Function ;



                // public string? Department ;



                // public string? Email ;



                // public string? Level ;

                

                // public string? SuperiorId ;



                // public string? SuperiorPositionId = "Superior Position Id";

                

                // public string? UserName ;

                
                // public string? Unit ;



                // public string? Posgrd ;

                

                // public string? CostCenter ;



                // public string? Entity ;

            
                // public DateTime? LastUpdate ;

                
                // public bool? HelperIsDelegate ;


                // public byte? HelperEmployeePositionTypeId ;'

            // }
            // new ExtEmployeeFromSintum { Id = 2, Name = "Phone", Price = 800.00M }
        );

    }

    public override int SaveChanges()
    {
        TouchTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TouchTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void TouchTimestamps()
    {
        var entries = ChangeTracker.Entries<Movie>();
        var now = DateTime.UtcNow;
        foreach (var e in entries)
        {
            if (e.State == EntityState.Added)
            {
                e.Entity.CreatedAtUtc = now;
                e.Entity.UpdatedAtUtc = now;
            }
            else if (e.State == EntityState.Modified)
            {
                e.Entity.UpdatedAtUtc = now;
            }
        }
    }
}