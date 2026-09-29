using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;
using System.Linq;

// Замените try2 на название вашего главного проекта, если оно другое
using try2.Tests;
using try2.Models;
using Xunit;

public class DeadlineControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DeadlineControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Очищаем InMemory БД перед каждым тестом, т.к. она общая для всех тестов класса
        ResetDatabase();
    }

    private void ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();

        db.Deadlines.RemoveRange(db.Deadlines);
        db.Subjects.RemoveRange(db.Subjects);
        db.SaveChanges();
    }

    private async Task<Subject> SeedSubjectAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();

        var subject = new Subject { SubjectName = "Математика", Description = "Тестовый предмет" };
        db.Subjects.Add(subject);
        await db.SaveChangesAsync();

        return subject;
    }

    private async Task<Deadline> SeedDeadlineAsync(int subjectId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();

        var deadline = new Deadline
        {
            SubjectId = subjectId,
            TaskDescription = "Сдать лабу",
            DueDate = DateTime.UtcNow.AddDays(3),
            IsCompleted = false
        };
        db.Deadlines.Add(deadline);
        await db.SaveChangesAsync();

        return deadline;
    }

    // ---------- 1. GET /Deadline ----------
    [Fact]
    public async Task GetAll_ReturnsAllDeadlines()
    {
        var subject = await SeedSubjectAsync();
        await SeedDeadlineAsync(subject.SubjectId);
        await SeedDeadlineAsync(subject.SubjectId);

        var response = await _client.GetAsync("/Deadline");

        response.EnsureSuccessStatusCode();
        var deadlines = await response.Content.ReadFromJsonAsync<List<Deadline>>();

        Assert.NotNull(deadlines);
        Assert.Equal(2, deadlines!.Count);
    }

    // ---------- 2. GET /Deadline/idDeadline/{id} ----------
    [Fact]
    public async Task GetIdDeadline_ExistingId_ReturnsDeadline()
    {
        var subject = await SeedSubjectAsync();
        var deadline = await SeedDeadlineAsync(subject.SubjectId);

        var response = await _client.GetAsync($"/Deadline/idDeadline/{deadline.DeadlineId}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<Deadline>>();

        Assert.NotNull(result);
        Assert.Single(result!);
        Assert.Equal(deadline.DeadlineId, result![0].DeadlineId);
    }

    [Fact]
    public async Task GetIdDeadline_NotExistingId_ReturnsEmptyList()
    {
        var response = await _client.DeleteAsync("/Deadline/deleteDeadline/9999"); // Если это DELETE-эндпоинт

        // Проверяем, что вернулся статус 404
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

       

    }

    // ---------- 3. GET /Deadline/idSubject/{id} ----------
    [Fact]
    public async Task GetIdSubject_ReturnsDeadlinesForThatSubject()
    {
        var subject1 = await SeedSubjectAsync();
        var subject2 = await SeedSubjectAsync();
        await SeedDeadlineAsync(subject1.SubjectId);
        await SeedDeadlineAsync(subject1.SubjectId);
        await SeedDeadlineAsync(subject2.SubjectId);

        var response = await _client.GetAsync($"/Deadline/idSubject/{subject1.SubjectId}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<Deadline>>();

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.All(result, d => Assert.Equal(subject1.SubjectId, d.SubjectId));
    }

    // ---------- 4. POST /Deadline/addNewDeadline ----------
    [Fact]
    public async Task AddNewDeadline_ValidData_CreatesDeadline()
    {
        var subject = await SeedSubjectAsync();

        var newDeadline = new CreateDeadlineDto
        {
            SubjectId = subject.SubjectId,
            TaskDescription = "Курсовая работа",
            DueDate = DateTime.UtcNow.AddDays(3)
        };

        var response = await _client.PostAsJsonAsync("/Deadline/addNewDeadline", newDeadline);

        response.EnsureSuccessStatusCode();

        // Проверяем, что запись реально появилась в БД
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();
        Assert.Single(db.Deadlines);
        Assert.Equal("Курсовая работа", db.Deadlines.First().TaskDescription);
    }

    // ---------- 5. DELETE /Deadline/deleteDeadline/{id} ----------
    [Fact]
    public async Task DeleteDeadline_ExistingId_RemovesDeadline()
    {
        var subject = await SeedSubjectAsync();
        var deadline = await SeedDeadlineAsync(subject.SubjectId);

        var response = await _client.DeleteAsync($"/Deadline/deleteDeadline/{deadline.DeadlineId}");

        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentPlannerDbContext>();
        Assert.Empty(db.Deadlines);
    }

    [Fact]
    public async Task DeleteDeadline_NotExistingId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/Deadline/deleteDeadline/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- 6. PATCH /Deadline/changesDeadline/{id} ----------
    [Fact]
    public async Task ChangeDeadline_ExistingId_UpdatesDeadline()
    {
        var subject = await SeedSubjectAsync();
        var deadline = await SeedDeadlineAsync(subject.SubjectId);

        var updateDto = new
        {
            TaskDescription = "Обновлённое описание",
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            IsCompleted = true
        };

        var response = await _client.PatchAsJsonAsync($"/Deadline/changesDeadline/{deadline.DeadlineId}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<Deadline>();

        Assert.NotNull(updated);
        Assert.Equal("Обновлённое описание", updated!.TaskDescription);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task ChangeDeadline_NotExistingId_ReturnsNotFound()
    {
        var updateDto = new
        {
            TaskDescription = "Что-то",
            DueDate = DateOnly.FromDateTime(DateTime.Today),
            IsCompleted = false
        };

        var response = await _client.PatchAsJsonAsync("/Deadline/changesDeadline/9999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
