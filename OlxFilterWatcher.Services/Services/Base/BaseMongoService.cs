namespace OlxFilterWatcher.Services.Services.Base;

public abstract class BaseMongoService
{
    protected readonly MongoClient client;

    public BaseMongoService(string connectionString)
    {
        client = new MongoClient(connectionString);
    }
}
