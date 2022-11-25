using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieManagementPanel.Persistence.Contexts;

namespace MovieManagementPanel.Persistence.Seed
{
    public static class SeedData
    {
        public static async Task SeedAsync(this IServiceProvider services)
        {
            using var cancelTokenSource = new CancellationTokenSource();

            using var scope = services.CreateScope();

            using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await dbContext.Database.MigrateAsync(cancelTokenSource.Token);

            if (!await dbContext.Users.AnyAsync(cancelTokenSource.Token))
            {
                await dbContext.Users.AddAsync(new()
                {
                    Email="test",
                    FullName="Test User",
                    Password="test"
                }, cancelTokenSource.Token);

                await dbContext.SaveChangesAsync(cancelTokenSource.Token);
            }

        }

    }
}
