namespace OlxFilterWatcher.Web.Data;

public class AppDataContext : IdentityDbContext<User>
{
    public AppDataContext(DbContextOptions<AppDataContext> options)
        : base(options)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseMySql("Server=dbo.ironside.dev;Database=olxfilteruser;Uid=root;Pwd=95653549Hh**;SSL Mode=None", ServerVersion.AutoDetect("Server=dbo.ironside.dev;Database=olxfilteruser;Uid=root;Pwd=95653549Hh**;SSL Mode=None"));

    //    base.OnConfiguring(optionsBuilder);
    //}
}
