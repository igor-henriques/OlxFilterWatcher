namespace OlxFilterWatcher.Domain.Models;

public sealed record OlxNotification : BaseMongoModel
{
    public string Filter { get; init; }
    public string URL { get; init; }
    public List<EmailNotifyCheck> NotifyChecks { get; init; }
}