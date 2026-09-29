
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using try2.Models;
namespace try2.Controllers;

[ApiController]
[Route("[controller]")]
public class DeadlineController : ControllerBase
{
    private readonly StudentPlannerDbContext _db;
    public DeadlineController(StudentPlannerDbContext db)
    {
        _db = db;
    }
    [HttpGet]
    public async Task<IActionResult> getAll()
    {

        return Ok(await _db.Deadlines.ToListAsync());
    }

    [HttpGet("idDeadline/{id}")]
    public async Task<IActionResult> getIdDeadline(int id)
    {

        return Ok(await _db.Deadlines.Where(s => s.DeadlineId == id).ToListAsync());
    }

    [HttpGet("idSubject/{id}")]
    public async Task<IActionResult> getIdSubject(int id)
    {

        return Ok(await _db.Deadlines.Where(s => s.SubjectId == id).ToListAsync());
    }

    [HttpPost("addNewDeadline")]
    public async Task<IActionResult> addNewDeadline( [FromBody] CreateDeadlineDto dto)
    {
        var newDeadline = new Deadline() { SubjectId = dto.SubjectId, TaskDescription = dto.TaskDescription, DueDate = dto.DueDate };

        _db.Deadlines.Add(newDeadline);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("deleteDeadline/{id}")]
    public async Task<IActionResult> deleteDeadline(int id)
    {
        var deadline = await _db.Deadlines.FindAsync(id);
        if (deadline == null) return NotFound("Такого Deadline в базе нет!");

        _db.Deadlines.Remove(deadline);
        await _db.SaveChangesAsync();

        return Ok("Успешно удалено!");
    }
    [HttpPatch("changesDeadline/{id}")]
    public async Task<IActionResult> changeDeadLine(int id, [FromBody] UpdateDeadlineDto dto)
    {

        var deadLine = await _db.Deadlines.FindAsync(id);
        if (deadLine == null) return NotFound("Такого Deadline в базе нет!");

        deadLine.TaskDescription = dto.TaskDescription;
        deadLine.DueDate = dto.DueDate;
        deadLine.IsCompleted = dto.IsCompleted;

        await _db.SaveChangesAsync();
        return Ok(deadLine);
    } 
}


