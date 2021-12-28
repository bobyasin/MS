using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace platformservice.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);// url kısmını appsettings.Development.json e parametre olarak ekledik

            if (response.IsSuccessStatusCode)
            {
                System.Console.WriteLine("--> Synch POST to CommandService was OK");
            }
            else
            {
                System.Console.WriteLine("--> Synch POST to CommandService was NOT OK");
            }
        }
    }
}