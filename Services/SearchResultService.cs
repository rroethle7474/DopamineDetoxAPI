using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class SearchResultService : ISearchResultService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITopicService _topicService;
        private readonly ISubTopicService _subTopicService;
        private readonly IChannelService _channelService;

        public SearchResultService(ApplicationDbContext context, IMapper mapper,
            ITopicService topicService, ISubTopicService subTopicService, IChannelService channelService)
        {
            _context = context;
            _mapper = mapper;
            _topicService = topicService;
            _subTopicService = subTopicService;
            _channelService = channelService;
        }

        public async Task<SearchResult> CreateSearchResultAsync(SearchResult searchResult, CancellationToken cancellationToken = default)
        {
            var searchResultEntity = _mapper.Map<SearchResultEntity>(searchResult);
            searchResultEntity.DateAdded = DateTime.Now;

            _context.SearchResults.Add(searchResultEntity);
            await _context.SaveChangesAsync(cancellationToken);

            searchResult.Id = searchResultEntity.Id;

            return searchResult;
        }

        public async Task<SearchResultsResponseDto> AddMultipleSearchResultsAsync(IEnumerable<SearchResult> searchResults, CancellationToken cancellationToken = default)
        {
            SearchResultsResponseDto response = new SearchResultsResponseDto();

            List<SearchResult> validatedResults = new List<SearchResult>();
            List<SearchResult> duplicateResults = new List<SearchResult>();
            foreach(var result in searchResults)
            {
                if (!String.IsNullOrEmpty(result?.Url))
                {
                    bool isExisting = await IsExistingSearchResultAsync(result);
                    if (!isExisting)
                        validatedResults.Add(result);
                    else
                        duplicateResults.Add(result);
                }
            }

            response.DuplicateCount = duplicateResults.Count();

            var searchResultEntities = _mapper.Map<IEnumerable<SearchResultEntity>>(validatedResults);
            _context.SearchResults.AddRange(searchResultEntities);

            if (searchResultEntities.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
                response.SuccessCount = searchResultEntities.Count();
                return response;
            }

            return response;
        }

        public async Task<bool> IsExistingSearchResultAsync(SearchResult searchResult)
        {
            return await _context.SearchResults.Where(
                sr => sr.Url.Trim().ToLower() == searchResult.Url && 
                sr.IsHomePage == searchResult.IsHomePage 
                && sr.IsChannel == searchResult.IsChannel).
                AnyAsync();
        }

        public async Task<SearchResult> GetSearchResultAsync(int id, CancellationToken cancellationToken = default)
        {
            var searchResultEntity = await _context.SearchResults.FindAsync(new object[] { id }, cancellationToken);

            if (searchResultEntity == null)
            {
                throw new Exception($"SearchResult with id {id} not found");
            }

            return _mapper.Map<SearchResult>(searchResultEntity);
        }

        public async Task<IEnumerable<SearchResult>> GetSearchResultsAsync(GetSearchResultsRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.SearchResults.
                Include(sr => sr.Notes).
                Include(sr => sr.TopSearchResults).
                AsQueryable();

            if(String.IsNullOrEmpty(request.UserId) && request.isMVPResults == true)
            {
                query = query.Where(sr => sr.TopSearchResult == true || (sr.Notes != null && sr.Notes.Any()));
                var topSearchResults = await query.ToListAsync(cancellationToken);
                return _mapper.Map<List<SearchResult>>(topSearchResults);
            }

            var homePageQuery = _context.SearchResults
                                .Include(sr => sr.Notes).
                                AsQueryable().Where(sr => sr.IsHomePage == true);

            if (String.IsNullOrEmpty(request.UserId) || request.IsHomePage)
            {
                var homePageResults = await _context.SearchResults.Where(sr => sr.IsHomePage == true).ToListAsync(cancellationToken);
                return _mapper.Map<List<SearchResult>>(homePageResults);
            }

            var channelTermsForUser = await _context.Channels
                .Where(c => c.UserId == request.UserId && c.IsActive)
                .Select(c => c.Identifier).ToListAsync(cancellationToken);

            var topicTermsForUser = await _context.Topics
                .Where(t => t.UserId == request.UserId && t.IsActive)
                .Select(t => t.Term).ToListAsync(cancellationToken);

            var subTopicTermsForUser = await _context.SubTopics
                .Where(st => st.UserId == request.UserId && st.IsActive)
                .Select(st => st.Term).ToListAsync(cancellationToken);

            var combinedTerms = channelTermsForUser
            .Union(topicTermsForUser)
            .Union(subTopicTermsForUser)
            .Distinct()
            .ToList();

            if (request.ToDate == null)
            {
                request.ToDate = DateTime.Today.AddDays(1);
            }

            if (request.FromDate == null)
            {
                request.FromDate = DateTime.Today.AddDays(-1);
            }

            if (request.FromDate >= request.ToDate)
            {
                throw new Exception($"Search Dates are invalid. FromDate needs to be before To Date.");
            }

            if (request.ContentTypeId.HasValue)
            {
                query = query.Where(sr => sr.ContentTypeId == request.ContentTypeId.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(sr => sr.DateAdded >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(sr => sr.DateAdded <= request.ToDate.Value);
            }
            query = query.Where(sr => combinedTerms.Contains(sr.Term));

            if (request.isMVPResults == true)
            {
                query = query.Where(sr => sr.TopSearchResult == true || (sr.Notes != null && sr.Notes.Any()));
                var topSearchResults = await query.ToListAsync(cancellationToken);
                return _mapper.Map<List<SearchResult>>(topSearchResults);
            }

            try
            {
                var searchResults = await query.ToListAsync(cancellationToken);
                var homePageSearchResults = await homePageQuery.ToListAsync(cancellationToken);

                if (homePageSearchResults != null && homePageSearchResults.Count() > 0)
                    searchResults.AddRange(homePageSearchResults);

                return _mapper.Map<List<SearchResult>>(searchResults);
            }
            catch(Exception e)
            {
                string exceptionMessage = e.Message;
                throw new Exception($"Error getting search results. {exceptionMessage}");
            }

        }

        public async Task<SearchResult> UpdateSearchResultAsync(int id, SearchResult searchResult, CancellationToken cancellationToken = default)
        {
            var searchResultEntity = await _context.SearchResults.FindAsync(new object[] { id }, cancellationToken);

            if (searchResultEntity == null)
            {
                throw new Exception($"SearchResult with id {id} not found");
            }

            _mapper.Map(searchResult, searchResultEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return searchResult;
        }

        public async Task DeleteSearchResultAsync(int id, CancellationToken cancellationToken = default)
        {
            var searchResultEntity = await _context.SearchResults.FindAsync(new object[] { id }, cancellationToken);

            if (searchResultEntity == null)
            {
                throw new Exception($"SearchResult with id {id} not found");
            }

            _context.SearchResults.Remove(searchResultEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<int> DeleteSearchResultsFromDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var resultsToDelete = await _context.SearchResults
                .Where(r => r.DateAdded > date)
                .ToListAsync(cancellationToken);

            if (resultsToDelete.Any())
            {
                _context.SearchResults.RemoveRange(resultsToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }

        public async Task<int> DeleteSearchResultsBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var resultsToDelete = await _context.SearchResults
                .Where(r => r.DateAdded < date)
                .ToListAsync(cancellationToken);

            if (resultsToDelete.Any())
            {
                _context.SearchResults.RemoveRange(resultsToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }
    }
}