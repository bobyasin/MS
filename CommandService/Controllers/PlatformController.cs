using System.Collections.Generic;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")] // c => command controller olduğunu belirtmek için eklendi (gateway için)
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;

        public PlatformController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        //[Route("GetPlatforms")] //bunu belirtirsek request path de metot adını da vermek gerekir.
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            System.Console.WriteLine("---> getting platforms from CommandService");

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_commandRepo.GetAllPlatforms()));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            System.Console.WriteLine("--> Inbound POST # Command Service");

            return Ok("Inbound test OK");
        }
    }
}