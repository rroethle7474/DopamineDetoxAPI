using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class ChannelMappingProfile : Profile
    {
        public ChannelMappingProfile()
        {
            CreateMap<ChannelDto, Channel>()
                .ForPath(dest => dest.ContentType.Title, opt => opt.MapFrom(src => src.ContentTypeName ?? string.Empty));

            CreateMap<Channel,  ChannelDto>()
                .ForPath(dest => dest.ContentTypeName, opt => opt.MapFrom(src => src.ContentType.Title ?? string.Empty));

            CreateMap<ChannelEntity, Channel>()
                //.ForPath(dest => dest.ContentType.Title, opt => opt.MapFrom(src => src.ContentTypeName ?? string.Empty))
                .ReverseMap();
        }
    }
}
