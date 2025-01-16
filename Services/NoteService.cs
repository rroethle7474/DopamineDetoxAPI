using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public NoteService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Note> CreateNoteAsync(Note note, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(note?.Title) || string.IsNullOrEmpty(note?.Message))
            {
                throw new Exception("Title and Message are required fields");
            }

            var noteEntity = _mapper.Map<NoteEntity>(note);
            _context.Notes.Add(noteEntity);
            await _context.SaveChangesAsync(cancellationToken);
            note.Id = noteEntity.Id;
            note.SearchResult = _mapper.Map<SearchResult>(noteEntity.SearchResult);
            return note;
        }

        public async Task<Note> GetNoteAsync(int id, CancellationToken cancellationToken = default)
        {
            var noteEntity = await _context.Notes
                                           .Include(n => n.SearchResult)
                                           .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
            if (noteEntity == null)
            {
                throw new Exception($"Note with id {id} not found");
            }
            return _mapper.Map<Note>(noteEntity);
        }

        public async Task<IEnumerable<string>> GetNotesUserList(CancellationToken cancellationToken = default)
        {
            return await _context.Notes.Select(n => n.UserId).Distinct().ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Note>> GetNotesAsync(GetNotesRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.Notes
                                .Include(n => n.SearchResult)
                                .AsQueryable();

            if (request.Id.HasValue)
            {
                query = query.Where(n => n.Id == request.Id.Value);
            }
            if (!string.IsNullOrEmpty(request.UserId))
            {
                query = query.Where(n => n.UserId == request.UserId);
            }
            if (request.SearchResultId.HasValue)
            {
                query = query.Where(n => n.SearchResultId == request.SearchResultId.Value);
            }

            var noteEntities = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<Note>>(noteEntities);
        }

        public async Task<Note> UpdateNoteAsync(int id, Note note, CancellationToken cancellationToken = default)
        {
            var noteEntity = await _context.Notes.Include(n => n.SearchResult).FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
            if (noteEntity == null)
            {
                throw new Exception($"Note with id {id} not found");
            }
            if (string.IsNullOrEmpty(note?.Title) || string.IsNullOrEmpty(note?.Message))
            {
                throw new Exception("Title and Message are required fields");
            }

            _mapper.Map(note, noteEntity);
            noteEntity.UpdatedOn = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
            return note;
        }

        public async Task DeleteNoteAsync(int id, CancellationToken cancellationToken = default)
        {
            var noteEntity = await _context.Notes.FindAsync(new object[] { id }, cancellationToken);
            if (noteEntity == null)
            {
                throw new Exception($"Note with id {id} not found");
            }
            _context.Notes.Remove(noteEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteNotesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var noteEntities = await _context.Notes.Where(n => n.UserId == userId).ToListAsync(cancellationToken);

            if (noteEntities == null)
            {
                return;
            }

            _context.Notes.RemoveRange(noteEntities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteNotesFromDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var notesToDelete = await _context.Notes
                .Where(n => n.CreatedOn > date)
                .ToListAsync(cancellationToken);

            if (notesToDelete.Any())
            {
                _context.Notes.RemoveRange(notesToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }

        public async Task<int> DeleteNotesBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var notesToDelete = await _context.Notes
                .Where(n => n.CreatedOn < date)
                .ToListAsync(cancellationToken);

            if (notesToDelete.Any())
            {
                _context.Notes.RemoveRange(notesToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }
    }
}