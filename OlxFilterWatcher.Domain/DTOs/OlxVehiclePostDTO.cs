namespace OlxFilterWatcher.Domain.DTOs;
public record OlxVehiclePostDTO : OlxGeneralPostDTO
{    
    public string Year { get; init; }
    public string KmCount { get; init; }
    public string Transmission { get; init; }

    public OlxVehiclePostDTO() { }
    public OlxVehiclePostDTO(string url, string title, decimal postPrice, string location, DateTime timePosted, List<string> images,
        string zipCode, string foundByFilter, string year, string kmCount, string transmission)
        : base(url, title, postPrice, location, zipCode, timePosted, images, foundByFilter)
    {
        Year = year;
        KmCount = kmCount;
        Transmission = transmission;
    }

    public new Type GetPostType()
    {
        return typeof(OlxVehiclePostDTO);
    }
}
