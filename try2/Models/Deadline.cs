using System;
using System.Collections.Generic;

namespace try2.Models;

public partial class Deadline
{
    public int DeadlineId { get; set; }

    public int SubjectId { get; set; }

    public string TaskDescription { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public bool? IsCompleted { get; set; }

    public virtual Subject Subject { get; set; } = null!;
}
public class CreateDeadlineDto
{
    public int SubjectId { get; set; }

    public string TaskDescription { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public bool? IsCompleted { get; set; }
}
public  class UpdateDeadlineDto
{
    public string TaskDescription { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public bool? IsCompleted { get; set; }
}