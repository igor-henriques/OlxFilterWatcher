namespace OlxFilterWatcher.Domain.DTOs;

public class OlxNotificationDTO
{
    public string Filter { get; init; }
    public string URL { get; init; }
    public List<EmailNotifyCheck> NotifyChecks { get; init; }
    private DateTime ExpiresAt { get => DateTime.Now.AddDays(2); }

    public void EmailsNotifiedCallback(IEnumerable<string> emails)
        =>  NotifyChecks.Where(x => emails.Contains(x.Email)).ToList().ForEach(x => x.IsNotified = true);
}
