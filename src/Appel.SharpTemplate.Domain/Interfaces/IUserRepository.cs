using Appel.SharpTemplate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>> filter = null);
    Task<User> GetByIdAsync(int id);
    Task AddAsync(User entity);
    Task UpdateAsync(User entity);
}
