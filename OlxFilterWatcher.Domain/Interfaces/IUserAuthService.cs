namespace OlxFilterWatcher.Domain.Interfaces;

public interface IUserAuthService
{
    Task<UserAuth> GetUser(UserAuthDTO user, CancellationToken cancellationToken = default);
    Task<bool> CheckUserExists(UserAuthDTO user, CancellationToken cancellationToken = default);    
}