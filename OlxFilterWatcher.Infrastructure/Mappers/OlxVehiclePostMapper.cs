namespace OlxFilterWatcher.Infrastructure.Mappers;

public class OlxVehiclePostMapper : IMapper<OlxVehiclePostDTO, OlxVehiclePost>
{
    public OlxVehiclePostDTO Map(OlxVehiclePost source)
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
            Year = source.Year,
            KmCount = source.KmCount,
            Transmission = source.Transmission
        };
    }

    public OlxVehiclePost Map(OlxVehiclePostDTO source)
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
            source.Year,
            source.KmCount,
            source.Transmission);
    }

    public IEnumerable<OlxVehiclePostDTO> Map(IEnumerable<OlxVehiclePost> source)
    {
        return source.Select(x => Map(x));
    }

    public IEnumerable<OlxVehiclePost> Map(IEnumerable<OlxVehiclePostDTO> source)
    {
        return source.Select(x => Map(x));
    }
}
