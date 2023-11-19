using Domain.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Sockets;

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
        while (!stoppingToken.IsCancellationRequested)
        {
            BalancerExtention.ResetServers();

            for (int i = 0; i < _balancerSettings.Hosts.Count; i++)
            {
                var alive = await CheckHealthCare(i);
                if (alive)
                {
                    BalancerExtention.AddWorkingServers(i);
                }
            }

            await Task.Delay(_balancerSettings.Delay * 1000, stoppingToken);
        }
    }

    private async Task<bool> CheckHealthCare(int ind)
    {
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(_balancerSettings.Timeout);
            client.BaseAddress = new Uri(_balancerSettings.Hosts[ind].Server);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync("/_health");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (SocketException)
            {

            }
            catch (Exception)
            {

            }

            return false;
        }
    }
}
