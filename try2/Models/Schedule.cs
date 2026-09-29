using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace try2.Models;

public partial class Schedule
{
    public int LessonId { get; set; }

    public int SubjectId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? Classroom { get; set; }

    public string? LessonType { get; set; }

    public virtual Subject Subject { get; set; } = null!;
}
 

public class CreateScheduleDto
{
    [Required]
    public int SubjectId { get; set; }

    [Required]
    [Range(1, 7, ErrorMessage = "DayOfWeek должен быть от 1 (понедельник) до 7 (воскресенье)")]
    public int DayOfWeek { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    public string? Classroom { get; set; }

    public string? LessonType { get; set; }
}

public class UpdateScheduleDto
{
    [Required]
    [Range(1, 7, ErrorMessage = "DayOfWeek должен быть от 1 (понедельник) до 7 (воскресенье)")]
    public int DayOfWeek { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    public string? Classroom { get; set; }

    public string? LessonType { get; set; }
}