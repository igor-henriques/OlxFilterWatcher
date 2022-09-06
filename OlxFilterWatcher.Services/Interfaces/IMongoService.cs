namespace OlxFilterWatcher.Services.Interfaces;

public interface IMongoService<T> where T : BaseMongoModel
{
    ValueTask AddManyAsync(IEnumerable<T> obj, CancellationToken cancellationToken = default);
    ValueTask AddOneAsync(T obj, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindManyAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null, CancellationToken cancellationToken = default);
    Task<T> FindOneAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default);
    Task<bool> FindAndUpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, CancellationToken cancellationToken = default);
    Task<IEnumerable<TField>> DistinctAsync<TField>(FieldDefinition<T, TField> field, FilterDefinition<T> filter, CancellationToken cancellationToken = default);
    Task<bool> AddIndex(TimeSpan TTL, FieldDefinition<T> field, CancellationToken cancellationToken = default);
    Task<T> FindById(string objectId, CancellationToken cancellationToken = default);
}