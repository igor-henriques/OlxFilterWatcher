namespace OlxFilterWatcher.Services.Interfaces;

public interface IOlxRentPostService
{
    ValueTask<bool> AnyAsync(string URL, CancellationToken cancellationToken = default);
    ValueTask AddAsync(OlxRentPostDTO olxPostDTO, CancellationToken cancellationToken = default);
    ValueTask<OlxRentPostDTO> GetAsync(string URL, CancellationToken cancellationToken = default);
}