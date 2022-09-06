namespace OlxFilterWatcher.Domain.DTOs;

public record OlxFilterDTO
{
    public string Filter { get; init; }
    public List<string> Emails { get; init; }
}
