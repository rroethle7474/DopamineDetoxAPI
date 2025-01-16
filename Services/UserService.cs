using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> GetUserByEmail(string email, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null)
            {
                throw new Exception($"User with email {email} not found");
            }
            return _mapper.Map<User>(user);
        }

        public async Task<User> GetUserById(string id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                throw new Exception($"User with email {id} not found");
            }
            return _mapper.Map<User>(user);
        }

        public ApplicationUserDto MapToAppicationUserDto(ApplicationUser user)
        {
            return _mapper.Map<ApplicationUserDto>(user);
        }
    }
}
