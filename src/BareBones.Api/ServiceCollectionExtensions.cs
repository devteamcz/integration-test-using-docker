using BareBones.Api.Application;
using BareBones.Api.Domain;
using BareBones.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BareBones.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BareBonesContext>(
            options => options.UseSqlServer(
            configuration["Database:ConnectionString"]));

        services.AddScoped<IDocumentRepository, DocumentRepository>();
            
        return services;
    }
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<IDocumentService, DocumentService>();
    }
}