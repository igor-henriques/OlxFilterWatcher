namespace OlxFilterWatcher.Services.Services;

public sealed class MailService : IMailService
{
    private const string ApiKey = "";
    private const string sentFrom = "";
    private readonly RestClient client;
    public MailService()
    {
        this.client = new RestClient("https://emailapi.netcorecloud.net");
    }
    public async Task<bool> SendEmailAsync(string htmlString, IEnumerable<string> emails, string subject, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest("/v5/mail/send", Method.Post);
        request.AddHeader("api_key", ApiKey);
        request.AddHeader("content-type", "application/json");

        var emailObject = new
        {
            from = new
            {
                email = sentFrom,
                name = "noreply"
            },
            subject,
            content = new[]
            {
                new
                {
                    type = "html",
                    value = htmlString
                }
            },
            personalizations = new[]
            {
                new
                {
                    to = emails.Select(email =>
                    {
                        return new
                        {
                            email,
                            name = string.Empty
                        };
                    })
                }
            }
        };

        request.AddParameter("application/json", JsonSerializer.Serialize(emailObject), ParameterType.RequestBody);

        var response = await client.ExecuteAsync(request, cancellationToken);

        return response.ResponseStatus == ResponseStatus.Completed;
    }
}
