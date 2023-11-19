using Application.Services;
using Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static void AddLoadBalancerApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRequestRedirect, RequestRedirect>();
        services.AddSingleton<BalancerExtention>();
        services.AddHostedService<WorkingServersService>();
        services.Configure<HostOptions>(hostOptions =>
        {
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
        });
    }
}
