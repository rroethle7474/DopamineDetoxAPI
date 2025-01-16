using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface INoteService
    {
        Task<Note> CreateNoteAsync(Note note, CancellationToken cancellationToken = default);
        Task<Note> GetNoteAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Note>> GetNotesAsync(GetNotesRequest request, CancellationToken cancellationToken = default);
        Task<Note> UpdateNoteAsync(int id, Note note, CancellationToken cancellationToken = default);
        Task DeleteNoteAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteNotesByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetNotesUserList(CancellationToken cancellationToken = default);
        Task<int> DeleteNotesFromDateAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<int> DeleteNotesBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
