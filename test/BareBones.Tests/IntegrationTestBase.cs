using BareBones.Api.Persistence;
using BareBones.Client;
using BareBones.Tests.Fixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Extensions.Ordering;

namespace BareBones.Tests;

public class IntegrationTestBase : IAssemblyFixture<TestingWebApplicationFactory>
{
    protected readonly TestingWebApplicationFactory Factory;

    
    protected IntegrationTestBase(TestingWebApplicationFactory factory)
    {
        Factory = factory;

        // Cleanup other resources, eg. Cache
        
        using var scope = factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<BareBonesContext>().Database;

        // Cleanup MSSQL
        database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
        database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'DELETE FROM ?'");
        database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
        
        // Add necessary MSSQL init here, eg. enumerations
    }
    
    
    protected BareBonesClient CreateIntegrationTestClient()
    {
        var httpClient = Factory.CreateClient();
        return new BareBonesClient(httpClient.BaseAddress!.AbsoluteUri, httpClient);
    }
}