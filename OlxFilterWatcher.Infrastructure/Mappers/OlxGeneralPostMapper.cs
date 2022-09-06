namespace OlxFilterWatcher.Infrastructure.Mappers;

public class OlxGeneralPostMapper : IMapper<OlxGeneralPostDTO, OlxGeneralPost>
{
    public OlxGeneralPostDTO Map(OlxGeneralPost source)
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
            FoundByFilter = source.FoundByFilter
        };
    }

    public OlxGeneralPost Map(OlxGeneralPostDTO source)
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
            source.FoundByFilter);
    }

    public IEnumerable<OlxGeneralPostDTO> Map(IEnumerable<OlxGeneralPost> source)
    {
        return source.Select(x => Map(x));
    }

    public IEnumerable<OlxGeneralPost> Map(IEnumerable<OlxGeneralPostDTO> source)
    {
        return source.Select(x => Map(x));
    }
}
