using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class SearchResultReportMappingProfile : Profile
    {
        public SearchResultReportMappingProfile()
        {
            CreateMap<SearchResultReportDto, SearchResultReport>().ReverseMap();
            CreateMap<SearchResultReportEntity, SearchResultReport>().ReverseMap();
        }
    }
}
