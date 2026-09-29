using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using try2.Models;
namespace try2.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly StudentPlannerDbContext _db;
    public SubjectController(StudentPlannerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> getAll()
    {

        return Ok(await _db.Subjects.ToListAsync());
    }

    [HttpGet("idSubject/{id}")]
    public async Task<IActionResult> getIdSubject(int id)
    {

        return Ok(await _db.Subjects.Where(s => s.SubjectId == id).ToListAsync());
    }

    [HttpGet("withDetails/{id}")]
    public async Task<IActionResult> getWithDetails(int id)
    {

        var subject = await _db.Subjects
            .Include(s => s.Deadlines)
            .Include(s => s.LecturesNotes)
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.SubjectId == id);

        if (subject == null) return NotFound("Такого Subject в базе нет!");

        return Ok(subject);
    }

    [HttpPost("addNewSubject")]
    public async Task<IActionResult> addNewSubject([FromBody] CreateSubjectDto dto)
    {
        var newSubject = new Subject()
        {
            SubjectName = dto.SubjectName,
            Description = dto.Description
        };

        _db.Subjects.Add(newSubject);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("deleteSubject/{id}")]
    public async Task<IActionResult> deleteSubject(int id)
    {
        var subject = await _db.Subjects
            .Include(s => s.Deadlines)
            .Include(s => s.LecturesNotes)
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.SubjectId == id);

        if (subject == null) return NotFound("Такого Subject в базе нет!");

        if (subject.Deadlines.Any() || subject.LecturesNotes.Any() || subject.Schedules.Any())
            return BadRequest("Нельзя удалить предмет, пока есть связанные дедлайны, лекции или расписание!");

        _db.Subjects.Remove(subject);
        await _db.SaveChangesAsync();

        return Ok("Успешно удалено!");
    }

    [HttpPatch("changesSubject/{id}")]
    public async Task<IActionResult> changeSubject(int id, [FromBody] UpdateSubjectDto dto)
    {

        var subject = await _db.Subjects.FindAsync(id);
        if (subject == null) return NotFound("Такого Subject в базе нет!");

        subject.SubjectName = dto.SubjectName;
        subject.Description = dto.Description;

        await _db.SaveChangesAsync();
        return Ok(subject);
    }
}