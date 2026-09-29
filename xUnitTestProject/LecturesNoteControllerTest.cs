using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using try2.Models;
 
using Xunit;

namespace try2.Tests;

public class LecturesNoteControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public LecturesNoteControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        ResetDatabase();
    }

    private void ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();

        db.LecturesNotes.RemoveRange(db.LecturesNotes);
        db.Subjects.RemoveRange(db.Subjects);
        db.SaveChanges();
    }

    private async Task<Subject> SeedSubjectAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();

        var subject = new Subject { SubjectName = "Физика", Description = "Тестовый предмет" };
        db.Subjects.Add(subject);
        await db.SaveChangesAsync();

        return subject;
    }

    private async Task<LecturesNote> SeedLecturesNoteAsync(int subjectId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();

        var note = new LecturesNote
        {
            SubjectId = subjectId,
            LectureTitle = "Введение в механику",
            LectureDate = DateOnly.FromDateTime(DateTime.Today),
            FileLink = "https://example.com/lecture1.pdf"
        };
        db.LecturesNotes.Add(note);
        await db.SaveChangesAsync();

        return note;
    }

    // ---------- 1. GET /LecturesNote ----------
    [Fact]
    public async Task GetAll_ReturnsAllNotes()
    {
        var subject = await SeedSubjectAsync();
        await SeedLecturesNoteAsync(subject.SubjectId);
        await SeedLecturesNoteAsync(subject.SubjectId);

        var response = await _client.GetAsync("/LecturesNote");

        response.EnsureSuccessStatusCode();
        var notes = await response.Content.ReadFromJsonAsync<List<LecturesNote>>();

        Assert.NotNull(notes);
        Assert.Equal(2, notes!.Count);
    }

    // ---------- 2. GET /LecturesNote/idNote/{id} ----------
    [Fact]
    public async Task GetIdNote_ExistingId_ReturnsNote()
    {
        var subject = await SeedSubjectAsync();
        var note = await SeedLecturesNoteAsync(subject.SubjectId);

        var response = await _client.GetAsync($"/LecturesNote/idNote/{note.NoteId}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<LecturesNote>>();

        Assert.NotNull(result);
        Assert.Single(result!);
        Assert.Equal(note.NoteId, result![0].NoteId);
    }

    [Fact]
    public async Task GetIdNote_NotExistingId_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/LecturesNote/idNote/9999");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<LecturesNote>>();

        Assert.NotNull(result);
        Assert.Empty(result!);
    }

    // ---------- 3. GET /LecturesNote/idSubject/{id} ----------
    [Fact]
    public async Task GetIdSubject_ReturnsNotesForThatSubject()
    {
        var subject1 = await SeedSubjectAsync();
        var subject2 = await SeedSubjectAsync();
        await SeedLecturesNoteAsync(subject1.SubjectId);
        await SeedLecturesNoteAsync(subject1.SubjectId);
        await SeedLecturesNoteAsync(subject2.SubjectId);

        var response = await _client.GetAsync($"/LecturesNote/idSubject/{subject1.SubjectId}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<LecturesNote>>();

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.All(result, n => Assert.Equal(subject1.SubjectId, n.SubjectId));
    }

    // ---------- 4. POST /LecturesNote/addNewLecturesNote ----------
    [Fact]
    public async Task AddNewLecturesNote_ValidData_CreatesNote()
    {
        var subject = await SeedSubjectAsync();

        var newNote = new CreateLecturesNoteDto
        {
            SubjectId = subject.SubjectId,
            LectureTitle = "Термодинамика",
            LectureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            FileLink = "https://example.com/thermo.pdf"
        };

        var response = await _client.PostAsJsonAsync("/LecturesNote/addNewLecturesNote", newNote);

        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();
        Assert.Single(db.LecturesNotes);
        Assert.Equal("Термодинамика", db.LecturesNotes.First().LectureTitle);
    }

    // ---------- 5. DELETE /LecturesNote/deleteLecturesNote/{id} ----------
    [Fact]
    public async Task DeleteLecturesNote_ExistingId_RemovesNote()
    {
        var subject = await SeedSubjectAsync();
        var note = await SeedLecturesNoteAsync(subject.SubjectId);

        var response = await _client.DeleteAsync($"/LecturesNote/deleteLecturesNote/{note.NoteId}");

        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();
        Assert.Empty(db.LecturesNotes);
    }

    [Fact]
    public async Task DeleteLecturesNote_NotExistingId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/LecturesNote/deleteLecturesNote/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- 6. PATCH /LecturesNote/changesLecturesNote/{id} ----------
    [Fact]
    public async Task ChangeLecturesNote_ExistingId_UpdatesNote()
    {
        var subject = await SeedSubjectAsync();
        var note = await SeedLecturesNoteAsync(subject.SubjectId);

        var updateDto = new
        {
            LectureTitle = "Обновлённая лекция",
            LectureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            FileLink = "https://example.com/updated.pdf"
        };

        var response = await _client.PatchAsJsonAsync($"/LecturesNote/changesLecturesNote/{note.NoteId}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<LecturesNote>();

        Assert.NotNull(updated);
        Assert.Equal("Обновлённая лекция", updated!.LectureTitle);
        Assert.Equal("https://example.com/updated.pdf", updated.FileLink);
    }

    [Fact]
    public async Task ChangeLecturesNote_NotExistingId_ReturnsNotFound()
    {
        var updateDto = new
        {
            LectureTitle = "Что-то",
            LectureDate = DateOnly.FromDateTime(DateTime.Today),
            FileLink = "https://example.com/x.pdf"
        };

        var response = await _client.PatchAsJsonAsync("/LecturesNote/changesLecturesNote/9999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}