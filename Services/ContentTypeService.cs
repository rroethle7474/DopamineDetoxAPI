using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class ContentTypeService : IContentTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILoggingService _loggingService;
        private readonly CacheService _cacheService;
        private const string ContentTypeCacheKey = "ContentTypes";

        public ContentTypeService(ApplicationDbContext context, IMapper mapper, ILoggingService loggingService, CacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _loggingService = loggingService;
            _cacheService = cacheService;
        }

        public async Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(contentType?.Title))
                {
                    throw new Exception("Name is a required field");
                }

                if (await _context.ContentTypes.AnyAsync(c => c.Title == contentType.Title, cancellationToken))
                {
                    throw new Exception($"A content type with the name '{contentType.Title}' already exists.");
                }

                var contentTypeEntity = new ContentTypeEntity
                {
                    Title = contentType.Title,
                    Description = contentType.Description
                };

                _context.ContentTypes.Add(contentTypeEntity);
                await _context.SaveChangesAsync(cancellationToken);

                contentType.Id = contentTypeEntity.Id;

                _cacheService.Remove(ContentTypeCacheKey);

                return contentType;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CT001", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<ContentType> GetContentTypeAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cacheService.GetOrCreate(
                    $"{ContentTypeCacheKey}_{id}",
                    async () =>
                    {
                        var contentTypeEntity = await _context.ContentTypes.FindAsync(new object[] { id }, cancellationToken);

                        if (contentTypeEntity == null)
                        {
                            throw new Exception($"Content type with id {id} not found");
                        }

                        return _mapper.Map<ContentType>(contentTypeEntity);
                    },
                    TimeSpan.FromHours(24)
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CT002", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cacheService.GetOrCreate(
                    ContentTypeCacheKey,
                    async () =>
                    {
                        var contentTypeEntities = await _context.ContentTypes.ToListAsync(cancellationToken);
                        return _mapper.Map<IEnumerable<ContentType>>(contentTypeEntities);
                    },
                    TimeSpan.FromHours(24)
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CT003", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<ContentType> UpdateContentTypeAsync(int id, ContentType contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                var contentTypeEntity = await _context.ContentTypes.FindAsync(new object[] { id }, cancellationToken);

                if (contentTypeEntity == null)
                {
                    throw new Exception($"Content type with id {id} not found");
                }

                if (string.IsNullOrEmpty(contentType?.Title))
                {
                    throw new Exception("Name is a required field");
                }

                if (await _context.ContentTypes.AnyAsync(c => c.Title == contentType.Title && c.Id != id, cancellationToken))
                {
                    throw new Exception($"A content type with the name '{contentType.Title}' already exists.");
                }

                contentTypeEntity.Title = contentType.Title;
                contentTypeEntity.Description = contentType.Description;
                contentTypeEntity.UpdatedOn = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                _cacheService.Remove(ContentTypeCacheKey);
                _cacheService.Remove($"{ContentTypeCacheKey}_{id}");

                return contentType;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CT004", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task DeleteContentTypeAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var contentTypeEntity = await _context.ContentTypes.FindAsync(new object[] { id }, cancellationToken);

                if (contentTypeEntity == null)
                {
                    throw new Exception($"Content type with id {id} not found");
                }

                _context.ContentTypes.Remove(contentTypeEntity);
                await _context.SaveChangesAsync(cancellationToken);

                _cacheService.Remove(ContentTypeCacheKey);
                _cacheService.Remove($"{ContentTypeCacheKey}_{id}");
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CT005", ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
