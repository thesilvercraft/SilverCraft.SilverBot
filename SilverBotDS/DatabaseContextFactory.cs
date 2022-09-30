using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SilverBotDS.Objects.Database;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        //should only be used by Add-Migration
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlite("Data Source=test.db", b => b.MigrationsAssembly("SilverBotDS"));
        return new DatabaseContext(optionsBuilder.Options);
    }
}