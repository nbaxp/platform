using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wta.Application.Default.Data;

public class DefaultDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
{
    public DefaultDbContext CreateDbContext(string[] args)
    {
        //WtaApplication.Run<Startup>(args);
        var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
        optionsBuilder.UseSqlite("Data Source=wta.db");
        return new DefaultDbContext(optionsBuilder.Options);
    }
}
