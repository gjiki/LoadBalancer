namespace Domain.Settings;

public class BackgroundJobsSettings
{
    public IEnumerable<JobSetting> TaskSettings { get; set; }
}

public class JobSetting
{
    public string Name { get; set; }
    public int Delay { get; set; }
}
