using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using System.Reflection.Emit;

namespace MovieApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Comment>()
           .HasOne(c => c.User)
           .WithMany(u => u.Comments)
           .HasForeignKey(c => c.UserId);

            builder.Entity<Comment>()
            .HasOne(c => c.Movie)
            .WithMany(m => m.Comments)
            .HasForeignKey(c => c.MovieId);

            

            builder.Entity<Rating>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Ratings)
            .HasForeignKey(c => c.MovieId);
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }

        private void SeedRoles(ModelBuilder builder)
        {   
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" },
                new IdentityRole() { Name = "HR", ConcurrencyStamp = "3", NormalizedName = "HR" }
                );
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
