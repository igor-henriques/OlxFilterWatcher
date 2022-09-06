namespace OlxFilterWatcher.Services.Interfaces
{
    public interface IOlxNotificationService
    {
        Task AddAsync(OlxNotificationDTO olxNotification, CancellationToken cancellationToken = default);
        Task<IEnumerable<OlxNotificationDTO>> GetAsync(CancellationToken cancellationToken = default);
        Task<OlxNotificationDTO> GetAsync(string URL, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(OlxNotificationDTO olxNotification, CancellationToken cancellationToken = default);
    }
}