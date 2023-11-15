namespace Domain.Settings;

public class BalancerSettings
{
    public List<Host> Hosts { get; set; }
}

public class Host
{
    public string Server { get; set; }
    public int Load { get; set; }
}