using Microsoft.EntityFrameworkCore;
using try2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

 

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<StudentPlannerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Defualt")));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();

}
else
{
    app.UseExceptionHandler("/Error");
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
