namespace OlxFilterWatcher.Domain.Models;

public record WorkerSettings : BaseMongoModel
{
    public string Name { get; init; }
    public bool IsActive { get; init; }
}
