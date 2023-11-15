namespace Domain.Services;

public interface IRequestRedirect
{
    Task<string> Redirect(string url);
}
