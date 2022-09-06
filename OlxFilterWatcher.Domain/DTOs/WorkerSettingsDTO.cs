namespace OlxFilterWatcher.Domain.DTOs;

public record WorkerSettingsDTO
{
    public string Name { get; init; }
    public bool IsActive { get; init; }
}
