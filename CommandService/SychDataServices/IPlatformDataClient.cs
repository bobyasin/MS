using System.Collections.Generic;
using CommandService.Models;

namespace CommandService.SychDataServices
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}