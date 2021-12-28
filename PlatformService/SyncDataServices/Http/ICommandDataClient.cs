using System.Threading.Tasks;
using PlatformService.Dtos;

namespace platformservice.SyncDataServices.Http
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto platformReadDto);
    }
}