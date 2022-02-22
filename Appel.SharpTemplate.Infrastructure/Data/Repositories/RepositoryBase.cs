using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Infrastructure.Data.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : BaseEntity
    {
        public readonly DbSet<TEntity> DbSet;
        private readonly SharpTemplateContext _context;

        public RepositoryBase(SharpTemplateContext context)
        {
            DbSet = context.Set<TEntity>();
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = DbSet.AsQueryable().AsNoTracking();

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

    }
}
