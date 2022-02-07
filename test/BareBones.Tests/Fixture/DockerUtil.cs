using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Data.SqlClient;

namespace BareBones.Tests.Fixture;

// Stolen from https://wrapt.dev/blog/integration-tests-using-sql-server-db-in-docker
public static class DockerUtil
{
    public const string CONTAINER_NAME_PREFIX = "IntegrationTestUsingDocker";
        
    public const string SQLSERVER_SA_PASSWORD = "yourStrong(!)Password";
    public const string SQLSERVER_IMAGE = "mcr.microsoft.com/mssql/server";
    public const string SQLSERVER_IMAGE_TAG = "2019-latest";

    
    public static async Task<DockerContainerInfo> EnsureDockerStartedAndGetContainerInfo()
    {
        await CleanupRunningContainers();
        var dockerClient = GetDockerClient();

        var sqlPort = GetFreePort();
            
        var sqlResp = await SetupDockerSqlDb(dockerClient, sqlPort);

        return new DockerContainerInfo(sqlResp!.ID, sqlPort);
    }
        
    private static async Task<CreateContainerResponse?> CreateContainerAsync(DockerClient dockerClient, string image,
        string tag, CreateContainerParameters parameters)
    {
        // This call ensures that the latest SQL Server Docker image is pulled
        await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
        {
            FromImage = $"{image}:{tag}"
        }, null, new Progress<JSONMessage>());

        var container = await dockerClient
            .Containers
            .CreateContainerAsync(parameters);

        await dockerClient
            .Containers
            .StartContainerAsync(container.ID, new ContainerStartParameters());

        return container;
    }

    private static async Task<CreateContainerResponse?> SetupDockerSqlDb(DockerClient dockerClient, string freePort)
    {
        var containerName = $"{CONTAINER_NAME_PREFIX}-Sql-{Guid.NewGuid()}";
            
        var sqlParameters = new CreateContainerParameters
        {
            Name = containerName,
            Image = $"{SQLSERVER_IMAGE}:{SQLSERVER_IMAGE_TAG}",
            Env = new List<string>
            {
                "ACCEPT_EULA=Y",
                $"SA_PASSWORD={SQLSERVER_SA_PASSWORD}"
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {
                        "1433/tcp",
                        new PortBinding[]
                        {
                            new PortBinding
                            {
                                HostPort = freePort
                            }
                        }
                    }
                }
            }
        };

        var res = await CreateContainerAsync(dockerClient, SQLSERVER_IMAGE, SQLSERVER_IMAGE_TAG, sqlParameters);
            
        await WaitUntilDatabaseAvailableAsync(freePort);

        return res;
    }

    public static async Task EnsureDockerStoppedAndRemovedAsync(string dockerContainerId)
    {
        var dockerClient = GetDockerClient();
        await dockerClient.Containers
            .StopContainerAsync(dockerContainerId, new ContainerStopParameters());
        await dockerClient.Containers
            .RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters());
    }

    private static DockerClient GetDockerClient()
    {
        var dockerUri = IsRunningOnWindows()
            ? "npipe://./pipe/docker_engine"
            : "unix:///var/run/docker.sock";
        using var dockerClientConfiguration = new DockerClientConfiguration(new Uri(dockerUri));

        return dockerClientConfiguration.CreateClient();
    }

    private static async Task CleanupRunningContainers()
    {
        var dockerClient = GetDockerClient();

        var runningContainers = await dockerClient.Containers
            .ListContainersAsync(new ContainersListParameters());

        foreach (var runningContainer in runningContainers.Where(cont =>
                     cont.Names.Any(n => n.Contains(CONTAINER_NAME_PREFIX))))
        {
            // Stopping all test containers that are older than one hour, they likely failed to cleanup
            if (runningContainer.Created < DateTime.UtcNow.AddHours(-1))
            {
                try
                {
                    await EnsureDockerStoppedAndRemovedAsync(runningContainer.ID);
                }
                catch
                {
                    // Ignoring failures to stop running containers
                }
            }
        }
    }

    private static async Task WaitUntilDatabaseAvailableAsync(string databasePort)
    {
        var start = DateTime.UtcNow;
        const int maxWaitTimeSeconds = 60;
        var connectionEstablished = false;
        while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
        {
            try
            {
                var sqlConnectionString =
                    $"Data Source=localhost,{databasePort};Integrated Security=False;User ID=SA;Password={SQLSERVER_SA_PASSWORD}";
                using var sqlConnection = new SqlConnection(sqlConnectionString);
                await sqlConnection.OpenAsync();
                connectionEstablished = true;
            }
            catch
            {
                // If opening the SQL connection fails, SQL Server is not ready yet
                await Task.Delay(500);
            }
        }

        if (!connectionEstablished)
        {
            throw new Exception(
                "Connection to the SQL docker database could not be established within 60 seconds.");
        }
    }

    private static string GetFreePort()
    {
        // Taken from https://stackoverflow.com/a/150974/4190785
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port.ToString();
    }

    private static bool IsRunningOnWindows()
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
    }
}