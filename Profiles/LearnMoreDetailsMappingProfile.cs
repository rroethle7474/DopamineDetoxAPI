using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class LearnMoreDetailsMappingProfile : Profile
    {
        public LearnMoreDetailsMappingProfile()
        {
            CreateMap<LearnMoreDetailDto, LearnMoreDetail>()
                .ReverseMap();

            CreateMap<LearnMoreDetailsEntity, LearnMoreDetail>()
                .ReverseMap();
        }
    }
}
