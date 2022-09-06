namespace OlxFilterWatcher.Domain.DTOs;

public record UserAuthDTO
{
    public string Email { get; init; }
    public string Password { get; init; }
}
