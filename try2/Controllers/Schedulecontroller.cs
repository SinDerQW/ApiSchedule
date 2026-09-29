using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using try2.Models;
namespace try2.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly StudentPlannerDbContext _db;
    public ScheduleController(StudentPlannerDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> getAll()
    {

        return Ok(await _db.Schedules.ToListAsync());
    }

    [HttpGet("idLesson/{id}")]
    public async Task<IActionResult> getIdLesson(int id)
    {

        return Ok(await _db.Schedules.Where(s => s.LessonId == id).ToListAsync());
    }

    [HttpGet("idSubject/{id}")]
    public async Task<IActionResult> getIdSubject(int id)
    {

        return Ok(await _db.Schedules.Where(s => s.SubjectId == id).ToListAsync());
    }

    [HttpGet("dayOfWeek/{day}")]
    public async Task<IActionResult> getDayOfWeek(int day)
    {

        return Ok(await _db.Schedules.Where(s => s.DayOfWeek == day).ToListAsync());
    }

    [HttpPost("addNewSchedule")]
    public async Task<IActionResult> addNewSchedule([FromBody] CreateScheduleDto dto)
    {
        var newSchedule = new Schedule()
        {
            SubjectId = dto.SubjectId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Classroom = dto.Classroom,
            LessonType = dto.LessonType
        };

        _db.Schedules.Add(newSchedule);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("deleteSchedule/{id}")]
    public async Task<IActionResult> deleteSchedule(int id)
    {
        var schedule = await _db.Schedules.FindAsync(id);
        if (schedule == null) return NotFound("Такого Schedule в базе нет!");

        _db.Schedules.Remove(schedule);
        await _db.SaveChangesAsync();

        return Ok("Успешно удалено!");
    }

    [HttpPatch("changesSchedule/{id}")]
    public async Task<IActionResult> changeSchedule(int id, [FromBody] UpdateScheduleDto dto)
    {

        var schedule = await _db.Schedules.FindAsync(id);
        if (schedule == null) return NotFound("Такого Schedule в базе нет!");

        schedule.DayOfWeek = dto.DayOfWeek;
        schedule.StartTime = dto.StartTime;
        schedule.EndTime = dto.EndTime;
        schedule.Classroom = dto.Classroom;
        schedule.LessonType = dto.LessonType;

        await _db.SaveChangesAsync();
        return Ok(schedule);
    }
}