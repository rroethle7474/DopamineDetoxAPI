using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class TopicMappingProfile : Profile
    {
        public TopicMappingProfile()
        {
            CreateMap<TopicDto, Topic>().ReverseMap();
            CreateMap<TopicEntity, Topic>()
                // handle default updates through base entity
                //.ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn == default ? DateTime.Now : src.CreatedOn))
                //.ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedOn == default ? DateTime.Now : src.UpdatedOn))
                .ReverseMap();
        }
    }
}