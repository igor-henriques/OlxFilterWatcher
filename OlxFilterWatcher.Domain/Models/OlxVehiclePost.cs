namespace OlxFilterWatcher.Domain.Models;

[BsonIgnoreExtraElements]
public sealed record OlxVehiclePost : OlxGeneralPost
{
    public string Year { get; init; }
    public string KmCount { get; init; }
    public string Transmission { get; init; }

    public OlxVehiclePost(string url, string title, decimal value, string location, DateTime timePosted,
        List<string> images, string zipCode, string foundByFilter, string year, string kmCount, string transmission)
        : base(url, title, value, location, timePosted, images, zipCode, foundByFilter)
    {
        Year = year;
        KmCount = kmCount;
        Transmission = transmission;
    }

    public OlxVehiclePost() { }
}
