namespace OlxFilterWatcher.Workers.Workers;

public class OlxNotifyWorker : BackgroundService
{
    private readonly IOlxGeneralPostService olxGeneralPostService;
    private readonly IOlxRentPostService olxRentPostService;
    private readonly IOlxVehiclePostService olxVehiclePostService;
    private readonly IOlxNotificationService notificationService;
    private readonly IMailService mailService;
    private readonly IWorkerSettingsService workerSettingsService;
    private string BaseHTML;
    private const string emailSubject = "Acabamos de encontrar mais um post na OLX!";

    public OlxNotifyWorker(
        IOlxNotificationService notificationService,
        IMailService mailService,
        IOlxGeneralPostService olxGeneralPostService,
        IOlxRentPostService olxRentPostService,
        IOlxVehiclePostService olxVehiclePostService,
        IWorkerSettingsService workerSettingsService)
    {
        this.olxGeneralPostService = olxGeneralPostService;
        this.olxVehiclePostService = olxVehiclePostService;
        this.olxRentPostService = olxRentPostService;
        this.notificationService = notificationService;
        this.mailService = mailService;
        this.olxGeneralPostService = olxGeneralPostService;
        this.olxVehiclePostService = olxVehiclePostService;
        this.olxRentPostService = olxRentPostService;
        this.workerSettingsService = workerSettingsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {            
            this.BaseHTML = await HtmlBuilder.GetBaseHTML(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!await CheckWorkerStateAsync(stoppingToken))
                {
                    Log.Information($"{nameof(OlxNotifyWorker)} isn't active. Re-checking...");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    continue;
                }

                CancellationTokenSource cts = new();

                cts.CancelAfter(TimeSpan.FromMinutes(3));

                var itemsToNotify = (await notificationService.GetAsync(cts.Token)).Distinct();

                if (!itemsToNotify.Any())
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cts.Token);
                    continue;
                }

                foreach (var item in itemsToNotify)
                {
                    dynamic getPostTask = item.URL switch
                    {
                        var localFilter when localFilter.Contains(HtmlElements.AutosPecasLink) & !localFilter.Contains("/pecas-e-acessorios") => olxVehiclePostService.GetAsync(item.URL, cts.Token),
                        var localFilter when localFilter.Contains("/imoveis")                                                      => olxRentPostService.GetAsync(item.URL, cts.Token),
                        _                                                                                                          => olxGeneralPostService.GetAsync(item.URL, cts.Token)
                    };

                    dynamic post = await getPostTask;

                    var emailsToSentEmail = FilterEmailsAlreadyNotified(item);

                    if (await NotifyAsync(emailsToSentEmail, post, item.URL, cts.Token))
                    {
                        item.EmailsNotifiedCallback(emailsToSentEmail);
                        await notificationService.UpdateAsync(item, cts.Token);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cts.Token);
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Notification service terminated unexpectedly");
        }
    }

    private async Task<bool> CheckWorkerStateAsync(CancellationToken cancellationToken)
    {
        return await workerSettingsService.IsWorkerActive(nameof(OlxNotifyWorker), cancellationToken);
    }

    private async Task<bool> NotifyAsync(IEnumerable<string> emails, dynamic post, string url, CancellationToken cancellationToken)
    {
        try
        {
            return await mailService.SendEmailAsync(
                HtmlBuilder.BuildHtml(BaseHTML, post, url), emails,
                emailSubject, 
                cancellationToken);
        }
        catch (Exception) { return false; }
    }

    private static IEnumerable<string> FilterEmailsAlreadyNotified(OlxNotificationDTO notification)
    {
        foreach (var notifyCheck in notification.NotifyChecks)
        {
            if (!notifyCheck.IsNotified)
                yield return notifyCheck.Email;
        }
    }
}
