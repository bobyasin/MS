using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using platformservice.SyncDataServices.Http;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [Route("api/platform")]// == [Route("api/[controller]")] // aynı. Burada controller adını atayıp aynı sonucu verir veya elle yazabiliriz
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]//[HttpGet("GetPlatforms")] // bu şekilde de olur    
        [Route("GetPlatforms")]// bu şekilde de olur    
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            System.Console.WriteLine("--> Getting platforms");

            var platforms = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        //[HttpGet]
        //[Route("GetPlatformById/{id}")]   // bu şekilde route tanımı yapabiliriz. parametre kısmında {} ile map olur
        //[HttpGet("GetPlatformById/{id}")]
        [HttpGet("{id}", Name = "GetPlatformById")]
        [Route("GetPlatformById/{id}")]

        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = _repository.GetPlatformById(id);
            if (platform == null)
                return NotFound();

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatfromCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            var result = _repository.SaveChange();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            //Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"---> could not send synchronously. {ex.Message}");
            }

            //Send Async Message

            try
            {
                var platformPublishDto = _mapper.Map<PlatformPublishDto>(platformReadDto);
                platformPublishDto.Event = "Platform_Published";

                _messageBusClient.PublishNewPlatform(platformPublishDto);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"---> could not send Asynchronously. {ex.Message}");
            }


            return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
            //burada hata almamak için http get içinde name attribute unu belirtmek lazım
        }
    }
}