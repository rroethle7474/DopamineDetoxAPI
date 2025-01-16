using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class TopSearchResultMappingProfile : Profile
    {
        public TopSearchResultMappingProfile()
        {
            CreateMap<TopSearchResultDto, TopSearchResult>()
                .ForMember(dest => dest.SearchResult, opt => opt.MapFrom(src => src.SearchResult))
                .ReverseMap();
            CreateMap<TopSearchResultEntity, TopSearchResult>()
                .ForMember(dest => dest.SearchResult, opt => opt.MapFrom(src => src.SearchResult))
                .ReverseMap();
        }
    }
}
