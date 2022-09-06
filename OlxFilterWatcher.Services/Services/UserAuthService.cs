namespace OlxFilterWatcher.Services.Services;

public class UserAuthService : IUserAuthService
{
    private readonly IMapper<UserAuthDTO, UserAuth> mapper;

    public UserAuthService(IMapper<UserAuthDTO, UserAuth> mapper)
    {
        this.mapper = mapper;
    }

    public async Task<UserAuth> GetUser(UserAuthDTO user, CancellationToken cancellationToken = default)
    {
        var userAuth = mapper.Map(user with { Password = EncryptPassword(user.Password) });

        if (userAuth.Email.Equals(UserAuth.MainUser.Email) & userAuth.Password.Equals(UserAuth.MainUser.Password))
            return await Task.FromResult(userAuth);

        return await Task.FromResult<UserAuth>(null);
    }

    public async Task<bool> CheckUserExists(UserAuthDTO user, CancellationToken cancellationToken = default)
    {
        var userAuth = mapper.Map(user with { Password = EncryptPassword(user.Password) });

        //Implementar verificação em dbo corretamente para execução em prod
        //Para fins de teste/didáticos, desta forma está funcional
        if (userAuth.Equals(UserAuth.MainUser))
            return await Task.FromResult(true);

        return await Task.FromResult(false);
    }

    private string EncryptPassword(string password)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Array.Empty<byte>(),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return hashed;
    }
}
