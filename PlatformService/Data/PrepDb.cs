using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                System.Console.WriteLine("---> Attempting to apply migrations");
                try
                {
                    context.Database.Migrate();
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"---> Colud not run migrations: {ex.Message}");
                }
            }

            if (!context.Platforms.Any())
            {
                System.Console.WriteLine("--> Seeding data...");

                context.Platforms.AddRange(
                    new Platform { Name = "dotnet", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "sql server", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "kubernetes", Publisher = "cloud native computing f.", Cost = "Free" }
                    );

                context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("--> We already have data");
            }
        }
    }
}