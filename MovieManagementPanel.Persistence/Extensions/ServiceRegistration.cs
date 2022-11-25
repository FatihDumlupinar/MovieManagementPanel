using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieManagementPanel.ApplicationService.Interfaces;
using MovieManagementPanel.Persistence.Contexts;
using MovieManagementPanel.Persistence.Repositories;

namespace MovieManagementPanel.Persistence.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            serviceCollection.AddDbContext<AppDbContext>(options =>
                {
                    options.UseLazyLoadingProxies();
                    options.UseSqlServer(configuration.GetConnectionString("SQLConnection"));
                });

            serviceCollection.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));
            serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
