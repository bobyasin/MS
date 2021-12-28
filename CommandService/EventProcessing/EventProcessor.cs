using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            System.Console.WriteLine("--> Determining the event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    System.Console.WriteLine("--> Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    System.Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string platformPublishMessage)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishDto);

                    if (!repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();

                        System.Console.WriteLine("--> Platform added");
                    }
                    else
                    {
                        System.Console.WriteLine("--> Platform already exists...");
                    }


                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"--> Could not add platform to DB : {ex.Message}");
                }
            }
        }

    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}