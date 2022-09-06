namespace OlxFilterWatcher.Infrastructure.Mappers;

public class OlxNotificationMapper : IMapper<OlxNotificationDTO, OlxNotification>
{
    public OlxNotificationDTO Map(OlxNotification source)
    {
        if (source is null)
            return null;

        return new OlxNotificationDTO()
        {
            Filter = source.Filter,
            NotifyChecks = source.NotifyChecks,
            URL = source.URL,
        };
    }

    public OlxNotification Map(OlxNotificationDTO source)
    {
        if (source is null)
            return null;

        return new OlxNotification()
        {
            Filter = source.Filter,
            NotifyChecks = source.NotifyChecks,
            URL = source.URL
        };
    }

    public IEnumerable<OlxNotificationDTO> Map(IEnumerable<OlxNotification> source)
    {
        return source.Select(x => Map(x));
    }

    public IEnumerable<OlxNotification> Map(IEnumerable<OlxNotificationDTO> source)
    {
        return source.Select(x => Map(x));
    }
}
