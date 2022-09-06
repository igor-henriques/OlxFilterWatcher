var builder = WebApplication.CreateBuilder(args);

var mongoDbConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:MongoDb");
var connectionString = builder.Configuration.GetConnectionString("AppDataContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDataContextConnection' not found.");

builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDataContext>();

builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddSingleton<IMapper<OlxFilterDTO, OlxFilter>, OlxFilterMapper>();
builder.Services.AddSingleton<IMongoService<OlxFilter>, MongoService<OlxFilter>>(x => new(mongoDbConnectionString));
builder.Services.AddSingleton<IOlxFilterService, OlxFilterService>();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

app.Run();