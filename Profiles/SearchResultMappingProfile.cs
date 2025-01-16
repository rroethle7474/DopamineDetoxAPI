using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class SearchResultMappingProfile : Profile
    {
        public SearchResultMappingProfile()
        {
            CreateMap<SearchResultDto, SearchResult>()
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.TopSearchResults, opt => opt.MapFrom(src => src.TopSearchResults))
                .ReverseMap();
            CreateMap<SearchResultEntity, SearchResult>()
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.TopSearchResults, opt => opt.MapFrom(src => src.TopSearchResults))
                .ReverseMap();
        }
    }
}
