SerilogExtension.AddSerilogApi();

const string mongoDbConnectionString = "";

try
{
    await Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton<IMongoService<OlxGeneralPost>, MongoService<OlxGeneralPost>>(x => new(mongoDbConnectionString));
            services.AddSingleton<IMongoService<OlxVehiclePost>, MongoService<OlxVehiclePost>>(x => new(mongoDbConnectionString));
            services.AddSingleton<IMongoService<OlxRentPost>, MongoService<OlxRentPost>>(x => new(mongoDbConnectionString));

            services.AddSingleton<IMapper<OlxGeneralPostDTO, OlxGeneralPost>, OlxGeneralPostMapper>();
            services.AddSingleton<IMapper<OlxRentPostDTO, OlxRentPost>, OlxRentPostMapper>();
            services.AddSingleton<IMapper<OlxVehiclePostDTO, OlxVehiclePost>, OlxVehiclePostMapper>();

            services.AddSingleton<IOlxRentPostService, OlxRentPostService>();
            services.AddSingleton<IOlxVehiclePostService, OlxVehiclePostService>();
            services.AddSingleton<IOlxGeneralPostService, OlxGeneralPostService>();

            services.AddSingleton<IMongoService<OlxFilter>, MongoService<OlxFilter>>(x => new(mongoDbConnectionString));            
            services.AddSingleton<IMongoService<OlxNotification>, MongoService<OlxNotification>>(x => new(mongoDbConnectionString));
            services.AddSingleton<IMongoService<WorkerSettings>, MongoService<WorkerSettings>>(x => new(mongoDbConnectionString));
            services.AddSingleton<IMapper<OlxFilterDTO, OlxFilter>, OlxFilterMapper>();            
            services.AddSingleton<IMapper<OlxNotificationDTO, OlxNotification>, OlxNotificationMapper>();
            services.AddSingleton<IMapper<UserAuthDTO, UserAuth>, UserAuthMapper>();
            services.AddSingleton<IOlxFilterService, OlxFilterService>();
            services.AddSingleton<IOlxNotificationService, OlxNotificationService>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<IUserAuthService, UserAuthService>();
            services.AddSingleton<IFilterHandlerService, FilterHandlerService>();
            services.AddSingleton<ITokenGeneratorService, TokenGeneratorService>();
            services.AddSingleton<IWorkerSettingsService, WorkerSettingsService>();
            services.AddSingleton<IValidator<OlxFilterDTO>, OlxFilterDTOValidator>();
            services.AddSingleton<IValidator<UserAuthDTO>, UserAuthDTOValidator>();

            services.AddHostedService<OlxScraperWorker>();
            services.AddHostedService<OlxNotifyWorker>();

        }).UseSerilog()
          .Build()
          .RunAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "Workers terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}