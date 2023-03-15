namespace OlxFilterWatcher.Workers.Workers;

public class OlxScraperWorker : BackgroundService
{
    private readonly IFilterHandlerService filterHandler;
    private readonly IOlxNotificationService olxNotificationService;
    private readonly IOlxGeneralPostService olxGeneralPostService;
    private readonly IOlxRentPostService olxRentPostService;
    private readonly IOlxVehiclePostService olxVehiclePostService;
    private readonly IOlxFilterService olxFilterService;
    private readonly IWorkerSettingsService workerSettingsService;

    public OlxScraperWorker(
        IOlxFilterService olxFilters,
        IFilterHandlerService filterHandler,
        IOlxNotificationService olxNotificationService,
        IOlxGeneralPostService olxGeneralPostService,
        IOlxRentPostService olxRentPostService,
        IOlxVehiclePostService olxVehiclePostService,
        IWorkerSettingsService workerSettingsService)
    {
        this.olxFilterService = olxFilters;
        this.filterHandler = filterHandler;
        this.olxNotificationService = olxNotificationService;
        this.olxGeneralPostService = olxGeneralPostService;
        this.olxRentPostService = olxRentPostService;
        this.olxVehiclePostService = olxVehiclePostService;
        this.olxGeneralPostService = olxGeneralPostService;
        this.olxRentPostService = olxRentPostService;
        this.olxVehiclePostService = olxVehiclePostService;
        this.workerSettingsService = workerSettingsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!await CheckWorkerStateAsync(stoppingToken))
                {
                    Log.Information($"{nameof(OlxScraperWorker)} isn't active. Re-checking...");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    continue;
                }

                filterHandler.UpdateFilters(await olxFilterService.GetFiltersAsync(stoppingToken));

                if (!(await olxFilterService.GetFiltersAsync(stoppingToken)).Any())
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                await ProcessFilters(olxFilterService.Filters);

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Scrap service terminated unexpectedly");
        }
    }

    private async Task<bool> CheckWorkerStateAsync(CancellationToken cancellationToken)
    {
        return await workerSettingsService.IsWorkerActive(nameof(OlxScraperWorker), cancellationToken);
    }

    private async ValueTask ProcessFilters(IEnumerable<string> filters)
    {
        try
        {
            List<Task> tasks = new();

            Parallel.ForEach(filters, (string filter) =>
            {
                filterHandler.StartTimer(filter);

                if (filterHandler.IsFilterReady(filter))
                {
                    tasks.Add(BuildTask(filter));
                }
            });

            if (!tasks.Any())
                return;

            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while processing filters");
        }
    }

    private Task BuildTask(string filter)
    {
        CancellationTokenSource cts = new();

        cts.CancelAfter(TimeSpan.FromMinutes(3));

        return Task.Run(async () =>
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(filter, cts.Token);

            var rawTextPosts = doc.DocumentNode
                                 ?.SelectNodes(HtmlElements.PostDivXPath)
                                 ?.Where(d => !string.IsNullOrEmpty(d.InnerText))
                                 ?.ToList();

            bool isVehiclePost = filter.Contains("/autos-e-pecas") & !filter.Contains("/pecas-e-acessorios");
            bool isRentPost = filter.Contains("/imoveis");

            var lastPostUrl = await olxGeneralPostService.GetLastPostUrlByFilterAsync(filter, cts.Token);

            for (int index = 0; index < rawTextPosts.Count; index++)
            {
                try
                {
                    var currentPost = rawTextPosts.ElementAt(index);
                    var currentPostUrl = GetPostUrl(currentPost);

                    if (!lastPostUrl.IsNullOrEmpty(out bool isFirstPost))
                    {
                        if (lastPostUrl.Equals(currentPostUrl))
                            break;                        
                    }

                    lastPostUrl ??= currentPostUrl;

                    var postTaskBuilder = filter switch
                    {
                        string localFilter when localFilter.Contains("/autos-e-pecas") & !localFilter.Contains("/pecas-e-acessorios") => BuildVehiclePost((currentPost, rawTextPosts.IndexOf(currentPost)), filter, currentPostUrl, cts.Token),
                        string localFilter when localFilter.Contains("/imoveis") => BuildRentPost((currentPost, rawTextPosts.IndexOf(currentPost)), filter, currentPostUrl, cts.Token),
                        _ => BuildGeneralPost((currentPost, rawTextPosts.IndexOf(currentPost)), filter, currentPostUrl, cts.Token)
                    };

                    await postTaskBuilder;

                    if (isFirstPost)
                        break;
                }
                catch (ApplicationException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unexpected error while scrapping olx");
                    break;
                }
            }

            filterHandler.ResetTimer(filter);

            GC.Collect(generation: 2, mode: GCCollectionMode.Optimized, blocking: true);
        }, cts.Token);
    }

    private async Task BuildRentPost((HtmlNode post, int index) record, string filter, string postUrl, CancellationToken cancellationToken)
    {
        if (await olxRentPostService.AnyAsync(postUrl, cancellationToken))
            throw new ApplicationException("Record exists already");

        var web = new HtmlWeb();

        var title = GetPostTitle(record.post);
        var price = GetPostPrice(record.post, record.index);
        var postGeneralInformations = GetPostDetailedInformations(record.post, record.index);
        var roomCount = (byte)GetPostRoomCount(postGeneralInformations);
        var placeM2 = GetPostPlaceM2(postGeneralInformations);
        var condominiumTax = GetPostCondominiumTax(postGeneralInformations);
        var carSpot = (byte)GetCarSpotCount(postGeneralInformations);
        var location = GetPostLocation(record.post, record.index);
        var timePosted = GetPostTimePosted(record.post, record.index).ToDateTime();

        var postPage = await web.LoadFromWebAsync(postUrl, cancellationToken);
        var postImages = !GetPostImageUrl(record.post, record.index).Contains(HtmlElements.NotFound) ? GetPostDetailedImagesUrl(postPage.DocumentNode, title) : default;
        var toiletsCount = GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.Banheiros) ?? "NÃO INFORMADO";
        var iptuTax = (GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.IPTU)?.ToDecimal()).GetValueOrDefault();
        var zipCode = GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.CEP);

        OlxRentPostDTO newOlxPost = new(            
            postUrl,
            title,
            price,
            location,
            timePosted,
            postImages,
            zipCode,
            filter,
            roomCount,
            placeM2,
            condominiumTax,
            iptuTax,
            toiletsCount,
            carSpot);

        OlxNotificationDTO notification = new()
        {
            Filter = filter,
            URL = newOlxPost.URL,
            NotifyChecks = (await olxFilterService.GetEmailsByFilterAsync(filter, cancellationToken))
                                                  .Select(email => new EmailNotifyCheck(email, false))
                                                  .ToList()
        };

        await olxRentPostService.AddAsync(newOlxPost, cancellationToken);
        await olxNotificationService.AddAsync(notification, cancellationToken);
    }
    private async Task BuildVehiclePost((HtmlNode post, int index) record, string filter, string postUrl, CancellationToken cancellationToken)
    {
        if (await olxVehiclePostService.AnyAsync(postUrl, cancellationToken))
            throw new ApplicationException("Record exists already");

        var web = new HtmlWeb();

        var title = GetPostTitle(record.post);
        var postImage = GetPostImageUrl(record.post, record.index);
        var price = GetPostPrice(record.post, record.index);
        var postGeneralInformations = GetPostDetailedInformations(record.post, record.index);
        var location = GetPostLocation(record.post, record.index);
        var timePosted = GetPostTimePosted(record.post, record.index).ToDateTime();
        var postPage = await web.LoadFromWebAsync(postUrl, cancellationToken);
        var postImages = !postImage.Contains(HtmlElements.NotFound) ? GetPostDetailedImagesUrl(postPage.DocumentNode, title) : default;
        var zipCode = GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.CEP);
        var vehicleYear = GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.Ano) ?? "NÃO INFORMADO";
        var vehicleKmCount = GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.Quilometragem) ?? "NÃO INFORMADO";
        var vehicleTransmission = GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.Cambio) ?? "NÃO INFORMADO";

        OlxVehiclePostDTO newOlxPost = new(
            postUrl,
            title,
            price,
            location,
            timePosted,
            postImages,
            zipCode,
            filter,
            vehicleYear,
            vehicleKmCount,
            vehicleTransmission
            );

        OlxNotificationDTO notification = new()
        {
            Filter = filter,
            URL = newOlxPost.URL,
            NotifyChecks = (await olxFilterService.GetEmailsByFilterAsync(filter, cancellationToken))
                                                  .Select(email => new EmailNotifyCheck(email, false))
                                                  .ToList()
        };

        await olxVehiclePostService.AddAsync(newOlxPost, cancellationToken);
        await olxNotificationService.AddAsync(notification, cancellationToken);
    }

    private async Task BuildGeneralPost((HtmlNode post, int index) record, string filter, string postUrl, CancellationToken cancellationToken)
    {
        if (await olxGeneralPostService.AnyAsync(postUrl, cancellationToken))
            throw new ApplicationException("Record exists already");

        var web = new HtmlWeb();

        var title = GetPostTitle(record.post);
        var postImage = GetPostImageUrl(record.post, record.index);
        var price = GetPostPrice(record.post, record.index);
        var location = GetPostLocation(record.post, record.index);
        var timePosted = GetPostTimePosted(record.post, record.index).ToDateTime();
        var postPage = await web.LoadFromWebAsync(postUrl, cancellationToken);
        var postImages = !postImage.Contains(HtmlElements.NotFound) ? GetPostDetailedImagesUrl(postPage.DocumentNode, title) : default;
        var zipCode = GetInformationFromPostPage(postPage.DocumentNode, HtmlElements.CEP);

        OlxGeneralPostDTO newOlxPost = new(
            postUrl,
            title,
            price,
            location,
            zipCode,
            timePosted,
            postImages,            
            filter);

        OlxNotificationDTO notification = new()
        {
            Filter = filter,
            URL = newOlxPost.URL,
            NotifyChecks = (await olxFilterService.GetEmailsByFilterAsync(filter, cancellationToken))
                                                  .Select(email => new EmailNotifyCheck(email, false))
                                                  .ToList()
        };

        await olxGeneralPostService.AddAsync(newOlxPost, cancellationToken);
        await olxNotificationService.AddAsync(notification, cancellationToken);
    }

    private static string GetPostTitle(HtmlNode node)
    {
        return node.GetAttributeValue(HtmlElements.Title, HtmlElements.Empty);
    }
    private static string GetPostUrl(HtmlNode node)
    {
        return node.GetAttributeValue(HtmlElements.Href, HtmlElements.Empty);
    }
    private static string GetPostImageUrl(HtmlNode node, int index)
    {
        return node.SelectNodes(HtmlElements.PostImageXPath)
                  ?.ElementAt(index)
                  ?.GetAttributeValue(HtmlElements.Src, HtmlElements.Empty);
    }
    private static decimal GetPostPrice(HtmlNode node, int index)
    {
        return (decimal)node.SelectNodes(HtmlElements.PostPriceXPath)
                           ?.Where(d => d.GetAttributeValue(HtmlElements.AriaLabel, HtmlElements.Empty).Contains(HtmlElements.PrecoDoItem, StringComparison.CurrentCultureIgnoreCase))
                           ?.ElementAt(index)
                           ?.InnerText
                           ?.ToDecimal();
    }
    private static List<string> GetPostDetailedInformations(HtmlNode node, int index)
    {
        var postGeneralInformations = node.SelectNodes(HtmlElements.PostDetailedInformationXPath)
                                         ?.Where(d => d.GetAttributeValue(HtmlElements.AriaLabel, HtmlElements.Empty).Equals(HtmlElements.InformacoesAnuncio, StringComparison.CurrentCultureIgnoreCase))
                                         ?.ElementAt(index)
                                         ?.InnerText;

        return postGeneralInformations.Split(HtmlElements.SplitCharacter)
                                      .Select(p => p.ToUpper().Trim())
                                      .Where(p => !string.IsNullOrEmpty(p))
                                      .ToList();
    }
    private static int GetPostRoomCount(List<string> detailedInformations)
    {
        return detailedInformations.Where(x => x.Contains(HtmlElements.Quartos, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToInt();
    }
    private static int GetPostPlaceM2(List<string> detailedInformations)
    {
        return detailedInformations.Where(x => x.Contains(HtmlElements.M2, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToInt();
    }
    private static decimal GetPostCondominiumTax(List<string> detailedInformations)
    {
        return detailedInformations.Where(x => x.Contains(HtmlElements.Condominio, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToDecimal();
    }
    private static int GetCarSpotCount(List<string> detailedInformations)
    {
        return detailedInformations.Where(x => x.Contains(HtmlElements.Vaga, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToInt();
    }
    private static string GetPostLocation(HtmlNode node, int index)
    {
        return node.SelectNodes(HtmlElements.PostLocationXPath)
                  ?.Where(d => d.GetAttributeValue(HtmlElements.AriaLabel, HtmlElements.Empty).Contains(HtmlElements.Localizacao))
                  ?.ElementAt(index)
                  ?.InnerText;
    }
    private static string GetPostTimePosted(HtmlNode node, int index)
    {
        return node.SelectNodes(HtmlElements.PostTimePostedXPath)
                  ?.Where(d => d.GetAttributeValue(HtmlElements.AriaLabel, HtmlElements.Empty).Contains(HtmlElements.AnuncioPublicadoEm))
                  ?.ElementAt(index)
                  ?.InnerText;
    }
    private static List<string> GetPostDetailedImagesUrl(HtmlNode node, string title)
    {
        return node.SelectNodes(HtmlElements.PostDetailedImagesXPath)
                  ?.Where(d => d.GetAttributeValue(HtmlElements.Alt, HtmlElements.Empty).Contains(title))
                  ?.Select(d => d.GetAttributeValue(HtmlElements.Src, HtmlElements.Empty))
                  ?.ToList();
    }
    private static string GetInformationFromPostPage(HtmlNode node, string innerText)
    {
        return node.SelectNodes(HtmlElements.Dt)
                  ?.Where(d => d.InnerText.Equals(innerText))
                  ?.FirstOrDefault()
                  ?.NextSibling
                  ?.InnerText;
    }
}