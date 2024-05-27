using Exam.Tools.Tests.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Exam.Tools.Tests.Extensions;

public static class IServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }
    
    public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
    {
        var provider = services.BuildServiceProvider();
    
        using var scope = provider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context =  scopedServices.GetRequiredService<T>();
        context.Database.Migrate();
    }
    
    public static IServiceCollection AddTestMySqlContext<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        services.RemoveDbContext<TContext>();
    
        services.AddDbContext<TContext>(delegate (DbContextOptionsBuilder options)
        {
            options.UseMySql(MySQLContainerFixture.ConnectionString, new MySqlServerVersion("8.0.26"), options =>
            {
                options.EnableRetryOnFailure();
                options.UseMicrosoftJson();
            });
    
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    
        services.EnsureDbCreated<TContext>();
        return services;
    }

    public static void SubstituteService<T>(this IServiceCollection services) where T : class
    {
        var descriptor = services.Single(x => x.ServiceType == typeof(T));

        services.Remove(descriptor);
        
        var mock = Substitute.For<T>();

        services.AddSingleton(mock);
    }
}