using Domain.Settings;
using Microsoft.Extensions.Options;

namespace LoadBalancer;

public class BalancerExtention
{
    // private fields
    private static List<int> _requestCounts;
    private static HashSet<int> _workingServers;
    private static int _totalRequests;
    private static int _lastServerInd;

    // injections
    private readonly BalancerSettings _balancerSettings;

    static BalancerExtention()
    {
        _requestCounts = new List<int>();
        _workingServers = new HashSet<int>();
        _totalRequests = 0;
        _lastServerInd = -1;
    }

    public BalancerExtention(IOptions<BalancerSettings> balancerSettings)
    {
        _balancerSettings = balancerSettings.Value;
        InitBalancerRequests();
    }

    private void InitBalancerRequests()
    {
        for (int i = 0; i < _balancerSettings.Hosts.Count(); i++)
        {
            _requestCounts.Add(0);
        }
    }

    public int NextServer()
    {
        if (_workingServers.Count == 0) return -1;

        int ind = _lastServerInd;
        while (true)
        {
            if (ind == _requestCounts.Count - 1)
            {
                ind = 0;
            }
            else
            {
                ind++;
            }

            if (ind == _lastServerInd) break;
            if (!_workingServers.Contains(ind)) continue;

            decimal percentage = ((decimal)_requestCounts[ind] / _totalRequests) * 100.0m;
            if (percentage < _balancerSettings.Hosts[ind].Load)
            {
                return ind;
            }
        }

        return _workingServers.First();
    }

    public static void ResetValues()
    {
        _totalRequests = 0;
        for (int i = 0; i < _requestCounts.Count(); i++)
        {
            _requestCounts[i] = 0;
        }
    }

    public static void ResetServers()
    {
        _workingServers.Clear();
    }

    public static void AddWorkingServers(int ind)
    {
        _workingServers.Add(ind);
    }
}
