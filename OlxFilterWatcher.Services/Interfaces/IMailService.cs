namespace OlxFilterWatcher.Services.Interfaces;

public interface IMailService
{
    Task<bool> SendEmailAsync(string htmlString, IEnumerable<string> emails, string subject, CancellationToken cancellationToken = default);
}