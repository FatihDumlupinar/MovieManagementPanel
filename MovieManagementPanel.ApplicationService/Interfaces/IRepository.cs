using MovieManagementPanel.Domain.Common;
using System.Linq.Expressions;

namespace MovieManagementPanel.ApplicationService.Interfaces
{
    /// <summary>
    /// Veritabanı komutları
    /// </summary>
    public interface IRepository<T> where T : BaseEntity, new()
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        T? FindOne(Expression<Func<T?, bool>> expression);

        IQueryable<T> Find(Expression<Func<T, bool>>? expression = default);

        Task<T> AddAsyncReturnEntity(T entity, CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }
}
