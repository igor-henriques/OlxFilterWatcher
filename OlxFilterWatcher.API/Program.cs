var builder = WebApplication.CreateBuilder();

SerilogExtension.AddSerilogApi();
builder.Host.UseSerilog(Log.Logger);

try
{
    var mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDb");

    builder.Services.AddCors();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "OlxFilterWatch",
            Version = "v1",
            Description = "API para inclusão e exclusão de filtros para notificador da OLX",
            Contact = new OpenApiContact() { Name = "Ironside DEV" }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Utilize 'Bearer <TOKEN>'",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>()
        }
    });

        c.EnableAnnotations();
    });

    builder.Services.AddHealthChecks();
    builder.Services.AddSingleton<IMongoService<OlxFilter>, MongoService<OlxFilter>>(x => new(mongoDbConnectionString));
    builder.Services.AddSingleton<IMongoService<WorkerSettings>, MongoService<WorkerSettings>>(x => new(mongoDbConnectionString));
    builder.Services.AddSingleton<IMapper<OlxFilterDTO, OlxFilter>, OlxFilterMapper>();
    builder.Services.AddSingleton<IMapper<UserAuthDTO, UserAuth>, UserAuthMapper>();
    builder.Services.AddSingleton<IOlxFilterService, OlxFilterService>();
    builder.Services.AddSingleton<IUserAuthService, UserAuthService>();
    builder.Services.AddSingleton<ITokenGeneratorService, TokenGeneratorService>();
    builder.Services.AddSingleton<IWorkerSettingsService, WorkerSettingsService>();
    builder.Services.AddSingleton<IValidator<OlxFilterDTO>, OlxFilterDTOValidator>();
    builder.Services.AddSingleton<IValidator<UserAuthDTO>, UserAuthDTOValidator>();

    builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(jwt =>
    {
        jwt.RequireHttpsMetadata = true;
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JwtBearerKey"))),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapHealthChecks("/olx-watcher/health");
    app.UseCors(c => c.AllowAnyOrigin());
    app.UseHttpsRedirection();
    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapFiltersEndpoints();
    app.MapAuthenticationEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
