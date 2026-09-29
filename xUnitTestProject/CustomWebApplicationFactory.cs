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
using try2;
using try2.Models;
using Xunit;
 

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Полностью вырезаем ВСЕ регистрации, связанные с вашим DbContext
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.IsGenericType &&
                            d.ServiceType.GetGenericArguments().Contains(typeof(StudentPlannerDbContext))
                            || d.ServiceType == typeof(StudentPlannerDbContext))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // 2. Регистрируем InMemory базу данных с чистого листа
            services.AddDbContext<StudentPlannerDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });
        });
    }
}
