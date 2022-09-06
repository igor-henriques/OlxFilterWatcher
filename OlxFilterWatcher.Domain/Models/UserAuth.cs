namespace OlxFilterWatcher.Domain.Models;

public sealed record UserAuth : BaseMongoModel
{
    public string Email { get; init; }
    public string Password { get; init; }
    public IEnumerable<string> Roles { get; init; }
        
    public static UserAuth MainUser
    {
        get => new UserAuth() 
        { 
            Email = "contato@ironside.dev", 
            Password = "MyHXMJ3dEMiJQNGHOADzQO4E3bb+1b7sEDh833w624M=", //TTTTTTTTeste1!
            Roles = Enumerable.Empty<string>() 
        };
    } 
}
