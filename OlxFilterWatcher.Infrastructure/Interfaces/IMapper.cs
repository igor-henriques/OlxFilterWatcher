namespace OlxFilterWatcher.Infrastructure.Interfaces;

public interface IMapper<TSource, TTarget> where TTarget : BaseMongoModel
{
    TSource Map(TTarget source);
    TTarget Map(TSource source);
    IEnumerable<TSource> Map(IEnumerable<TTarget> source);
    IEnumerable<TTarget> Map(IEnumerable<TSource> source);
}
