using Testcontainers.MySql;

namespace Exam.Tools.Tests.Containers;

public class MySQLContainerFixture : IDisposable
{
    public static string ConnectionString = null!;
    
    private readonly MySqlContainer _container;

    public MySQLContainerFixture()
    {
        _container = new MySqlBuilder().Build();
        _container.StartAsync().Wait();
        
        ConnectionString = _container.GetConnectionString();
        if (ConnectionString.Contains("host.docker.internal"))
        {
            ConnectionString = ConnectionString.Replace("host.docker.internal", "127.0.0.1");
        }

    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _container.StopAsync().Wait();
        }
    }
}