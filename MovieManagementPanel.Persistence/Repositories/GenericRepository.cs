using Microsoft.EntityFrameworkCore;
using MovieManagementPanel.ApplicationService.Interfaces;
using MovieManagementPanel.Domain.Common;
using MovieManagementPanel.Persistence.Contexts;
using System.Linq.Expressions;

namespace MovieManagementPanel.Persistence.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        protected readonly AppDbContext _appDbContext;
        protected readonly DbSet<T> _entities;

        public GenericRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _entities = appDbContext.Set<T>();
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _entities.AddAsync(entity, cancellationToken);
        }

        public async Task<T> AddAsyncReturnEntity(T entity, CancellationToken cancellationToken = default)
        {
            var data = await _entities.AddAsync(entity, cancellationToken);
            return data.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _entities.AddRangeAsync(entities, cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                entity.IsActive = false;
                _entities.Update(entity);
            }, cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                foreach (var entity in entities)
                {
                    entity.IsActive = false;
                }
                _entities.UpdateRange(entities);
            }, cancellationToken);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>>? expression = null)
        {
            return expression != null ? _entities.Where(expression) : _entities;
        }

        public T? FindOne(Expression<Func<T?, bool>> expression)
        {
            return _entities.FirstOrDefault(expression);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _entities.FirstOrDefaultAsync(i => i.IsActive == true && i.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                _entities.Update(entity);
            }, cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                _entities.UpdateRange(entities);
            }, cancellationToken);
        }
    }
}
