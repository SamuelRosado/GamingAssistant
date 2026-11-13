using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace GamingAssistant.Infrastructure.Persistence
{
    public class SamiraDbContextFactory: IDesignTimeDbContextFactory<SamiraDbContext>
    {
        public SamiraDbContext CreateDbContext(string[] args)
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "SAMIRA"
            );

            Directory.CreateDirectory(appDataPath);

            string databasePath = Path.Combine(
                appDataPath,
                "samira.db"
            );

            var optionsBuilder =
                new DbContextOptionsBuilder<SamiraDbContext>();

            optionsBuilder.UseSqlite(
                $"Data Source={databasePath}"
            );

            return new SamiraDbContext(optionsBuilder.Options);
        }
    }
}