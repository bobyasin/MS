using System.Collections.Generic;
using CommandService.Models;
using CommandService.SychDataServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            System.Console.WriteLine("---> Seeding new platforms");
            if (platforms != null)
            {
                foreach (var platform in platforms)
                {
                    if (!repo.ExternalPlatformExists(platform.ExternalId))
                        repo.CreatePlatform(platform);

                    repo.SaveChanges();
                }
            }
        }
    }
}