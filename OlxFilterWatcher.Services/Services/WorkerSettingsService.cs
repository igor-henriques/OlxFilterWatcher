namespace OlxFilterWatcher.Services.Services;

public class WorkerSettingsService : IWorkerSettingsService
{
    private readonly IMongoService<WorkerSettings> collection;

    public WorkerSettingsService(IMongoService<WorkerSettings> collection)
    {
        this.collection = collection;
    }

    public async Task<bool> IsWorkerActive(string workerName, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<WorkerSettings>.Filter.Eq(x => x.Name, workerName) & Builders<WorkerSettings>.Filter.Eq(x => x.IsActive, true);

        return await collection.AnyAsync(searchFilter, cancellationToken);
    }

    public async ValueTask<bool> UpdateAsync(WorkerSettingsDTO workerSettings, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<WorkerSettings>.Filter.Eq(x => x.Name, workerSettings.Name);

        return await collection.FindAndUpdateAsync(searchFilter, Builders<WorkerSettings>.Update.Set(rec => rec.IsActive, workerSettings.IsActive), cancellationToken);
    }
}