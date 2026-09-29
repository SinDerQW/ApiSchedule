using System;
using System.Collections.Generic;

namespace try2.Models;

public partial class LecturesNote
{
    public int NoteId { get; set; }

    public int SubjectId { get; set; }

    public string LectureTitle { get; set; } = null!;

    public DateOnly? LectureDate { get; set; }

    public string FileLink { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}


public class CreateLecturesNoteDto
{
    public int SubjectId { get; set; }

    public string LectureTitle { get; set; } = null!;

    public DateOnly? LectureDate { get; set; }

    public string FileLink { get; set; } = null!;
}

public class UpdateLecturesNoteDto
{
    public string LectureTitle { get; set; } = null!;

    public DateOnly? LectureDate { get; set; }

    public string FileLink { get; set; } = null!;
}