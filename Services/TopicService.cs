using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class TopicService : ITopicService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly CacheService _cacheService;
        private const string TopicCacheKey = "Topics";
        private const string SubTopicCacheKey = "SubTopics";

        public TopicService(ApplicationDbContext context, IMapper mapper, CacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<Topic> CreateTopicAsync(Topic topic, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrEmpty(topic?.UserId) || String.IsNullOrEmpty(topic?.Term))
            {
                throw new Exception("UserId and Term are required fields");
            }

            topic.Term = topic.Term.Trim();

            var existingTopic = await _context.Topics
                .FirstOrDefaultAsync(t => t.UserId == topic.UserId && t.Term.ToLower() == topic.Term.ToLower(), cancellationToken);

            if (existingTopic != null)
            {
                throw new Exception("A topic with the same Term and UserId already exists.");
            }

            var ct = new TopicEntity
            {
                IsActive = topic.IsActive,
                Term = topic.Term.Trim(),
                UserId = topic.UserId
            };

            _context.Topics.Add(ct);
            await _context.SaveChangesAsync(cancellationToken);

            topic.Id = ct.Id;

            // Clear cache
            _cacheService.Remove(TopicCacheKey);

            return topic;
        }

        public async Task<Topic> GetTopicAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _cacheService.GetOrCreate(
                $"{TopicCacheKey}_{id}",
                async () =>
                {
                    var topic = await _context.Topics.FindAsync(new object[] { id }, cancellationToken);
                    if (topic == null)
                    {
                        throw new Exception($"Topic with id {id} not found");
                    }
                    return new Topic
                    {
                        Id = topic.Id,
                        IsActive = topic.IsActive,
                        Term = topic.Term,
                        UserId = topic.UserId
                    };
                },
                TimeSpan.FromHours(24)
            );
        }

        public async Task<IEnumerable<Topic>> GetTopicsAsync(GetTopicsRequest request, CancellationToken cancellationToken = default)
        {
                var query = _context.Topics.AsQueryable();

                if (request.Id.HasValue)
                {
                    query = query.Where(t => t.Id == request.Id.Value);
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(t => t.IsActive == request.IsActive.Value);
                }

                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(t => t.Term.Contains(request.Term));
                }

                if (!String.IsNullOrEmpty(request.UserId))
                {
                    query = query.Where(t => t.UserId == request.UserId);
                }

                return _mapper.Map<List<Topic>>(await query.ToListAsync(cancellationToken));
        }

        public async Task<Topic> UpdateTopicAsync(int id, Topic topicDto, CancellationToken cancellationToken = default)
        {
            var topic = await _context.Topics.FindAsync(new object[] { id }, cancellationToken);
            if (topic == null)
            {
                throw new Exception($"Topic with id {id} not found");
            }

            topic.IsActive = topicDto.IsActive;
            topic.Term = topicDto.Term;
            topic.UserId = topicDto.UserId;
            topic.UpdatedOn = DateTime.Now;

            _context.Topics.Update(topic);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            _cacheService.Remove(TopicCacheKey);
            _cacheService.Remove($"{TopicCacheKey}_{id}");
            _cacheService.Remove(SubTopicCacheKey);

            return topicDto;
        }

        public async Task DeleteTopicAsync(int id, CancellationToken cancellationToken = default)
        {
            var topic = await _context.Topics.FindAsync(new object[] { id }, cancellationToken);
            if (topic == null)
            {
                throw new Exception($"Topic with id {id} not found");
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            _cacheService.Remove(TopicCacheKey);
            _cacheService.Remove($"{TopicCacheKey}_{id}");
        }

        public async Task DeleteTopicsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var topics = await _context.Topics.Where(t => t.UserId == userId).ToListAsync(cancellationToken);

            if (topics == null || !topics.Any())
            {
                return;
            }

            _context.Topics.RemoveRange(topics);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            _cacheService.Remove(TopicCacheKey);
        }
    }
}