using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Domain.Interfaces
{
    public interface IRepositoryBase<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null);
        Task<TEntity> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
