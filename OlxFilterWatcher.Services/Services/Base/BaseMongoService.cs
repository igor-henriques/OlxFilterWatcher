namespace OlxFilterWatcher.Services.Services.Base;

public abstract class BaseMongoService
{
    protected readonly MongoClient _client;

    public BaseMongoService(string connectionString)
    {
        _client = new MongoClient(connectionString);
    }
}
