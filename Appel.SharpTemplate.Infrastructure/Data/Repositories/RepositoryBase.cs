using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Infrastructure.Data.Repositories;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : BaseEntity
{
    protected readonly DbSet<TEntity> DbSet;
    private readonly SharpTemplateContext _context;

    public RepositoryBase(IDbContextFactory<SharpTemplateContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
        DbSet = _context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null)
    {
        var query = DbSet.AsQueryable();

        if (filter != null)
            query = query
                .Where(filter);

        return await query.ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        DbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    //public ValueTask DisposeAsync() => _context.DisposeAsync();
}
