using MovieManagementPanel.Domain.Entities;

namespace MovieManagementPanel.ApplicationService.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<Movie> Movies { get; }
        public IRepository<MovieAndSaloon> MoviesAndSaloons { get; }
        public IRepository<Saloon> Saloons { get; }
        public IRepository<User> Users { get; }

        Task<bool> CommitAsync(CancellationToken cancellationToken = default);
    }
}
