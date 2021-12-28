using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }

        public DbSet<Platform> Platforms { get; set; }

        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Platform>()
                .HasMany(c => c.Commands)
                .WithOne(p => p.Platform!)
                .HasForeignKey(f => f.PlatformId);

            modelBuilder
                .Entity<Command>()
                .HasOne(p => p.Platform)
                .WithMany(c => c.Commands)
                .HasForeignKey(f => f.PlatformId);
        }

    }
}