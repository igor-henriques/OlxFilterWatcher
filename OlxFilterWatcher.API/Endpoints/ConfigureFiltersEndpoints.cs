namespace OlxFilterWatcher.API.Endpoints;

public static class ConfigureFiltersEndpoints
{
    public static void MapFiltersEndpoints(this WebApplication app)
    {
        app.MapPost("/olx-watcher/v1/filters/subscribe", async (
            [FromBody] OlxFilterDTO filter, 
            [FromServices] IOlxFilterService olxFilterService,
            [FromServices] IValidator<OlxFilterDTO> validator,
            CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(filter, cancellationToken);

            if (!await olxFilterService.AddFilter(filter, cancellationToken))
                return Results.BadRequest("Erro ao criar filtro");

            Log.Information("Emails {emails} adicionados ao filtro {filter} com sucesso", string.Join(", ", filter.Emails), filter.Filter);

            return Results.NoContent();
        }).RequireAuthorization();

        app.MapPut("/olx-watcher/v1/filters/unsubscribe", async (
            [FromQuery] string filterId,
            [FromQuery] string email,
            [FromServices] IOlxFilterService olxFilterService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrEmpty(filterId))
                return Results.BadRequest("Filtro não encontrado");

            if (string.IsNullOrEmpty(email))
                return Results.BadRequest("E-mail inválido");

            if (!await olxFilterService.UnsubscribeEmail(filterId, email, cancellationToken))
                return Results.BadRequest("Erro ao excluir filtro");

            Log.Information("Email {emails} removidos do filtro {filter} com sucesso", email, filterId);

            return Results.NoContent();
        });
    }
}