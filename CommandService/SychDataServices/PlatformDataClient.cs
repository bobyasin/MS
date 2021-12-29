using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;

namespace CommandService.SychDataServices
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                System.Console.WriteLine($"---> Caling GRPC Server");
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"---> Could not call GRPC Server : {ex}");
                return null;
            }
        }
    }
}