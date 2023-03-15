namespace OlxFilterWatcher.Domain.Interfaces;

public interface IFilterHandlerService
{
    void AddFilter(IEnumerable<string> filters, int delayInMilliseconds = 60_000);
    void AddFilter(string filter, int delayInMilliseconds = 60_000);
    bool IsFilterReady(string filter);
    void ResetTimer(string filter);
    void StartTimer(string filter);
    void UpdateFilters(IEnumerable<string> filters);
}