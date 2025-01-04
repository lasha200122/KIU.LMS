namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL.Base;

public interface IBaseRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default);
    T Update(T entity);
    void UpdateRange(ICollection<T> entities);

    void Remove(T entity);
    void RemoveRange(ICollection<T> entities);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultWithTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TResult?> FirstOrDefaultMappedAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultSortedAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? sortPredicate, bool isDescending, params Expression<Func<T, object>>[] includeProperties);
    Task<T?> FirstOrDefaultIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
    Task<T?> FirstOrDefaultWithTrackingIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
    Task<T?> SingleOrDefaultWithTrackingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

    Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ICollection<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<ICollection<T>> GetWhereIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
    Task<ICollection<T>> GetWhereAsTrackingIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
    Task<ICollection<T>> GetWhereAsTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<ICollection<TResult>> GetMappedAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default);
    Task<ICollection<TResult>> GetAsync<TResult>(Expression<Func<T, TResult>> select);
    Task<ICollection<TResult>> GetSortedAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, object>>? sortPredicate, bool isDescending);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountWithPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> ExecuteDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<PagedEntities<T>> GetPaginatedAsync(int pageNumber, int itemsPerPage, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default);
    Task<PagedEntities<T>> GetPaginatedWhereAsync(Expression<Func<T, bool>> predicate, int pageNumber, int itemsPerPage, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default);
    Task<PagedEntities<TResult>> GetPaginatedMappedAsync<TResult>(int pageNumber, int itemsPerPage, Expression<Func<T, TResult>> select, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default);
    Task<PagedEntities<TResult>> GetPaginatedWhereMappedAsync<TResult>(Expression<Func<T, bool>> predicate, int pageNumber, int itemsPerPage, Expression<Func<T, TResult>> select, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default);
}
