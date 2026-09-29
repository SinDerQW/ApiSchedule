using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using try2.Models;
namespace try2.Controllers;

[ApiController]
[Route("[controller]")]
public class LecturesNoteController : ControllerBase
{
    private readonly StudentPlannerDbContext _db;
    public LecturesNoteController(StudentPlannerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> getAll()
    {

        return Ok(await _db.LecturesNotes.ToListAsync());
    }

    [HttpGet("idNote/{id}")]
    public async Task<IActionResult> getIdNote(int id)
    {

        return Ok(await _db.LecturesNotes.Where(s => s.NoteId == id).ToListAsync());
    }

    [HttpGet("idSubject/{id}")]
    public async Task<IActionResult> getIdSubject(int id)
    {

        return Ok(await _db.LecturesNotes.Where(s => s.SubjectId == id).ToListAsync());
    }

    [HttpPost("addNewLecturesNote")]
    public async Task<IActionResult> addNewLecturesNote([FromBody] CreateLecturesNoteDto dto)
    {
        var newNote = new LecturesNote()
        {
            SubjectId = dto.SubjectId,
            LectureTitle = dto.LectureTitle,
            LectureDate = dto.LectureDate,
            FileLink = dto.FileLink
        };

        _db.LecturesNotes.Add(newNote);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("deleteLecturesNote/{id}")]
    public async Task<IActionResult> deleteLecturesNote(int id)
    {
        var note = await _db.LecturesNotes.FindAsync(id);
        if (note == null) return NotFound("Такого LecturesNote в базе нет!");

        _db.LecturesNotes.Remove(note);
        await _db.SaveChangesAsync();

        return Ok("Успешно удалено!");
    }

    [HttpPatch("changesLecturesNote/{id}")]
    public async Task<IActionResult> changeLecturesNote(int id, [FromBody] UpdateLecturesNoteDto dto)
    {

        var note = await _db.LecturesNotes.FindAsync(id);
        if (note == null) return NotFound("Такого LecturesNote в базе нет!");

        note.LectureTitle = dto.LectureTitle;
        note.LectureDate = dto.LectureDate;
        note.FileLink = dto.FileLink;

        await _db.SaveChangesAsync();
        return Ok(note);
    }
}