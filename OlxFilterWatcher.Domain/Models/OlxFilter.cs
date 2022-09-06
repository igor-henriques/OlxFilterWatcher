namespace OlxFilterWatcher.Domain.Models;

public sealed record OlxFilter : BaseMongoModel
{
    public string Filter { get; init; }
    public List<string> Emails { get; init; }
}
