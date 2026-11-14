using Microsoft.EntityFrameworkCore;
using Test.Data;

namespace Test.Tests.Helpers
{
    /// <summary>
    /// Creates a fresh in-memory AppDbContext for each test so tests are fully isolated.
    /// </summary>
    public static class DbContextFactory
    {
        public static AppDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
