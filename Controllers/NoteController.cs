using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;
        private readonly IMapper _mapper;

        public NoteController(INoteService noteService, IMapper mapper)
        {
            _noteService = noteService;
            _mapper = mapper;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes(GetNotesRequest request, CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<NoteDto>>(await _noteService.GetNotesAsync(request));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDto>> GetNote(int id, CancellationToken cancellationToken = default)
        {
            var note = await _noteService.GetNoteAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return _mapper.Map<NoteDto>(note);
        }

        [HttpPost]
        public async Task<ActionResult<NoteDto>> CreateNote(NoteDto noteDto, CancellationToken cancellationToken = default)
        {
            var createdNote = await _noteService.CreateNoteAsync(_mapper.Map<Note>(noteDto), cancellationToken);

            return CreatedAtAction(nameof(GetNote), new { id = createdNote.Id }, _mapper.Map<NoteDto>(createdNote));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditNote(int id, NoteDto noteDto, CancellationToken cancellationToken = default)
        {
            if (id != noteDto.Id)
            {
                return BadRequest();
            }

            await _noteService.UpdateNoteAsync(id, _mapper.Map<Note>(noteDto), cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id, CancellationToken cancellationToken = default)
        {
            await _noteService.DeleteNoteAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpGet("user-list")]
        public async Task<ActionResult<IEnumerable<string>>> GetNotesUserList(CancellationToken cancellationToken = default)
        {
            var results = await _noteService.GetNotesUserList(cancellationToken);
            return Ok(results);
        }

        [HttpDelete("deleteByDate")]
        public async Task<IActionResult> DeleteNotesByDate([FromQuery] DateTime date, [FromQuery] bool isBefore, CancellationToken cancellationToken = default)
        {
            int deletedCount = 0;
            if(isBefore)
            {
                deletedCount = await _noteService.DeleteNotesBeforeDateAsync(date, cancellationToken);
            }
            else
            {
                deletedCount = await _noteService.DeleteNotesFromDateAsync(date, cancellationToken);
            }

            if (deletedCount > 0)
            {
                return Ok(new { message = $"{deletedCount} notes deleted." });
            }

            return NotFound(new { message = "No notes found to delete." });
        }


    }
}
