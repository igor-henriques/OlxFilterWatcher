namespace OlxFilterWatcher.Infrastructure.Mappers;

public class OlxFilterMapper : IMapper<OlxFilterDTO, OlxFilter>
{
    public OlxFilterDTO Map(OlxFilter source)
    {
        if (source is null)
            return null;

        return new OlxFilterDTO
        {
            Emails = source.Emails,
            Filter = source.Filter,
        };
    }

    public OlxFilter Map(OlxFilterDTO source)
    {
        if (source is null)
            return null;

        return new OlxFilter
        {
            Emails = source.Emails,
            Filter = source.Filter
        };
    }

    public IEnumerable<OlxFilterDTO> Map(IEnumerable<OlxFilter> source)
    {
        return source.Select(x => Map(x));
    }

    public IEnumerable<OlxFilter> Map(IEnumerable<OlxFilterDTO> source)
    {
        return source.Select(x => Map(x));
    }
}
