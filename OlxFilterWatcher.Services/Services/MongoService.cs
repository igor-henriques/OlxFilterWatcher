namespace OlxFilterWatcher.Services.Services;

public sealed class MongoService<T> : BaseMongoService, IMongoService<T> where T : BaseMongoModel
{
    private const string databaseName = "olxfilterwatcher";
    private readonly IMongoCollection<T> collection;

    public MongoService(string connectionString) : base(connectionString)
    {
        this.collection = base._client.GetDatabase(databaseName)
                                      .GetCollection<T>(GetCollectionName());
    }

    private static string GetCollectionName()
    {
        return typeof(T).Name switch
        {
            string typeName when typeName.Contains("Post", StringComparison.CurrentCultureIgnoreCase) => "olxpost",
            string typeName when typeName.Contains("Filter", StringComparison.CurrentCultureIgnoreCase) => "olxfilter",
            string typeName when typeName.Contains("Notification", StringComparison.CurrentCultureIgnoreCase) => "olxnotification",
            string typeName when typeName.Contains("Worker", StringComparison.CurrentCultureIgnoreCase) => "workersettings",
            _ => throw new ArgumentException($"{typeof(T).Name} isn't defined as a collection")
        };
    }

    public async Task<bool> AddIndex(TimeSpan TTL, FieldDefinition<T> field, CancellationToken cancellationToken = default)
    {
        var indexModel = new CreateIndexModel<T>(
            keys: Builders<T>.IndexKeys.Ascending(field),
            options: new CreateIndexOptions
            {
                ExpireAfter = TTL,
                Name = $"{field}Index"
            });

        return !string.IsNullOrEmpty(await collection.Indexes.CreateOneAsync(indexModel, null, cancellationToken));
    }

    public async ValueTask AddOneAsync(T obj, CancellationToken cancellationToken = default)
    {        
        if (obj is null)
            return;

        await collection.InsertOneAsync(obj, null, cancellationToken);
    }
    public async ValueTask AddManyAsync(IEnumerable<T> obj, CancellationToken cancellationToken = default)
    {
        if (obj is null)
            return;

        if (!obj.Any())
            return;

        await collection.InsertManyAsync(obj, null, cancellationToken);
    }

    public async Task DeleteManyAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default)
    {
        await collection.DeleteManyAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<T>> FindManyAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null, CancellationToken cancellationToken = default)
    {
        var response = await collection.FindAsync<T>(filter, findOptions, cancellationToken);

        return await response.ToListAsync(cancellationToken);
    }

    public async Task<T> FindOneAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null, CancellationToken cancellationToken = default)
    {
        var response = await collection.FindAsync<T>(filter, findOptions, cancellationToken);

        return await response.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TField>> DistinctAsync<TField>(FieldDefinition<T, TField> field, FilterDefinition<T> filter, CancellationToken cancellationToken = default)
    {
        var response = await collection.DistinctAsync<TField>(field, filter, null, cancellationToken);        

        return await response.ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default)
    {
        var response = await collection.FindAsync<T>(filter, null, cancellationToken);

        return await response.AnyAsync(cancellationToken);
    }

    public async Task<bool> FindAndUpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, CancellationToken cancellationToken = default)
    {
        var response = await collection.UpdateManyAsync(filter, update, null, cancellationToken);

        return response.ModifiedCount > 0;
    }

    public async Task<T> FindById(string objectId, CancellationToken cancellationToken = default)
    {
        var searchFilter = Builders<T>.Filter.Eq(x => x._id, objectId);

        var response = await collection.FindAsync(searchFilter, null, cancellationToken);

        return await response.FirstOrDefaultAsync(cancellationToken);
    }
}
