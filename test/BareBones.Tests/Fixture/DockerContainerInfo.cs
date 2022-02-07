namespace BareBones.Tests.Fixture;

public class DockerContainerInfo
{
    public string Id { get; }
    
    public string Port { get; }


    public DockerContainerInfo(string id, string port)
    {
        Id = id;
        Port = port;
    }
    
    
    public string GetSqlConnectionString()
    {
        return $"Data Source=localhost,{Port};Integrated Security=False;User ID=SA;Password={DockerUtil.SQLSERVER_SA_PASSWORD}";
    }
}