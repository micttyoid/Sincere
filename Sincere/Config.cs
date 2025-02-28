using Sincere;

namespace Microsoft.Extensions.DependencyInjection;

public static class Config
{
    public static IServiceCollection AddSincere(this IServiceCollection services)
    {
        // TODO: debug-specific service
        services.AddScoped<ViewerContainerJsInterop>();
        return services;
    }
}