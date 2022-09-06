namespace OlxFilterWatcher.Domain.Models;

[BsonIgnoreExtraElements]
public sealed record OlxRentPost : OlxGeneralPost
{
    public byte RoomCount { get; init; }
    public double PlaceM2 { get; init; }
    public decimal CondominiumTax { get; init; }
    public decimal IptuTax { get; init; }
    public string ToiletsCount { get; init; }
    public byte CarSpot { get; init; }
    public decimal FullRentPrice { get => PostPrice + IptuTax + CondominiumTax; }

    public OlxRentPost(string url, string title, decimal value, string location, DateTime timePosted,
        List<string> images, string zipCode, string foundByFilter, byte roomCount, double placeM2,
        decimal condominiumTax, decimal iptuTax, string toiletsCount, byte carSpot)
        : base(url, title, value, location, timePosted, images, zipCode, foundByFilter)
    {
        RoomCount = roomCount;
        PlaceM2 = placeM2;
        CondominiumTax = condominiumTax;
        IptuTax = iptuTax;
        ToiletsCount = toiletsCount;
        CarSpot = carSpot;
    }

    public OlxRentPost() { }
}
