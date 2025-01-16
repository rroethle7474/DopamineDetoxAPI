using DopamineDetox.Domain.Models;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IContentTypeService
    {
        Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default);
        Task<ContentType> GetContentTypeAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default);
        Task<ContentType> UpdateContentTypeAsync(int id, ContentType contentType, CancellationToken cancellationToken = default);
        Task DeleteContentTypeAsync(int id, CancellationToken cancellationToken = default);
    }
}
