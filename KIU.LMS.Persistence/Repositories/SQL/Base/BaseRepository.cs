namespace KIU.LMS.Persistence.Repositories.SQL.Base;

public class BaseRepository<T>(DbContext Context) : IBaseRepository<T> where T : class
{
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var result = await Context.Set<T>().AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddRangeAsync(entities, cancellationToken);
    }

    public T Update(T entity)
    {
        var result = Context.Set<T>().Update(entity);
        return result.Entity;
    }

    public void UpdateRange(ICollection<T> entities)
    {
        Context.Set<T>().UpdateRange(entities);
    }

    public void Remove(T entity)
    {
        Context.Set<T>().Remove(entity);
    }

    public void RemoveRange(ICollection<T> entities)
    {
        Context.Set<T>().RemoveRange(entities);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> FirstOrDefaultWithTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AsTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TResult?> FirstOrDefaultMappedAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AsNoTracking().Where(predicate).Select(select).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> FirstOrDefaultSortedAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? sortPredicate, bool isDescending, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = Context.Set<T>().AsNoTracking().AsQueryable();
        query = includeProperties.Aggregate(query, (current, property) => current.Include(property));
        if (sortPredicate is not null)
        {
            query = isDescending ? query.OrderByDescending(sortPredicate) : query.OrderBy(sortPredicate);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> FirstOrDefaultIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = Context.Set<T>().AsNoTracking();

        foreach (var property in includeProperties)
        {
            query = query.Include(property);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> FirstOrDefaultWithTrackingIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = Context.Set<T>().AsQueryable();

        foreach (var property in includeProperties)
        {
            query = query.Include(property);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = Context.Set<T>().AsNoTracking();

        foreach (var property in includeProperties)
        {
            query = query.Include(property);
        }

        return await query.SingleOrDefaultAsync(predicate);
    }

    public async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<ICollection<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<T>> GetWhereIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = Context.Set<T>().AsNoTracking();

        foreach (var property in includeProperties)
        {
            query = query.Include(property);
        }

        return await query.Where(predicate).ToListAsync();
    }

    public async Task<ICollection<T>> GetWhereAsTrackingIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = Context.Set<T>().AsTracking();

        foreach (var property in includeProperties)
        {
            query = query.Include(property);
        }

        return await query.Where(predicate).ToListAsync();
    }

    public async Task<ICollection<T>> GetWhereAsTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<T>().AsTracking();

        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<TResult>> GetMappedAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AsNoTracking().Where(predicate).Select(select).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<TResult>> GetAsync<TResult>(Expression<Func<T, TResult>> select)
    {
        return await Context.Set<T>().AsNoTracking().Select(select).ToListAsync();
    }

    public async Task<ICollection<TResult>> GetSortedAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, object>>? sortPredicate, bool isDescending)
    {
        var query = Context.Set<T>().AsNoTracking().AsQueryable();

        if (sortPredicate != null)
        {
            query = isDescending ? query.OrderByDescending(sortPredicate) : query.OrderBy(sortPredicate);
        }

        return await query.Select(select).ToListAsync();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().CountAsync(cancellationToken);
    }

    public async Task<int> CountWithPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> ExecuteDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<PagedEntities<T>> GetPaginatedAsync(int pageNumber, int itemsPerPage, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<T>().AsNoTracking();

        if (orderBy is not null)
        {
            query = query.OrderBy(orderBy);
        }

        return new PagedEntities<T>(
            await query
                .Paginate(pageNumber, itemsPerPage)
                .ToListAsync(cancellationToken),
            await query.CountAsync(cancellationToken));
    }

    public async Task<PagedEntities<T>> GetPaginatedWhereAsync(Expression<Func<T, bool>> predicate, int pageNumber, int itemsPerPage, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<T>()
            .AsNoTracking()
            .Where(predicate);

        if (orderBy is not null)
        {
            query = query.OrderBy(orderBy);
        }

        return new PagedEntities<T>(
            await query
                .Paginate(pageNumber, itemsPerPage)
                .ToListAsync(cancellationToken),
            await query.CountAsync(cancellationToken));
    }

    public async Task<PagedEntities<TResult>> GetPaginatedMappedAsync<TResult>(int pageNumber, int itemsPerPage, Expression<Func<T, TResult>> select, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<T>()
            .AsNoTracking();

        if (orderBy is not null)
        {
            query = query.OrderBy(orderBy);
        }

        return new PagedEntities<TResult>(
            await query
                .Paginate(pageNumber, itemsPerPage)
                .Select(select)
                .ToListAsync(cancellationToken),
            await query.CountAsync(cancellationToken));
    }

    public async Task<PagedEntities<TResult>> GetPaginatedWhereMappedAsync<TResult>(Expression<Func<T, bool>> predicate, int pageNumber, int itemsPerPage, Expression<Func<T, TResult>> select, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<T>()
            .AsNoTracking()
            .Where(predicate);

        if (orderBy is not null)
        {
            query = query.OrderBy(orderBy);
        }

        return new PagedEntities<TResult>(
            await query
                .Paginate(pageNumber, itemsPerPage)
                .Select(select)
                .ToListAsync(cancellationToken),
            await query.CountAsync(cancellationToken));
    }
}
