namespace OlxFilterWatcher.Domain.Interfaces
{
    public interface IWorkerSettingsService
    {
        Task<bool> IsWorkerActive(string workerName, CancellationToken cancellationToken = default);
        ValueTask<bool> UpdateAsync(WorkerSettingsDTO workerSettings, CancellationToken cancellationToken = default);
    }
}