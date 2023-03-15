namespace OlxFilterWatcher.Services.Services;

public sealed class MailService : IMailService
{
    private readonly EmailClient _emailClient;
    private readonly string _emailSentFrom;
    private readonly ILogger<MailService> _logger;

    public MailService(
        IConfiguration configuration, 
        ILogger<MailService> logger)
    {
        _emailClient = new EmailClient(configuration.GetConnectionString("AzureEmailService"));
        _emailSentFrom = configuration.GetSection("App:Email:SentFrom").Get<string>();
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string htmlString, IEnumerable<string> emails, string subject, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Start sending emails");

            var mailAddresses = emails.Select(email => new EmailAddress(email));
            var mailMessages = new EmailMessage(
                _emailSentFrom,
                new EmailRecipients(mailAddresses), 
                new EmailContent(subject) 
                { 
                    Html = htmlString 
                });

            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(Azure.WaitUntil.Completed, mailMessages, cancellationToken);
            EmailSendResult statusMonitor = emailSendOperation.Value;

            string operationId = emailSendOperation.Id;
            var emailSendStatus = statusMonitor.Status;

            if (emailSendStatus == EmailSendStatus.Succeeded)
            {
                _logger.LogInformation("Email send operation succeeded with OperationId = {operationId}.\nEmail is out for delivery.", operationId);
                return true;
            }
            else
            {
                var error = statusMonitor.Error;

                _logger.LogInformation("Failed to send email.\n OperationId = {operationId}.\n Status = {emailSendStatus}.", operationId, emailSendStatus);
                _logger.LogInformation("Error Code = {errorCode}, Message = {errorMessage}", error.Code, error.Message);

                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in sending email:\n{exception}", ex.ToString());
            return false;
        }
    }
}
