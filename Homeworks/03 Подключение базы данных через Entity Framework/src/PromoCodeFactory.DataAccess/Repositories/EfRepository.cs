using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;
    private DbSet<T> _dbSet = context.Set<T>();

    private IQueryable<T> Query(bool withIncludes)
    {
        var query = _dbSet.AsQueryable();
        return withIncludes ? ApplyIncludes(query) : query;
    }

    public async Task Add(
        T entity,
        CancellationToken ct)
    {
        _dbSet.Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(
        Guid id,
        CancellationToken ct)
    {
        var entity = await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null)
            throw new EntityNotFoundException(typeof(T), id);

        _dbSet.Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetAll(
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        return await Query(withIncludes)
            .ToListAsync(ct);
    }

    public async Task<T?> GetById(
        Guid id,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        return await Query(withIncludes)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(
        IEnumerable<Guid> ids,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        return await Query(withIncludes)
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(
        Expression<Func<T, bool>> predicate,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        return await Query(withIncludes)
            .Where(predicate)
            .ToListAsync(ct);
    }

    public async Task Update(
        T entity,
        CancellationToken ct)
    {
        _dbSet.Update(entity);
        await context.SaveChangesAsync(ct);
    }
}
