using Domain.Services;
using Domain.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application;

public class LoadBalancingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BalancerExtention _balancerExtention;
    private readonly BalancerSettings _balancerSettings;

    public LoadBalancingMiddleware(RequestDelegate next, BalancerExtention balancerExtention, IOptions<BalancerSettings> balancerSettings)
    {
        _next = next;
        _balancerExtention = balancerExtention;
        _balancerSettings = balancerSettings.Value;
    }

    public async Task InvokeAsync(HttpContext httpContext, IRequestRedirect svc)
    {
        var url = httpContext.Request.Path;
        var nextServerInd = _balancerExtention.NextServer();
        _balancerExtention.IncreaseCount(nextServerInd);
        await svc.Redirect(string.Concat(url, _balancerSettings.Hosts[nextServerInd].Server));
        await _next(httpContext);
    }
}

public static class LoadBalancingMiddlewareExtensions
{
    public static IApplicationBuilder UseLoadBalancingMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoadBalancingMiddleware>();
    }
}