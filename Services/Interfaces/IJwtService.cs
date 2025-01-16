using DopamineDetoxAPI.Models.Entities;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IJwtService
    {
        SecurityToken GenerateJwtToken(ApplicationUser user);
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token);
    }
}
