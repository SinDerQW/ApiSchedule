using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace try2.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Deadline> Deadlines { get; set; } = new List<Deadline>();

    public virtual ICollection<LecturesNote> LecturesNotes { get; set; } = new List<LecturesNote>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
public class CreateSubjectDto
{
    [Required(ErrorMessage = "Название предмета обязательно")]
    [StringLength(150, MinimumLength = 2)]
    public string SubjectName { get; set; } = null!;

    public string? Description { get; set; }
}

public class UpdateSubjectDto
{
    [Required(ErrorMessage = "Название предмета обязательно")]
    [StringLength(150, MinimumLength = 2)]
    public string SubjectName { get; set; } = null!;

    public string? Description { get; set; }
}