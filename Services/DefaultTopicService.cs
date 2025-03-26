using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class DefaultTopicService : IDefaultTopicService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DefaultTopicService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DefaultTopic> CreateDefaultTopicAsync(DefaultTopic topic, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrEmpty(topic?.Term))
            {
                throw new Exception("Term is a required fields");
            }

            topic.Term = topic.Term.Trim();

            var existingTopic = await _context.DefaultTopics
                .FirstOrDefaultAsync(t => t.Term.ToLower() == topic.Term.ToLower(), cancellationToken);

            if (existingTopic != null)
            {
                throw new Exception("A default topic with the same Term already exists.");
            }

            var ct = new DefaultTopicEntity
            {
                Term = topic.Term.Trim(),
                ExcludeFromTwitter = topic.ExcludeFromTwitter,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };

            _context.DefaultTopics.Add(ct);
            await _context.SaveChangesAsync(cancellationToken);

            topic.Id = ct.Id;

            return topic;
        }

        public async Task<DefaultTopic> GetDefaultTopicAsync(int id, CancellationToken cancellationToken = default)
        {
            var topic = await _context.DefaultTopics.FindAsync(new object[] { id }, cancellationToken);
            if (topic == null)
            {
                throw new Exception($"Default Topic with id {id} not found");
            }
            return new DefaultTopic
            {
                Id = topic.Id,
                Term = topic.Term,
                ExcludeFromTwitter = topic.ExcludeFromTwitter,
                CreatedOn = topic.CreatedOn,
                UpdatedOn = topic.UpdatedOn
            };
        }

        public async Task<IEnumerable<DefaultTopic>> GetDefaultTopicsAsync(CancellationToken cancellationToken = default)
        {
            var defaultTopics = await _context.DefaultTopics.ToListAsync(cancellationToken);
            if (defaultTopics == null)
            {
                throw new Exception("No default topics found");
            }
            return defaultTopics.Select(t => new DefaultTopic
            {
                Id = t.Id,
                Term = t.Term,
                ExcludeFromTwitter = t.ExcludeFromTwitter,
                CreatedOn = t.CreatedOn,
                UpdatedOn = t.UpdatedOn
            });
        }

        public async Task<DefaultTopic> UpdateDefaultTopicAsync(int id, DefaultTopic topicDto, CancellationToken cancellationToken = default)
        {
            var topic = await _context.DefaultTopics.FindAsync(new object[] { id }, cancellationToken);
            if (topic == null || topicDto == null)
            {
                throw new Exception($"Default Topic with id {id} not found");
            }

            topic.Term = topicDto.Term;
            topic.ExcludeFromTwitter = topicDto.ExcludeFromTwitter;
            topic.UpdatedOn = DateTime.Now;

            _context.DefaultTopics.Update(topic);
            await _context.SaveChangesAsync(cancellationToken);

            return topicDto;
        }

        public async Task DeleteDefaultTopicAsync(int id, CancellationToken cancellationToken = default)
        {
            var topic = await _context.DefaultTopics.FindAsync(new object[] { id }, cancellationToken);
            if (topic == null)
            {
                throw new Exception($"Default Topic with id {id} not found");
            }

            _context.DefaultTopics.Remove(topic);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
