using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class SubTopicMappingProfile : Profile
    {
        public SubTopicMappingProfile()
        {
            CreateMap<SubTopicDto, SubTopic>()
                .ForPath(dest => dest.Topic.Term, opt => opt.MapFrom(src => src.TopicTerm ?? string.Empty))
                .ReverseMap();

            CreateMap<SubTopicEntity, SubTopic>()
                .ForPath(dest => dest.Topic.Term, opt => opt.MapFrom(src => src.Topic.Term ?? string.Empty))
                .ReverseMap();
        }
    }
}