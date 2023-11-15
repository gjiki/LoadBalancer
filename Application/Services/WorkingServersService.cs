﻿using Domain.Settings;
using LoadBalancer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;

namespace Application.Services;

public class WorkingServersService : BackgroundService
{
    private readonly BalancerSettings _balancerSettings;

    public WorkingServersService(IOptions<BalancerSettings> balancerSettings)
    {
        _balancerSettings = balancerSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ReevaluateServers(stoppingToken);
    }

    public async Task ReevaluateServers(CancellationToken stoppingToken)
    {
        BalancerExtention.ResetServers();

        for (int i = 0; i < _balancerSettings.Hosts.Count; i++)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_balancerSettings.Hosts[i].Server);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("_health");
                if (response.IsSuccessStatusCode)
                {
                    BalancerExtention.AddWorkingServers(i);
                }
            }
        }

        /*var ping = new Ping();
        for (int i = 0; i < _balancerSettings.Hosts.Count; i++)
        {
            var reply = ping.Send("google.com", 60 * 1000); // 1 minute time out (in ms)
            if (reply.Status == IPStatus.Success)
            {
                BalancerExtention.AddWorkingServers(i);
            }
        }*/
    }
}