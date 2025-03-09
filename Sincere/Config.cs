using Sincere;

namespace Microsoft.Extensions.DependencyInjection;

public static class Config
{
    public static IServiceCollection AddSincere(this IServiceCollection services)
    {
        services.AddScoped<ViewerContainerJsInterop>();

        // register debug service unconditionally
        services.AddSingleton<DebugLogger>();

        return services;
    }
}