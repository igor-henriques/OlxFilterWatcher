namespace OlxFilterWatcher.Infrastructure.Mappers;

public class UserAuthMapper : IMapper<UserAuthDTO, UserAuth>
{
    public UserAuthDTO Map(UserAuth source)
    {
        return new UserAuthDTO
        {
            Email = source.Email,
            Password = source.Password
        };
    }

    public UserAuth Map(UserAuthDTO source)
    {
        return new UserAuth
        {
            Email = source.Email,
            Password = source.Password,
            Roles = Enumerable.Empty<string>()
        };
    }

    public IEnumerable<UserAuthDTO> Map(IEnumerable<UserAuth> source)
    {
        return source.Select(x => Map(x));
    }

    public IEnumerable<UserAuth> Map(IEnumerable<UserAuthDTO> source)
    {
        return source.Select(x => Map(x));
    }
}
