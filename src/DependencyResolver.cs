using Librarium.Librarium.Application.Interfaces;
using Librarium.Librarium.Application.Services;
using Librarium.Librarium.Data;
using Librarium.Librarium.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Librarium;

public static class DependencyResolver
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.RegisterDbContext();
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ILibraryService, LibraryService>();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<ILibraryRepository, LibraryRepository>();
    }

    private static void RegisterDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDb")));
    }
}
