namespace OlxFilterWatcher.Services.Services;

public class OlxNotificationService : IOlxNotificationService
{
    private readonly IMongoService<OlxNotification> collection;
    private readonly IMapper<OlxNotificationDTO, OlxNotification> mapper;

    public OlxNotificationService(IMongoService<OlxNotification> collection, IMapper<OlxNotificationDTO, OlxNotification> mapper)
    {
        this.collection = collection;
        this.mapper = mapper;
    }

    public async Task AddAsync(OlxNotificationDTO olxNotification, CancellationToken cancellationToken = default)
    {
        if (olxNotification == null)
            return;

        if (await GetAsync(olxNotification.URL, cancellationToken) != null)
            await UpdateAsync(olxNotification, cancellationToken);

        await collection.AddOneAsync(mapper.Map(olxNotification), cancellationToken);
    }

    public async Task<IEnumerable<OlxNotificationDTO>> GetAsync(CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxNotification>.Filter.ElemMatch(x => x.NotifyChecks, 
            Builders<EmailNotifyCheck>.Filter.Eq(y => y.IsNotified, false));

        var response = mapper.Map(await collection.FindManyAsync(searchFilter, null, cancellationToken));

        return response;
    }

    public async Task<OlxNotificationDTO> GetAsync(string URL, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxNotification>.Filter.Eq(x => x.URL, URL);

        var response = mapper.Map(await collection.FindOneAsync(searchFilter, null, cancellationToken));

        return response;
    }

    public async Task<bool> UpdateAsync(OlxNotificationDTO olxNotification, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<OlxNotification>.Filter.Eq(x => x.URL, olxNotification.URL);

        var response = await collection.FindAndUpdateAsync(
                            searchFilter,
                            Builders<OlxNotification>.Update.Set(rec => rec.NotifyChecks, olxNotification.NotifyChecks),
                            cancellationToken);

        return response;
    }
}
