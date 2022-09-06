namespace OlxFilterWatcher.Domain.Models;

public record OlxGeneralPost : BaseMongoModel
{
    public string URL { get; init; }
    public string Category { get; init; }
    public string Title { get; init; }
    public decimal PostPrice { get; init; }
    public string Location { get; init; }
    public string ZipCode { get; init; }
    public DateTime TimePosted { get; init; }
    public List<string> Images { get; init; }
    public string FoundByFilter { get; init; }

    public OlxGeneralPost(string url, string title, decimal value, string location,
        DateTime timePosted, List<string> images, string zipCode, string foundByFilter)
    {
        URL = url;
        Title = title;
        PostPrice = value;
        Location = location;
        TimePosted = timePosted;
        Images = images;
        ZipCode = zipCode;
        FoundByFilter = foundByFilter;
    }

    public OlxGeneralPost() { }

    public Type GetPostType()
    {
        return typeof(OlxGeneralPost);
    }
}