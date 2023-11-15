using Application.Services;
using Domain.Services;
using LoadBalancer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static void AddLoadBalancerApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBackgroundJobsService, BackgroundJobsService>();
        services.AddSingleton<BalancerExtention>();
        services.AddHostedService<WorkingServersService>();
    }
}
