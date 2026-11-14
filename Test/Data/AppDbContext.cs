using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.StudentCode)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.HasIndex(e => e.StudentCode)
                      .IsUnique();

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasIndex(e => e.Email)
                      .IsUnique();
            });
        }
    }
}
