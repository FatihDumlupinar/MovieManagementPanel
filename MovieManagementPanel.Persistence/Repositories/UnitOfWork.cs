using MovieManagementPanel.ApplicationService.Interfaces;
using MovieManagementPanel.Domain.Entities;
using MovieManagementPanel.Persistence.Contexts;

namespace MovieManagementPanel.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Ctor&Fields
        
        private readonly AppDbContext _appDbContext;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<MovieAndSaloon> _movieAndSaloonRepository;
        private readonly IRepository<Saloon> _saloonRepository;
        private readonly IRepository<User> _userRepository;

        public UnitOfWork(IRepository<Movie> movieRepository, IRepository<MovieAndSaloon> movieAndSaloonRepository, IRepository<Saloon> saloonRepository, IRepository<User> userRepository, AppDbContext appDbContext)
        {
            _movieRepository = movieRepository;
            _movieAndSaloonRepository = movieAndSaloonRepository;
            _saloonRepository = saloonRepository;
            _userRepository = userRepository;
            _appDbContext = appDbContext;
        }

        #endregion

        #region Properties
        
        public IRepository<Movie> Movies => _movieRepository;

        public IRepository<MovieAndSaloon> MoviesAndSaloons => _movieAndSaloonRepository;

        public IRepository<Saloon> Saloons => _saloonRepository;

        public IRepository<User> Users => _userRepository;

        #endregion

        #region Methods

        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            await using var dbContextTransaction = _appDbContext.Database.BeginTransaction();
            try
            {
                await _appDbContext.SaveChangesAsync(cancellationToken);
                await dbContextTransaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                return false;
            }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _appDbContext.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        } 

        #endregion
    }
}
