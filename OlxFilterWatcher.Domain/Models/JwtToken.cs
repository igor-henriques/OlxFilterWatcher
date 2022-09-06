namespace OlxFilterWatcher.Domain.Models;

public record JwtToken
{
    public string Token { get; init; }
    public DateTime ExpiresAt { get; init; }
}
