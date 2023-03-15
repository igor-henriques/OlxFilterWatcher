namespace OlxFilterWatcher.Services.Services;

public sealed class OlxFilterService : IOlxFilterService
{
    private DateTime LastTimeUpdated;
    private readonly IMongoService<OlxFilter> mongoService;
    private readonly IMapper<OlxFilterDTO, OlxFilter> mapper;
    private readonly Dictionary<string, List<string>> _filters;
    public List<string> Filters { get => _filters.Select(x => x.Key).ToList(); }

    public OlxFilterService(IMongoService<OlxFilter> mongoService, IMapper<OlxFilterDTO, OlxFilter> mapper)
    {
        _filters = new Dictionary<string, List<string>>();
        this.mongoService = mongoService;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<string>> GetFiltersAsync(CancellationToken cancellationToken = default)
    {        
        if (DateTime.Now - LastTimeUpdated < TimeSpan.FromMinutes(5) & _filters.Any())
            return _filters.Select(x => x.Key);

        var searchFilter = Builders<OlxFilter>.Filter.Exists(x => x.Filter) & Builders<OlxFilter>.Filter.SizeGt(x => x.Emails, 0);

        var response = mapper.Map(await mongoService.FindManyAsync(searchFilter, null, cancellationToken));

        _filters.Clear();

        foreach (var item in response)
        {
            AddInMemoryFilter(item);
        }        

        return _filters.Select(x => x.Key);
    }

    public async Task<string> GetFilterUrlByIdAsync(string objectId, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxFilter>.Filter.Eq(x => x._id, objectId);

        var response = await mongoService.FindOneAsync(searchFilter, null, cancellationToken);

        return response.Filter;
    }

    public async Task<IEnumerable<OlxFilterDTO>> GetOlxFiltersAsync(CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxFilter>.Filter.Exists(x => x.Filter);

        var response = mapper.Map(await mongoService.FindManyAsync(searchFilter, null, cancellationToken));

        return response;
    }

    public async Task<IEnumerable<string>> GetEmailsByFilterAsync(string filter, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxFilter>.Filter.Eq(x => x.Filter, filter);

        var response = await mongoService.FindOneAsync(searchFilter, null, cancellationToken);

        return response.Emails;
    }

    public async Task<bool> AddFilter(OlxFilterDTO filter, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxFilter>.Filter.Eq(x => x.Filter, filter.Filter);

        var filterFromDb = mapper.Map(await mongoService.FindOneAsync(searchFilter, null, cancellationToken));

        if (filterFromDb is not null)
        {
            if (filterFromDb.Emails.Any(x => filter.Emails.Contains(x)))
                return false;

            filterFromDb.Emails.AddRange(filter.Emails);

            AddInMemoryFilter(filterFromDb);

            return await this.UpdateFilter(filterFromDb, cancellationToken);
        }

        AddInMemoryFilter(filter);

        await mongoService.AddOneAsync(mapper.Map(filter), cancellationToken);

        return true;
    }

    public async Task<bool> RemoveEmailsFromFilter(OlxFilterDTO filter, CancellationToken cancellationToken = default)
    {
        if (!_filters.Any())
            await GetFiltersAsync(cancellationToken);

        if (!_filters.TryGetValue(filter.Filter, out List<string> emails))
        {
            return false;
        }

        RemoveInMemoryFilter(filter);

        var searchFilter = Builders<OlxFilter>.Filter.Exists(x => x.Filter);

        var filterFromDb = await mongoService.FindOneAsync(searchFilter, null, cancellationToken);

        filter.Emails.ForEach(email => filterFromDb.Emails.Remove(email));

        var response = await this.UpdateFilter(filter with { Emails = filterFromDb.Emails }, cancellationToken);

        return response;
    }

    public async Task<bool> UnsubscribeEmail(string filterId, string email, CancellationToken cancellationToken = default)
    {
        var olxFilter = mapper.Map(await GetFilterById(filterId, cancellationToken));

        olxFilter.Emails.Remove(email);

        return await UpdateFilter(olxFilter, cancellationToken);
    }

    public async Task<bool> UnsubscribeEmailByUrl(string filterURL, string email, CancellationToken cancellationToken = default)
    {
        var olxFilter = mapper.Map(await GetFilterByUrl(filterURL, cancellationToken));

        olxFilter.Emails.Remove(email);

        return await UpdateFilter(olxFilter, cancellationToken);
    }

    public async Task<OlxFilter> GetFilterById(string filterId, CancellationToken cancellationToken = default)
    {
        return await mongoService.FindById(filterId, cancellationToken);
    }

    public async Task<OlxFilter> GetFilterByUrl(string filterUrl, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxFilter>.Filter.Eq(x => x.Filter, filterUrl);

        return await mongoService.FindOneAsync(searchFilter, null, cancellationToken);
    }

    public async Task<bool> UpdateFilter(OlxFilterDTO filter, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxFilter>.Filter.Eq(x => x.Filter, filter.Filter);

        var response = await mongoService.FindAndUpdateAsync(
                            searchFilter,
                            Builders<OlxFilter>.Update.Set(rec => rec.Emails, filter.Emails),
                            cancellationToken);

        return response;
    }

    private void AddInMemoryFilter(OlxFilterDTO filter)
    {
        if (!_filters.TryGetValue(filter.Filter, out List<string> emails))
        {
            _filters.Add(filter.Filter, emails = filter.Emails);
        }

        _filters[filter.Filter] = emails;

        LastTimeUpdated = DateTime.Now;
    }

    private void RemoveInMemoryFilter(OlxFilterDTO filter)
    {
        if (!_filters.TryGetValue(filter.Filter, out List<string> emails))
        {
            return;
        }

        filter.Emails.ForEach(email => emails.Remove(email));

        _filters[filter.Filter] = emails;
    }
}
