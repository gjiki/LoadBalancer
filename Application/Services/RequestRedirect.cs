using Domain.Services;
using Domain.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Application.Services;

public class RequestRedirect : IRequestRedirect
{
    private readonly BalancerSettings _balancerSettings;

    public RequestRedirect(IOptions<BalancerSettings> balancerSettings)
    {
        _balancerSettings = balancerSettings.Value;
    }

    public async Task<HttpResponseMessage> Redirect(string baseAddress, string url)
    {
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(_balancerSettings.Timeout);
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url);
            return response;
        }
    }
}
