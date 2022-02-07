using System;
using System.Linq;
using System.Threading.Tasks;
using BareBones.Api.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BareBones.Tests.Fixture;

public class TestingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private DockerContainerInfo? _dockerContainerInfo;
    
      
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _dockerContainerInfo = DockerUtil
            .EnsureDockerStartedAndGetContainerInfo()
            .ConfigureAwait(true)
            .GetAwaiter()
            .GetResult();

        builder.ConfigureTestServices(services =>
        {
            var dbContextOptionsDesc =
                services.Single(d => d.ServiceType == typeof(DbContextOptions<BareBonesContext>));
            services.Remove(dbContextOptionsDesc);

            services.AddDbContext<BareBonesContext>(options =>
            {
                options.UseSqlServer(_dockerContainerInfo.GetSqlConnectionString());
            });
        });
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new async Task DisposeAsync()
    {
        var dockerContainerId = _dockerContainerInfo?.Id;
        if (dockerContainerId != null)
        {
            await DockerUtil.EnsureDockerStoppedAndRemovedAsync(dockerContainerId);    
        }
    }
}