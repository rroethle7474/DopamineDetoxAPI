using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class QuoteMappingProfile : Profile
    {
        public QuoteMappingProfile()
        {
            CreateMap<QuoteDto, Quote>()
                .ReverseMap();

            CreateMap<QuoteEntity, Quote>()
                .ReverseMap();
        }
    }
}
