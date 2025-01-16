using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using DopamineDetoxAPI.Services.Interfaces;

namespace DopamineDetoxAPI.Services
{
    public class SubTopicService : ISubTopicService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly CacheService _cacheService;
        private const string SubTopicCacheKey = "SubTopics";

        public SubTopicService(ApplicationDbContext context, IMapper mapper, CacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<SubTopic> CreateSubTopicAsync(SubTopic subTopic, CancellationToken cancellationToken = default)
        {
            if (subTopic == null || String.IsNullOrEmpty(subTopic?.UserId) || String.IsNullOrEmpty(subTopic?.Term))
            {
                throw new Exception("UserId and Term are required fields");
            }

            if (subTopic.TopicId == 0)
            {
                throw new Exception("TopicId is a required field");
            }

            var st = new SubTopicEntity
            {
                IsActive = subTopic.IsActive,
                Term = subTopic.Term,
                UserId = subTopic.UserId,
                TopicId = subTopic.TopicId
            };

            _context.SubTopics.Add(st);
            await _context.SaveChangesAsync(cancellationToken);

            subTopic.Id = st.Id;

            // Clear cache
            _cacheService.Remove(SubTopicCacheKey);

            return subTopic;
        }

        public async Task<SubTopic> GetSubTopicAsync(int id, CancellationToken cancellationToken = default)
        {
            var subTopic = await _context.SubTopics
                                                 .Include(st => st.Topic)  // Include the related Topic entity
                                                 .FirstOrDefaultAsync(st => st.Id == id, cancellationToken);

            if (subTopic == null)
            {
                throw new Exception($"SubTopic with id {id} not found");
            }

            return _mapper.Map<SubTopic>(subTopic);
        }

        public async Task<IEnumerable<SubTopic>> GetSubTopicsAsync(GetSubTopicsRequest request, CancellationToken cancellationToken = default)
        {
            return await _cacheService.GetOrCreate(
                SubTopicCacheKey,
                async () =>
                {
                    var query = _context.SubTopics
                        .Include(st => st.Topic)
                        .AsQueryable();

                    if (!String.IsNullOrEmpty(request.UserId))
                    {
                        query = query.Where(st => st.UserId.ToLower() == request.UserId.ToLower());
                    }

                    if (request.IsActive.HasValue)
                    {
                        query = query.Where(st => st.IsActive == request.IsActive.Value);
                    }

                    if (!string.IsNullOrEmpty(request.Term))
                    {
                        query = query.Where(st => st.Term.Contains(request.Term));
                    }

                    if (request.TopicId.HasValue)
                    {
                        query = query.Where(st => st.TopicId == request.TopicId.Value);
                    }

                    return _mapper.Map<List<SubTopic>>(await query.ToListAsync(cancellationToken));
                },
                TimeSpan.FromHours(24)
            );
        }

        public async Task<SubTopic> UpdateSubTopicAsync(int id, SubTopic subTopic, CancellationToken cancellationToken = default)
        {
            var ste = await _context.SubTopics.FindAsync(new object[] { id }, cancellationToken);

            if (ste == null)
            {
                throw new Exception($"SubTopic with id {id} not found");
            }

            if (String.IsNullOrEmpty(subTopic?.UserId) || String.IsNullOrEmpty(subTopic?.Term))
            {
                throw new Exception("UserId and Term are required fields");
            }

            ste.IsActive = subTopic.IsActive;
            ste.Term = subTopic.Term;
            ste.UserId = subTopic.UserId;
            ste.TopicId = subTopic.TopicId;
            ste.UpdatedOn = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            _cacheService.Remove(SubTopicCacheKey);
            _cacheService.Remove($"{SubTopicCacheKey}_{id}");

            return subTopic;
        }

        public async Task DeleteSubTopicAsync(int id, CancellationToken cancellationToken = default)
        {
            var subTopic = await _context.SubTopics.FindAsync(new object[] { id }, cancellationToken);

            if (subTopic == null)
            {
                throw new Exception($"SubTopic with id {id} not found");
            }

            _context.SubTopics.Remove(subTopic);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            _cacheService.Remove(SubTopicCacheKey);
            _cacheService.Remove($"{SubTopicCacheKey}_{id}");
        }

        public bool ValidateCreateSubTopicDto(SubTopicDto subTopic)
        {
            // This method is not async, so no cancellation token is needed
            if (subTopic == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(subTopic.UserId) || String.IsNullOrEmpty(subTopic.Term))
            {
                return false;
            }

            if (subTopic.TopicId == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<SubTopic> GetSubTopicByUserIdAndTermAsync(string userId, string term, CancellationToken cancellationToken = default)
        {
            var subTopic = await _context.SubTopics
                                         .Include(st => st.Topic)
                                         .FirstOrDefaultAsync(st => st.UserId.ToLower() == userId.ToLower() && st.Term.ToLower() == term.ToLower().Trim(), cancellationToken);

            return _mapper.Map<SubTopic>(subTopic);
        }

        public async Task DeleteSubTopicsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var subTopics = await _context.SubTopics.Where(sb => sb.UserId == userId).ToListAsync(cancellationToken);

            if (subTopics == null || !subTopics.Any())
            {
                return;
            }

            _context.SubTopics.RemoveRange(subTopics);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            _cacheService.Remove(SubTopicCacheKey);
        }
    }
}