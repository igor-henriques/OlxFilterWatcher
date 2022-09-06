namespace OlxFilterWatcher.Domain.DTOs;

public record OlxRentPostDTO : OlxGeneralPostDTO
{
    public byte RoomCount { get; init; }
    public double PlaceM2 { get; init; }
    public decimal CondominiumTax { get; init; }
    public decimal IptuTax { get; init; }
    public string ToiletsCount { get; init; }
    public byte CarSpot { get; init; }
    public decimal FullRentPrice { get => base.PostPrice + IptuTax + CondominiumTax; }
    
    public OlxRentPostDTO(string url, string title, decimal postPrice, string location, DateTime timePosted, List<string> images,
        string zipCode, string foundByFilter, byte roomCount, double placeM2, decimal condominiumTax, decimal iptuTax, string toiletsCount, byte carSpot)
        : base(url, title, postPrice, location, zipCode, timePosted, images, foundByFilter)
    {
        RoomCount = roomCount;
        PlaceM2 = placeM2;
        CondominiumTax = condominiumTax;
        IptuTax = iptuTax;
        ToiletsCount = toiletsCount;
        CarSpot = carSpot;
    }

    public OlxRentPostDTO() { }

    public new Type GetPostType()
    {
        return typeof(OlxRentPostDTO);
    }
}
