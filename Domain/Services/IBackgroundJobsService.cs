namespace Domain.Services;

public interface IBackgroundJobsService
{
    Task ReevaluateServers();
}