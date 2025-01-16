using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class HistorySearchResultMappingProfile : Profile
    {
        public HistorySearchResultMappingProfile()
        {
            CreateMap<HistorySearchResultDto, HistorySearchResult>().ReverseMap();
            CreateMap<HistorySearchResultEntity, HistorySearchResult>().ReverseMap();
        }
    }
}
