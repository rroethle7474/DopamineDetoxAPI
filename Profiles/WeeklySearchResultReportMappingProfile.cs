using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class WeeklySearchResultReportMappingProfile : Profile
    {
        public WeeklySearchResultReportMappingProfile()
        {
            CreateMap<WeeklySearchResultReportDto, WeeklySearchResultReport>().ReverseMap();
            CreateMap<WeeklySearchResultReportEntity, WeeklySearchResultReport>().ReverseMap();
        }
    }
}
