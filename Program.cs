using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieAppsProject.Models;
using MovieAppsProject.Models.DataModels;
using MovieAppsProject.Areas.Data;


namespace MovieAppsProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -----------------------------
            // Configure Services
            // -----------------------------
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // -----------------------------
            // Optional: Check Migrations
            // -----------------------------
            // using (var scope = app.Services.CreateScope())
            // {
            //     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //     if (db.Database.GetPendingMigrations().Any())
            //     {
            //         Console.WriteLine("⚠️ Pending migrations exist.");
            //     }
            //     else
            //     {
            //         Console.WriteLine("✅ All migrations applied.");
            //     }
            // }

            // -----------------------------
            // Configure Middleware
            // -----------------------------
            Configure(app, app.Environment);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                // dev-friendly defaults; tighten later
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToAreaFolder("Identity", "/Account");

                // options.Conventions.AllowAnonymousToPage("/Index");
                // options.Conventions.AllowAnonymousToPage("/Privacy");

                // Global fallback: require auth elsewhere
                options.Conventions.AuthorizeFolder("/Secure"); // all pages in /Secure require sign-in

                // options.Conventions.AuthorizeFolder("/Admin", "RequireAdmins"); // only Admins
                options.Conventions.AuthorizeFolder("/Admin"); // all pages in /Secure require sign-in
            });


            // (Optional) Policy examples
            services.AddAuthorization(options =>
            {

                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("RequireAdmins", policy => policy.RequireRole("Admin"));
            });
        }

        private static void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapRazorPages();
        }
    }
}