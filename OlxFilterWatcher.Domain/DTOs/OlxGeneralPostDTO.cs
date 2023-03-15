namespace OlxFilterWatcher.Domain.DTOs;

public record OlxGeneralPostDTO
{
    public string URL { get; init; }
    public string Title { get; init; }
    public decimal PostPrice { get; init; }
    public string Location { get; init; }
    public string ZipCode { get; init; }
    public DateTime TimePosted { get; init; }
    public List<string> Images { get; init; }
    public string FoundByFilter { get; init; }
    public DateTime BrazilianDateTimeOffset => TimePosted.AddHours(-3);

    public OlxGeneralPostDTO(string url, string title, decimal postPrice, string location, 
        string zipCode, DateTime timePosted, List<string> images, string foundByFilter)
    {
        URL = url;
        Title = title;
        PostPrice = postPrice;
        Location = location;
        ZipCode = zipCode;
        TimePosted = timePosted;
        Images = images;
        FoundByFilter = foundByFilter;
    }

    public OlxGeneralPostDTO() { }

    public virtual Type GetPostType()
    {
        return typeof(OlxGeneralPostDTO);
    }    
}
