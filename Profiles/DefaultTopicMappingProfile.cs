using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class DefaultTopicMappingProfile : Profile
    {
        public DefaultTopicMappingProfile()
        {
            CreateMap<DefaultTopicDto, DefaultTopic>().ReverseMap();
            CreateMap<DefaultTopicEntity, DefaultTopic>().ReverseMap();
        }
    }
}