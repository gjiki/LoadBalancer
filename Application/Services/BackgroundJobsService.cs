using Domain.Services;
using Domain.Settings;
using LoadBalancer;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace Application.Services;

public class BackgroundJobsService : IBackgroundJobsService
{
    private readonly BalancerSettings _balancerSettings;

    public BackgroundJobsService(IOptions<BalancerSettings> balancerSettings)
    {
        _balancerSettings = balancerSettings.Value;
    }

    public async Task ReevaluateServers()
    {
        BalancerExtention.ResetServers();
        var ping = new Ping();
        for (int i = 0; i < _balancerSettings.Hosts.Count; i++)
        {
            var reply = ping.Send(_balancerSettings.Hosts[i].Server, 60 * 1000); // 1 minute time out (in ms)
            if (reply.Status != IPStatus.Success)
            {
                BalancerExtention.AddWorkingServers(i);
            }
        }
    }
}
