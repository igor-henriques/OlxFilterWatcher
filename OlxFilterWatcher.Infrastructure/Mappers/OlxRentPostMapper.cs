namespace OlxFilterWatcher.Infrastructure.Mappers;

public class OlxRentPostMapper : IMapper<OlxRentPostDTO, OlxRentPost>
{
    public OlxRentPostDTO Map(OlxRentPost source)
    {
        if (source is null)
            return null;

        return new()
        {
            URL = source.URL,
            Title = source.Title,
            PostPrice = source.PostPrice,
            Location = source.Location,
            TimePosted = source.TimePosted,
            Images = source.Images,
            ZipCode = source.ZipCode,
            FoundByFilter = source.FoundByFilter,
            RoomCount = source.RoomCount,
            PlaceM2 = source.PlaceM2,
            CondominiumTax = source.CondominiumTax,
            IptuTax = source.IptuTax,
            ToiletsCount = source.ToiletsCount,
            CarSpot = source.CarSpot
        };
    }

    public OlxRentPost Map(OlxRentPostDTO source)
    {
        if (source is null)
            return null;

        return new(source.URL,
            source.Title,
            source.PostPrice,
            source.Location,
            source.TimePosted,
            source.Images,
            source.ZipCode,
            source.FoundByFilter,
            source.RoomCount,
            source.PlaceM2,
            source.CondominiumTax,
            source.IptuTax,
            source.ToiletsCount,
            source.CarSpot);
    }

    public IEnumerable<OlxRentPostDTO> Map(IEnumerable<OlxRentPost> source)
    {
        return source.Select(x => Map(x));
    }

    public IEnumerable<OlxRentPost> Map(IEnumerable<OlxRentPostDTO> source)
    {
        return source.Select(x => Map(x));
    }
}
