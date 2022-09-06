namespace OlxFilterWatcher.Services.Services;

public class FilterHandlerService : IFilterHandlerService
{
    private readonly record struct Filter(string FilterName, int Delay = 60_000);    
    private readonly Dictionary<Filter, Stopwatch> filterHandler;

    public FilterHandlerService()
    {
        filterHandler = new Dictionary<Filter, Stopwatch>();
    }

    public void AddFilter(string filter, int delayInMilliseconds = 60_000)
    {
        filterHandler.Add(new Filter(filter, delayInMilliseconds), new Stopwatch());
    }

    public void AddFilter(IEnumerable<string> filters, int delayInMilliseconds = 60_000)
    {
        foreach (var filter in filters)
        {
            AddFilter(filter, delayInMilliseconds);
        }
    }

    public void UpdateFilters(IEnumerable<string> filters)
    {
        foreach (var filter in filters)
        {
            if (!TryGetFilter(filter, out _))
            {
                filterHandler.Add(new Filter(filter), new Stopwatch());
            }
        }
    }

    public void StartTimer(string filter)
    {
        if (!TryGetFilterStopwatch(filter, out Stopwatch stopwatch))
            return;

        if (!stopwatch.IsRunning)
            stopwatch.Start();
    }
    public void ResetTimer(string filter)
    {
        if (!TryGetFilterStopwatch(filter, out Stopwatch stopwatch))
            return;

        stopwatch.Restart();
    }

    public bool IsFilterReady(string filter)
    {
        if (!TryGetFilter(filter, out Filter filterObj))
            return false;

        if (!TryGetFilterStopwatch(filter, out Stopwatch stopwatch))
            return false;

        if (!stopwatch.IsRunning)
            return false;

        if (stopwatch.ElapsedMilliseconds >= filterObj.Delay)
            return true;

        return false;
    }

    private bool TryGetFilter(string filter, out Filter outFilter)
    {
        outFilter = filterHandler.Where(x => x.Key.FilterName.Equals(filter)).Select(x => x.Key).FirstOrDefault();

        return outFilter != default;   
    }

    private bool TryGetFilterStopwatch(string filter, out Stopwatch outFilter)
    {
        outFilter = filterHandler.Where(x => x.Key.FilterName.Equals(filter)).Select(x => x.Value).FirstOrDefault();

        return outFilter != null;
    }
}
