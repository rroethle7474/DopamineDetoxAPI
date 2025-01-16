using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class ContentTypeMappingProfile : Profile
    {
        public ContentTypeMappingProfile()
        {
            CreateMap<ContentTypeDto, ContentType>().ReverseMap();
            CreateMap<ContentTypeEntity, ContentType>().ReverseMap();
        }
    }
}