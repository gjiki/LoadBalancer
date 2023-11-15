namespace Domain.Services;

public interface IRequestRedirect
{
    Task<HttpResponseMessage> Redirect(string baseAddress, string url);
}
