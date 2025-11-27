using System.Reflection;
using K9.SharedKernel;

namespace K9.Bootstrapper.Extensions;

public static class ModuleExtensions
{
    private static readonly List<IModule> RegisteredModules = new();

    public static IServiceCollection RegisterModules(this IServiceCollection services, IConfiguration configuration)
    {
        var path = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;

        var modules = Directory
            .GetFiles(path, "K9.Modules.*.dll")
            .Select(Assembly.LoadFrom)
            .SelectMany(asm => asm.GetTypes())
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();

        foreach (var module in modules)
        {
            module.RegisterModule(services, configuration);
            RegisteredModules.Add(module);
        }

        return services;
    }

    public static WebApplication MapModuleEndpoints(this WebApplication app)
    {
        foreach (var module in RegisteredModules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }
}