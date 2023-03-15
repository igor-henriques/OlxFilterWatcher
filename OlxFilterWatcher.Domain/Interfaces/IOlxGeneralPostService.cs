namespace OlxFilterWatcher.Domain.Interfaces;

public interface IOlxGeneralPostService
{
    ValueTask<bool> AnyAsync(string URL, CancellationToken cancellationToken = default);
    Task AddAsync(OlxGeneralPostDTO olxPostDTO, CancellationToken cancellationToken = default);
    ValueTask<OlxGeneralPostDTO> GetAsync(string URL, CancellationToken cancellationToken = default);
    ValueTask<string> GetLastPostUrlByFilterAsync(string filter, CancellationToken cancellationToken = default);
}