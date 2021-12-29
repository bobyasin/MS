using System.Collections.Generic;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platform/{platformId}/[controller]")]// resimlerdeki multi resource urls e göre bu pattern buraya verildi
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;

        public CommandController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommands(int platformId)
        {
            System.Console.WriteLine($"---> Gettting commands by platformId({platformId}) from CommandController");

            if (!_commandRepo.PlatformExists(platformId))
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_commandRepo.GetCommandsByPlatformId(platformId)));
        }

        [HttpGet("{commandId}", Name = "GetCommandByPlatformId")]
        public ActionResult<CommandReadDto> GetCommandByPlatformId(int platformId, int commandId)
        {
            System.Console.WriteLine($"---> Gettting command by platformId({platformId}/commandId={commandId}) from GetCommandByPlatformId");

            if (!_commandRepo.PlatformExists(platformId))
                return NotFound();

            var command = _commandRepo.GetCommand(platformId, commandId);

            if (command == null)
                return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            System.Console.WriteLine($"---> Hit CreateCommandForPlatform: {platformId}");

            // if (!_commandRepo.PlatformExists(platformId))
            //     return NotFound();

            var command = _mapper.Map<Command>(commandCreateDto);
            _commandRepo.CreateCommand(platformId, command);

            _commandRepo.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandByPlatformId),
                new
                {
                    platformId = platformId,
                    commandId = command.Id
                }, commandReadDto);
        }
    }
}