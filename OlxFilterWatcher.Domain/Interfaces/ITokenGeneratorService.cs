namespace OlxFilterWatcher.Domain.Interfaces;

public interface ITokenGeneratorService
{
    JwtToken GenerateToken(UserAuth user = null);
}