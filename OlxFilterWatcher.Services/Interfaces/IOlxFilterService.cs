namespace OlxFilterWatcher.Services.Interfaces;

public interface IOlxFilterService
{
    List<string> Filters { get; }
    Task<bool> UpdateFilter(OlxFilterDTO filter, CancellationToken cancellationToken = default);
    Task<bool> AddFilter(OlxFilterDTO filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetFiltersAsync(CancellationToken cancellationToken = default);
    Task<bool> RemoveEmailsFromFilter(OlxFilterDTO filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<OlxFilterDTO>> GetOlxFiltersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetEmailsByFilterAsync(string filter, CancellationToken cancellationToken = default);
    Task<bool> UnsubscribeEmail(string filterId, string email, CancellationToken cancellationToken = default);
    Task<bool> UnsubscribeEmailByUrl(string URL, string email, CancellationToken cancellationToken = default);
    Task<string> GetFilterUrlByIdAsync(string objectId, CancellationToken cancellationToken = default);
    Task<OlxFilter> GetFilterByUrl(string filterUrl, CancellationToken cancellationToken = default);
}