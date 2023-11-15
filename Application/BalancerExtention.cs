using Domain.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Application;

public class BalancerExtention
{
    // private fields
    private static List<int> _requestCounts;
    private static ConcurrentDictionary<int, bool> _workingServers;
    private static int _totalRequests;
    private static int _lastServerInd;

    // injections
    private readonly BalancerSettings _balancerSettings;

    static BalancerExtention()
    {
        _requestCounts = new List<int>();
        _workingServers = new ConcurrentDictionary<int, bool>();
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
            _workingServers.TryAdd(i, false);
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

            //if (ind == _lastServerInd) break;
            if (!_workingServers[ind]) continue;

            decimal percentage = (decimal)_requestCounts[ind] / _totalRequests * 100.0m;
            if (percentage < _balancerSettings.Hosts[ind].Load)
            {
                return ind;
            }
        }
    }

    public void ResetValues()
    {
        _totalRequests = 0;
        for (int i = 0; i < _requestCounts.Count(); i++)
        {
            _requestCounts[i] = 0;
        }
    }

    public void IncreaseCount(int ind)
    {
        _requestCounts[ind]++;
        _totalRequests++;
    }

    public static void ResetServers()
    {
        foreach (KeyValuePair<int, bool> it in _workingServers)
        {
            _workingServers[it.Key] = false;
        }
    }

    public static void AddWorkingServers(int ind)
    {
        _workingServers[ind] = true;
    }

    public static ConcurrentDictionary<int, bool> GetWorkingServers()
    {
        return _workingServers;
    }
}
