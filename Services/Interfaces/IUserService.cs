using DopamineDetox.Domain.Models;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmail(string email, CancellationToken cancellationToken = default);
        Task<User> GetUserById(string id, CancellationToken cancellationToken = default);
    }

}