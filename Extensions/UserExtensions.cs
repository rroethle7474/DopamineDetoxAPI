using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetoxAPI.Models.Entities;

public static class UserExtensions
{
    public static ApplicationUserDto ToApplicationUserDto(this ApplicationUser user, IMapper mapper)
    {
        return mapper.Map<ApplicationUserDto>(user);
    }
}