using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Profiles
{
    public class ApplicationUserMappingProfile : Profile
    {
        public ApplicationUserMappingProfile()
        {
            CreateMap<ApplicationUserDto, User>().ReverseMap();
            CreateMap<ApplicationUser, User>().ReverseMap();
            CreateMap<ApplicationUserDto, ApplicationUser>().ReverseMap();
        }
    }
}