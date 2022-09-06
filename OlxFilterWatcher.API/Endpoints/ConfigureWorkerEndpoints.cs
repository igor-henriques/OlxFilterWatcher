namespace OlxFilterWatcher.API.Endpoints;

public static class ConfigureWorkerEndpoints
{
    public static void MapWorkerEndpoints(this WebApplication app)
    {
        app.MapPut("/olx-watcher/v1/workers/update", async (
            [FromBody] WorkerSettingsDTO workerSettings,
            [FromServices] IWorkerSettingsService olxFilterService,
            CancellationToken cancellationToken) =>
        {
            if (await olxFilterService.UpdateAsync(workerSettings))
                return Results.NoContent();

            return Results.BadRequest();
        }).RequireAuthorization();
    }
}
