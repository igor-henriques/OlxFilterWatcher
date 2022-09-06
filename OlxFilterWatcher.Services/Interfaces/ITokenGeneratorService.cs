namespace OlxFilterWatcher.Services.Interfaces;

public interface ITokenGeneratorService
{
    JwtToken GenerateToken(UserAuth user = null);
}