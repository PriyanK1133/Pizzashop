using System.Reflection;
using Pizzashop.Entity.Data;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Web;

public static class DependencyInjection
{
    public static void RegisterServices(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PizzashopContext>(options =>
            options.UseNpgsql(connectionString)
        );

        RegisterImplementations(services, "Pizzashop.Repository");
        RegisterImplementations(services, "Pizzashop.Service");
    }

    private static void RegisterImplementations(
        IServiceCollection services,
        string assemblyName
    )
    {
        var assembly = Assembly.Load(assemblyName);
        var types = assembly.GetTypes();

        var interfaces = types.Where(t => t.IsInterface && t.Namespace is not null);
        var implementations = types.Where(t =>
            t.IsClass && !t.IsAbstract && t.Namespace is not null
        );

        foreach (var serviceInterface in interfaces)
        {
            var implementation = implementations.FirstOrDefault(implementation =>
                serviceInterface.Name[1..] == implementation.Name
            );
            if (implementation is not null)
            {
                services.AddScoped(serviceInterface, implementation);
            }
        }
    }
}
