using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace DopamineDetoxAPI.Services
{
    public class ChannelService : IChannelService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILoggingService _loggingService;
        private readonly CacheService _cacheService;
        private const string ChannelCacheKey = "Channels";
        private readonly HttpClient _httpClient;

        public ChannelService(ApplicationDbContext context, IMapper mapper, ILoggingService loggingService, CacheService cacheService, HttpClient httpClient)
        {
            _context = context;
            _mapper = mapper;
            _loggingService = loggingService;
            _cacheService = cacheService;
            _httpClient = httpClient;
        }

        public async Task<Channel> CreateChannelAsync(Channel channel, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(channel?.ChannelName) || string.IsNullOrEmpty(channel?.Identifier))
                {
                    throw new Exception("ChannelName and Identifier are required fields");
                }

                if (await _context.Channels.AnyAsync(c => c.ChannelName == channel.ChannelName && c.ContentTypeId == channel.ContentTypeId, cancellationToken))
                {
                    throw new Exception($"A channel with the name '{channel.ChannelName}' and content type '{channel.ContentTypeId}' already exists.");
                }

                if (await _context.Channels.AnyAsync(c => c.Identifier == channel.Identifier && c.ContentTypeId == channel.ContentTypeId, cancellationToken))
                {
                    throw new Exception($"A channel with the identifier '{channel.Identifier}' and content type '{channel.ContentTypeId}' already exists.");
                }

                var channelEntity = new ChannelEntity
                {
                    IsActive = channel.IsActive,
                    ChannelName = channel.ChannelName,
                    Identifier = channel.Identifier,
                    Description = channel.Description,
                    UserId = channel.UserId,
                    ContentTypeId = channel.ContentTypeId
                };

                _context.Channels.Add(channelEntity);
                await _context.SaveChangesAsync(cancellationToken);

                channel.Id = channelEntity.Id;

                _cacheService.Remove(ChannelCacheKey);

                return channel;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CHN001", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<Channel> GetChannelAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cacheService.GetOrCreate(
                    $"{ChannelCacheKey}_{id}",
                    async () =>
                    {
                        var channelEntity = await _context.Channels.FindAsync(new object[] { id }, cancellationToken);

                        if (channelEntity == null)
                        {
                            throw new Exception($"Channel with id {id} not found");
                        }

                        return _mapper.Map<Channel>(channelEntity);
                    },
                    TimeSpan.FromHours(24)
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CHN002", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<IEnumerable<Channel>> GetChannelsAsync(GetChannelsRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.Channels
                    .Include(c => c.ContentType)
                    .AsQueryable();

                if (!String.IsNullOrEmpty(request.UserId))
                {
                    query = query.Where(st => st.UserId.ToLower() == request.UserId.ToLower());
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(st => st.IsActive == request.IsActive.Value);
                }

                if (!string.IsNullOrEmpty(request.ChannelName))
                {
                    query = query.Where(st => st.ChannelName.Contains(request.ChannelName));
                }

                if (!string.IsNullOrEmpty(request.Identifier))
                {
                    query = query.Where(st => st.Identifier.Contains(request.Identifier));
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    query = query.Where(st => st.Description.Contains(request.Description));
                }

                if (request.ContentTypeId.HasValue)
                {
                    query = query.Where(st => st.ContentTypeId == request.ContentTypeId.Value);
                }

                var channelEntities = await query.ToListAsync(cancellationToken);
                return _mapper.Map<IEnumerable<Channel>>(channelEntities);
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CHN003", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<Channel> UpdateChannelAsync(int id, Channel channel, CancellationToken cancellationToken = default)
        {
            try
            {
                var channelEntity = await _context.Channels.FindAsync(new object[] { id }, cancellationToken);

                if (channelEntity == null)
                {
                    throw new Exception($"Channel with id {id} not found");
                }

                if (string.IsNullOrEmpty(channel?.ChannelName) || string.IsNullOrEmpty(channel?.Identifier))
                {
                    throw new Exception("ChannelName and Identifier are required fields");
                }

                if (await _context.Channels.AnyAsync(c => c.ChannelName == channel.ChannelName && c.ContentTypeId == channel.ContentTypeId && c.Id != id, cancellationToken))
                {
                    throw new Exception($"A channel with the name '{channel.ChannelName}' and content type '{channel.ContentTypeId}' already exists.");
                }

                if (await _context.Channels.AnyAsync(c => c.Identifier == channel.Identifier && c.ContentTypeId == channel.ContentTypeId && c.Id != id, cancellationToken))
                {
                    throw new Exception($"A channel with the identifier '{channel.Identifier}' and content type '{channel.ContentTypeId}' already exists.");
                }

                channelEntity.ChannelName = channel.ChannelName;
                channelEntity.Identifier = channel.Identifier;
                channelEntity.Description = channel.Description;
                channelEntity.IsActive = channel.IsActive;
                channelEntity.UserId = channel.UserId;
                channelEntity.ContentTypeId = channel.ContentTypeId;
                channelEntity.UpdatedOn = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                _cacheService.Remove(ChannelCacheKey);
                _cacheService.Remove($"{ChannelCacheKey}_{id}");

                return channel;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CHN004", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task DeleteChannelAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var channelEntity = await _context.Channels.FindAsync(new object[] { id }, cancellationToken);

                if (channelEntity == null)
                {
                    throw new Exception($"Channel with id {id} not found");
                }

                _context.Channels.Remove(channelEntity);
                await _context.SaveChangesAsync(cancellationToken);

                _cacheService.Remove(ChannelCacheKey);
                _cacheService.Remove($"{ChannelCacheKey}_{id}");
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CHN005", ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task DeleteChannelsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var channelEntities = await _context.Channels.Where(c => c.UserId == userId).ToListAsync(cancellationToken);

                if (channelEntities == null)
                {
                    return;
                }

                _context.Channels.RemoveRange(channelEntities);
                await _context.SaveChangesAsync(cancellationToken);

                _cacheService.Remove(ChannelCacheKey);
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync("CHN006", ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
