namespace OlxFilterWatcher.Domain.Models.Base;

public abstract record BaseMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; private init; }
}
