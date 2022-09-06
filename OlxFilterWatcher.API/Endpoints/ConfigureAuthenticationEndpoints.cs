namespace OlxFilterWatcher.API.Endpoints;

public static class ConfigureAuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/olx-watcher/v1/auth/authenticate", async (
            [FromBody] UserAuthDTO credential,
            [FromServices] ITokenGeneratorService tokenService,
            [FromServices] IUserAuthService userService,
            [FromServices] IValidator<UserAuthDTO> validator,
            CancellationToken cancellationToken) =>
        {
            validator.ValidateAndThrow(credential);

            var user = await userService.GetUser(credential, cancellationToken);

            if (user is null)
                return Results.Forbid();

            var jwtToken = tokenService.GenerateToken(user);

            return Results.Ok(jwtToken);
        });
    }
}
